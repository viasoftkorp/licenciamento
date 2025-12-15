using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Messages;
using Viasoft.Licensing.LicenseServer.Domain.Services;
using Viasoft.Licensing.LicenseServer.Domain.Services.LicensingManager;
using Viasoft.Licensing.LicenseServer.Domain.Services.TenantLicensing;
using Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;

namespace Viasoft.Licensing.LicenseServer.Domain.Catalogs
{
    public class TenantCatalog : ITenantCatalog, ITransientDependency
    {
        private readonly ITenantLicensingService _tenantLicensingService;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IProvideHardwareIdService _provideHardwareIdService;
        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _semaphoreSlims;
        private readonly ILogger<TenantCatalog> _logger;
        private readonly ILicensingManagerServiceFactory _licensingManagerServiceFactory;
        private readonly IMemoryCache _memoryCache;

        private const string LicenseCacheKeyPrefix = "tenant_manager_server_license_";
        private const string TenantsCacheKeyPrefix = "tenant_manager_server_active_tenants";
        
        public TenantCatalog(
            ITenantLicensingService tenantLicensingService, 
            IProvideHardwareIdService provideHardwareIdService, 
            ILogger<TenantCatalog> logger, 
            ILicensingManagerServiceFactory licensingManagerServiceFactory,
            IMemoryCache memoryCache)
        {
            // Não modificar o tipo do param Logger pois ele é passado para outras classes...
            _provideHardwareIdService = provideHardwareIdService;
            _logger = logger;
            _licensingManagerServiceFactory = licensingManagerServiceFactory;
            _memoryCache = memoryCache;
            _semaphoreSlims = new ConcurrentDictionary<Guid, SemaphoreSlim>();
            _tenantLicensingService = tenantLicensingService;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task<List<LicenseTenantStatusCurrent>> GetAllTenantCurrentLicenseStatus()
        {
            if (!DefaultConfigurationConsts.IsRunningAsLegacy)
            {
                _logger.LogError("Tenativa de utilização de GetAllTenantCurrentLicenseStatus com IsRunningAsLegacy = false");
                throw new NotSupportedException("Não é possível utilizar GetAllTenantCurrentLicenseStatus quando IsRunningAsLegacy = false");
            }
            
            var legacySettings = LicenseServerSettingsExtension.LoadSettings();
            if (legacySettings == null)
                throw new NotSupportedException();
            
            await _semaphoreSlim.WaitAsync();

            //clona o catálogo para uma variável local pois precisamos iterar no dicionario
            //se alguma chamada estiver adicionando um tenant novo, teriamos problema na iteração
            Dictionary<Guid, ILicensingManagerService> localCatalog;
            try
            {
                localCatalog = GetAllCachedTenants();
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            var results = new List<LicenseTenantStatusCurrent>();
            foreach (var (tenantId, _) in localCatalog)
            {
                //não está pegando lock no semáforo pois o próprio método GetTenantCurrentLicenseStatus já faz isso
                var currentState = await GetTenantCurrentLicenseStatus(tenantId);
                results.Add(currentState);
            }
            
            //isso daqui está sendo feito pois pode ser que tenham tenants que ainda não foram carregados
            //por exemplo, o servidor de licenças está configurado para 5 tenants, porém somente houve consumo de 3
            //portanto os outros 2 tenants ainda não estão no catalogo
            var configuredTenants = LicenseServerSettingsExtension.GetTenantLegacyDatabases();
            var tenantsIdentifiers = results.Select(x => x.TenantDetails.Identifier)
                .ToList();
            var remainingTenants = configuredTenants.Where(t => !tenantsIdentifiers.Contains(t.TenantId))
                .ToList();

            foreach (var legacyTenantMapping in remainingTenants)
            {
                //não está pegando lock no semáforo pois o próprio método GetTenantCurrentLicenseStatus já faz isso
                var currentState = await GetTenantCurrentLicenseStatus(legacyTenantMapping.TenantId);
                //vai ser nulo quando não conseguir carregar o tenant do licenciamento
                if(currentState != null)
                    results.Add(currentState);
            }
            
            return results;
        }

        public async Task<LicenseTenantStatusCurrent> GetTenantCurrentLicenseStatus(Guid tenantId)
        {
            EnsureTenantInSemaphores(tenantId);
            
            var cacheKey = GetTenantCacheKey(tenantId);
            if (_memoryCache.TryGetValue(cacheKey, out ILicensingManagerService service))
            {
                var licensingManagerSemaphore = _semaphoreSlims[tenantId];
                await licensingManagerSemaphore.WaitAsync();
                try
                {
                    return service.GetCurrentState();
                }
                finally
                {
                    licensingManagerSemaphore.Release();
                }
            }

            //isso daqui vai ser nulo quando não for possível carregar as licenças desse tenant
            var licensingManager = await GetTenantLicensingManager(tenantId);
            if (licensingManager != null)
            {
                var licensingManagerSemaphore = _semaphoreSlims[tenantId];
                await licensingManagerSemaphore.WaitAsync();
                try
                {
                    return licensingManager.GetCurrentState();
                }
                finally
                {
                    licensingManagerSemaphore.Release();
                }
            }

            return null;
        }
        
        public async Task RefreshTenantLicensing(LicensingDetailsUpdated licensingDetailsUpdated)
        {
            var cacheKey = GetTenantCacheKey(licensingDetailsUpdated.TenantId);
            if (_memoryCache.TryGetValue(cacheKey, out ILicensingManagerService licensingManager))
            {
                EnsureTenantInSemaphores(licensingDetailsUpdated.TenantId);
                var licensingManagerSemaphore = _semaphoreSlims[licensingDetailsUpdated.TenantId];
                await licensingManagerSemaphore.WaitAsync();
                try
                {
                    // isso é necessário pois as configurações do servidor de licenças para esse tenant podem ter sido alteradas
                    _provideHardwareIdService.Reset();

                    var licensesInUse = licensingManager.GetAllLicensesInUse();
                    var tenantLicensingManager = _licensingManagerServiceFactory.CreateLicensingManagerService(licensingDetailsUpdated.LicenseByIdentifier);
                    await tenantLicensingManager.RestoreLicensesInUse(licensesInUse);

                    await _semaphoreSlim.WaitAsync();
                    try
                    {
                        _memoryCache.Remove(cacheKey);
                        SetTenantCache(licensingDetailsUpdated.TenantId, tenantLicensingManager);
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }
                }
                finally
                {
                    licensingManagerSemaphore.Release();
                }
            }
        }
        
        public async Task RefreshAllTenantsLicensing()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var tenantCatalogDictionary = GetAllCachedTenants();
                
                foreach (var tenantId in tenantCatalogDictionary.Keys)
                {
                    var cacheKey = GetTenantCacheKey(tenantId);
                    _memoryCache.Remove(cacheKey);
                }
                
                foreach (var (tenantId, licensingManagerService) in tenantCatalogDictionary)
                {
                    var licensesInUse = licensingManagerService.GetAllLicensesInUse();
                    
                    var tenantLicensing = await _tenantLicensingService.GetTenantLicensing(tenantId);
                    if (tenantLicensing == null)
                        continue;
                    
                    var tenantDetails = new LicenseByTenantId(tenantLicensing);
                    var tenantLicensingManager = _licensingManagerServiceFactory.CreateLicensingManagerService(tenantDetails);
                    await tenantLicensingManager.RestoreLicensesInUse(licensesInUse);
                    
                    var cacheKey = GetTenantCacheKey(tenantId);
                    SetTenantCache(tenantId, tenantLicensingManager);
                }
                
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
        
        public async Task<ConsumeLicenseOutput> ConsumeLicense(ConsumeLicenseInput input)
        {
            var licensingManager = await GetTenantLicensingManager(input.TenantId);
            if (licensingManager == null)
                return new ConsumeLicenseOutput(ConsumeAppLicenseStatus.TenantLicensingNotLoaded, null);

            EnsureTenantInSemaphores(input.TenantId);
            
            var licensingManagerSemaphore = _semaphoreSlims[input.TenantId];
            
            await licensingManagerSemaphore.WaitAsync();
            try
            {
                return await licensingManager.ConsumeLicense(input);
            }
            finally
            {
                licensingManagerSemaphore.Release();
            }
        }

        public async Task<ReleaseLicenseOutput> ReleaseLicense(ReleaseLicenseInput input)
        {
            var licensingManager = await GetTenantLicensingManager(input.TenantId);
            if (licensingManager == null)
                return new ReleaseLicenseOutput(ReleaseAppLicenseStatus.TenantLicensingNotLoaded, null, null);
            
            EnsureTenantInSemaphores(input.TenantId);
            
            var licensingManagerSemaphore = _semaphoreSlims[input.TenantId];
            
            await licensingManagerSemaphore.WaitAsync();
            try
            {
                return await licensingManager.ReleaseLicense(input);
            }
            finally
            {
                licensingManagerSemaphore.Release();
            }
        }
        
        public async Task<List<LicenseUsageInRealTimeRawData>> GetTenantsLicensesUsageInRealTime()
        {
            //leitura em todos os estados dos licenciamentos, por isso o lock é no semáforo global
            await _semaphoreSlim.WaitAsync();
            try
            {
                var tenantCatalogDictionary = GetAllCachedTenants();

                var tenantsLicensesUsageInRealTime = tenantCatalogDictionary.Select(l => new LicenseUsageInRealTimeRawData
                    {
                        TenantId = l.Key,
                        LicenseUsageInRealTimeDetails = l.Value.GetCurrentState().GetLicenseUsageInRealTime(),
                        SoftwareUtilized = l.Value.GetCurrentState().GetLicenseUsageInRealTime().Select(l => l.SoftwareIdentifier).Distinct().ToList()
                    })
                    .ToList();
            
                return tenantsLicensesUsageInRealTime;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task ReleaseLicenseBasedOnHeartbeat()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var tenantCatalogDictionary = GetAllCachedTenants();
                
                foreach (var (tenantId, licensingManager) in tenantCatalogDictionary)
                {
                    //tenant may not be in semaphore if a license 
                    EnsureTenantInSemaphores(tenantId);
                    var licensingManagerSemaphore = _semaphoreSlims[tenantId];
                    await licensingManagerSemaphore.WaitAsync();
                    try
                    {
                        await licensingManager.EvaluateAndReleaseLicensesBasedOnHeartbeat();
                    }
                    finally
                    {
                        licensingManagerSemaphore.Release();
                    }
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<RefreshAppLicenseInUseByUserOutput> RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInput input)
        {
            var licensingManager = await GetTenantLicensingManager(input.TenantId);
            if (licensingManager == null)
                return new RefreshAppLicenseInUseByUserOutput { Status = RefreshAppLicenseInUseByUserStatus.TenantLicensingNotLoaded };

            EnsureTenantInSemaphores(input.TenantId);
            
            var licensingManagerSemaphore = _semaphoreSlims[input.TenantId];

            await licensingManagerSemaphore.WaitAsync();
            try
            {
                return await licensingManager.RefreshAppLicenseInUseByUser(input);
            }
            finally
            {
                licensingManagerSemaphore.Release();
            }
        }
        
        private async Task<ILicensingManagerService> GetTenantLicensingManager(Guid tenantId)
        {
            var cacheKey = GetTenantCacheKey(tenantId);
            if (_memoryCache.TryGetValue(cacheKey, out ILicensingManagerService tenantLicensingManager)) 
                return tenantLicensingManager;
            
            await _semaphoreSlim.WaitAsync();
            try 
            {
                // This is needed because if two requests of the same tenantId arrive simultaneously, the second one is going to
                // await on the semaphore, then it will get the object from the cache,
                // so it will not execute the method "GetTenantLicensing" twice for the same tenantId.
                if (_memoryCache.TryGetValue(cacheKey, out ILicensingManagerService tenantLicensingManagerWithinSemaphore)) 
                    return tenantLicensingManagerWithinSemaphore;
                
                var tenantLicensing = await _tenantLicensingService.GetTenantLicensing(tenantId);

                if (tenantLicensing == null)
                    return null;

                var tenantDetails = new LicenseByTenantId(tenantLicensing);
                tenantLicensingManager = _licensingManagerServiceFactory.CreateLicensingManagerService(tenantDetails);
                SetTenantCache(tenantId, tenantLicensingManager);
                
                return tenantLicensingManager;
            } 
            finally 
            {
                _semaphoreSlim.Release();
            }
        }

        private void EnsureTenantInSemaphores(Guid tenantId)
        {
            if (!_semaphoreSlims.ContainsKey(tenantId))
            {
                var semaphore = new SemaphoreSlim(1, 1);
                _semaphoreSlims.TryAdd(tenantId, semaphore);
            }
        }

        private string GetTenantCacheKey(Guid tenantId)
        {
            return $"{LicenseCacheKeyPrefix}{tenantId}";
        }

        private Dictionary<Guid, ILicensingManagerService> GetAllCachedTenants()
        {
            var result = new Dictionary<Guid, ILicensingManagerService>();

            _memoryCache.TryGetValue(TenantsCacheKeyPrefix, out List<Guid> activeTenants);

            if (activeTenants == null)
                return result;

            foreach (var tenantId in activeTenants)
            {
                var cacheKey = GetTenantCacheKey(tenantId);
                if (_memoryCache.TryGetValue(cacheKey, out ILicensingManagerService tenantLicensingManager))
                {
                    result[tenantId] = tenantLicensingManager;
                }

            }
                
            return result;
        }

        private void SetTenantCache(Guid tenantId, ILicensingManagerService tenantLicensingManager)
        {
            var cacheKey = GetTenantCacheKey(tenantId);
            _memoryCache.Set(cacheKey, tenantLicensingManager);
            
            if (!_memoryCache.TryGetValue(TenantsCacheKeyPrefix, out List<Guid> activeTenants))
            {
                activeTenants = new List<Guid> { tenantId };
                _memoryCache.Set(TenantsCacheKeyPrefix, activeTenants);
            }
            else if (!activeTenants.Contains(tenantId))
            {
                activeTenants.Add(tenantId);
                _memoryCache.Set(TenantsCacheKeyPrefix, activeTenants);
            }
        }
    }
}