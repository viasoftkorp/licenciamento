using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Old.Messages;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicensingManager;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.TenantLicensing;
using Viasoft.Licensing.LicenseServer.Shared.Enums;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Catalogs
{
    public class TenantCatalog : Dictionary<Guid, ILicensingManagerService>
    {
        private readonly ILicenseUsageService _licenseUsageService;
        private readonly ITenantLicensingService _tenantLicensingService;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IConfiguration _configuration;
        private readonly IProvideHardwareIdService _provideHardwareIdService;

        public TenantCatalog(IServiceProvider serviceProvider, IConfiguration configuration, IProvideHardwareIdService provideHardwareIdService)
        {
            _configuration = configuration;
            _provideHardwareIdService = provideHardwareIdService;
            _licenseUsageService  = serviceProvider.GetService<ILicenseUsageService>();
            _tenantLicensingService = serviceProvider.GetService<ITenantLicensingService>();
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task<LicenseTenantStatusCurrentOld> GetTenantCurrentLicenseStatus(Guid tenantId)
        {
            return TryGetValue(tenantId, out var service) 
                ? service.GetCurrentState() 
                : (await GetTenantLicensingManager(tenantId))?.GetCurrentState();
        }
        
        private async Task<ILicensingManagerService> GetTenantLicensingManager(Guid tenantId)
        {
            if (tenantId == Guid.Empty)
                return null;
            
            if (TryGetValue(tenantId, out var tenantLicensingManager)) 
                return tenantLicensingManager;
            
            await _semaphoreSlim.WaitAsync();
            try 
            {
                // This is needed because if two requests of the same tenantId arrive simultaneously, the second one is going to
                // await on the semaphore, then it will get the object from the dictionary,
                // so it will not execute the method "GetTenantLicensing" twice for the same tenantId.
                if (TryGetValue(tenantId, out var tenantLicensingManagerWithinSemaphore)) 
                    return tenantLicensingManagerWithinSemaphore;
                
                var tenantLicensing = await _tenantLicensingService.GetTenantLicensing(tenantId);
                
                if (tenantLicensing == null)
                    return null;
                
                tenantLicensingManager = new LicensingManagerService(tenantLicensing, _licenseUsageService, _configuration, _provideHardwareIdService);
                Add(tenantId, tenantLicensingManager);
                return tenantLicensingManager;
            } 
            finally 
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<ConsumeLicenseOutputOld> ConsumeLicense(ConsumeLicenseInput input)
        {
            var licensingManager = await GetTenantLicensingManager(input.TenantId);
            if (licensingManager == null)
                return new ConsumeLicenseOutputOld(ConsumeAppLicenseStatusOld.TenantLicensingNotLoaded, null);
            ConsumeLicenseOutputOld outputOld;
            lock (licensingManager)
                outputOld = licensingManager.ConsumeLicense(input);
            return outputOld;
        }

        public async Task<ReleaseLicenseOutputOld> ReleaseLicense(ReleaseLicenseInput input)
        {
            var licensingManager = await GetTenantLicensingManager(input.TenantId);
            if (licensingManager == null)
                return new ReleaseLicenseOutputOld(ReleaseAppLicenseStatusOld.TenantLicensingNotLoaded, null, null);
            ReleaseLicenseOutputOld outputOld;
            lock (licensingManager)
                outputOld = licensingManager.ReleaseLicense(input);
            return outputOld;
        }
        
        public IEnumerable<LicenseUsageInRealTime> GetTenantsLicensesUsageInRealTime()
        {
            var tenantCatalogDictionary = this.ToDictionary(catalog => catalog.Key, catalog => catalog.Value);
            
            var tenantsLicensesUsageInRealTime = tenantCatalogDictionary.Select(l => new LicenseUsageInRealTime
                {
                    TenantId = l.Key,
                    LicenseUsageInRealTimeDetails = l.Value.GetCurrentState().GetLicenseUsageInRealTime()
                })
                .ToList();

            foreach (var licenseUsageInRealTime in tenantsLicensesUsageInRealTime)
                licenseUsageInRealTime.SoftwareUtilized = licenseUsageInRealTime.LicenseUsageInRealTimeDetails
                                                                                    .Select(l => l.SoftwareIdentifier)
                                                                                    .Distinct()
                                                                                    .ToList();
            return tenantsLicensesUsageInRealTime;
        }

        public async Task ReleaseLicenseBasedOnHeartbeat()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                foreach (var licensesUsageService in Values)
                    lock (licensesUsageService)
                        licensesUsageService.EvaluateAndReleaseLicensesBasedOnHeartbeat();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<RefreshAppLicenseInUseByUserOutputOld> RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInputOld inputOld)
        {
            var licensingManager = await GetTenantLicensingManager(inputOld.TenantId);
            if (licensingManager == null)
                return new RefreshAppLicenseInUseByUserOutputOld { Status = RefreshAppLicenseInUseByUserStatusOld.TenantLicensingNotLoaded };
            RefreshAppLicenseInUseByUserOutputOld outputOld;
            lock (licensingManager)
                outputOld = licensingManager.RefreshAppLicenseInUseByUser(inputOld);
            return outputOld;
        }

        public async Task<TenantLicensedAppsOutput> GetTenantLicensedApps(Guid tenantId)
        {
            var licensingManager = await GetTenantLicensingManager(tenantId);
            if (licensingManager == null)
                return new TenantLicensedAppsOutput { Status = TenantLicensedAppsStatus.TenantNotFound };

            var output = licensingManager.GetTenantLicensedApps();
            return output;
        }

        public async Task<TenantLicenseStatusOutput> GetTenantLicenseStatus(Guid tenantId)
        {
            var licensingManager = await GetTenantLicensingManager(tenantId);
            if (licensingManager == null)
                return new TenantLicenseStatusOutput { Status = TenantLicenseStatus.TenantNotFound };

            var output = licensingManager.GetTenantLicenseStatus();
            return  new TenantLicenseStatusOutput { Status = output };
        }

        public async Task<IsTenantCnpjLicensedOutput> IsTenantCnpjLicensed(Guid tenantId, string cnpj)
        {
            var licensingManager = await GetTenantLicensingManager(tenantId);
            if (licensingManager == null)
                return new IsTenantCnpjLicensedOutput { IsCnpjLicensed = false };
            
            var output = licensingManager.IsTenantCnpjLicensed(cnpj);
            return new IsTenantCnpjLicensedOutput { IsCnpjLicensed = output };
        }

        public async Task RefreshAllTenantsLicensing()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var tenantCatalogDictionary = this.ToDictionary(catalog => catalog.Key, catalog => catalog.Value);
                Clear();
                foreach (var tenant in tenantCatalogDictionary)
                {
                    var licensesInUse = tenant.Value.GetAllLicensesInUse();
                    
                    var tenantLicensing = await _tenantLicensingService.GetTenantLicensing(tenant.Key);
                    if (tenantLicensing == null)
                        continue;

                    var tenantLicensingManager = new LicensingManagerService(tenantLicensing, _licenseUsageService, _configuration, _provideHardwareIdService);
                    tenantLicensingManager.RestoreLicensesInUse(licensesInUse);
                    Add(tenant.Key, tenantLicensingManager);
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task RefreshTenantLicensing(LicensingDetailsUpdated licensingDetailsUpdated)
        {
            if (TryGetValue(licensingDetailsUpdated.TenantId, out var licensingManager))
            {
                await _semaphoreSlim.WaitAsync();
                try
                {
                    if (licensingManager.GetLastUpdatedDateTime() >= licensingDetailsUpdated.UpdatedDateTime)
                        return;
                    Remove(licensingDetailsUpdated.TenantId);
                    var licensesInUse = licensingManager.GetAllLicensesInUse();
                    var tenantLicensingManager = new LicensingManagerService(licensingDetailsUpdated.LicenseByIdentifier, _licenseUsageService, _configuration, _provideHardwareIdService);
                    tenantLicensingManager.RestoreLicensesInUse(licensesInUse);
                    Add(licensingDetailsUpdated.TenantId, tenantLicensingManager);
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
        }
    }
}