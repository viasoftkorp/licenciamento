using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.App;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.LicenseBatchRepository;
using Viasoft.Licensing.LicensingManagement.Domain.Services.BatchOperationServices.BatchOperationLoggerService;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.BatchOperationServices
{
    public class LicensedTenantBatchOperationsService: ILicensedTenantBatchOperationsService, ITransientDependency
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILicensedTenantCacheService _cacheService;
        private readonly ILicensedTenantService _licensedTenantService;
        private readonly IAppRepository _appRepository;
        private readonly ILicenseBatchRepository _licenseBatchRepository;
        private readonly ILicenseRepository _licenseRepository;
        private readonly IBundleRepository _bundleRepository;
        private readonly IRepository<BundledApp> _bundledApp;
        private readonly IBatchOperationLoggerService _batchOperationLoggerServiceService;
        public LicensedTenantBatchOperationsService(IUnitOfWork unitOfWork, ILicensedTenantCacheService cacheService,
            ILicensedTenantService licensedTenantService, IAppRepository appRepository,
            ILicenseBatchRepository licenseBatchRepository, ILicenseRepository licenseRepository,
            IBundleRepository bundleRepository, IRepository<BundledApp> bundledApp, IBatchOperationLoggerService batchOperationLoggerServiceService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _licensedTenantService = licensedTenantService;
            _appRepository = appRepository;
            _licenseBatchRepository = licenseBatchRepository;
            _licenseRepository = licenseRepository;
            _bundleRepository = bundleRepository;
            _bundledApp = bundledApp;
            _batchOperationLoggerServiceService = batchOperationLoggerServiceService;
        }
        
        public async Task InsertAppsFromBundlesInLicenses(List<LicensedBundleApp> licensedBundleApps)
        {
            var appsByBundles = GroupAppsByBundle(licensedBundleApps);

            var licensedBundlesWithUnLicensedApps = await GetLicensedBundlesWithUnLicensedApps(appsByBundles);

            if (licensedBundlesWithUnLicensedApps.Count == 0)
            {
                return;
            }
            
            var licenseTenantIds = licensedBundlesWithUnLicensedApps.Select(r => r.LicensedTenantId).Distinct().ToList();
            var licensedTenantIdToIdentifier = await _licenseBatchRepository.GetLicensedTenantToIdentifierDictionary(licenseTenantIds);
            var alreadyLicensedAppsByLicensedTenantId = new List<AlreadyLicensedApp>();
            var licenseTenantIdentifiers = new List<Guid>();
            var appIdentifiers = new List<string>();
            
            using (_unitOfWork.Begin())
            {
                foreach (var licensedBundleWithUnLicensedApps in licensedBundlesWithUnLicensedApps)
                {
                    foreach (var currentAppIdToLicense in licensedBundleWithUnLicensedApps.AppsGetForBatchOperations)
                    {
                        var alreadyLicensed = alreadyLicensedAppsByLicensedTenantId.Any(l => l.AppId == currentAppIdToLicense.Id && l.LicensedTenantId == licensedBundleWithUnLicensedApps.LicensedTenantId);
                        if (!alreadyLicensed)
                        {
                            var licenseIdentifier = licensedTenantIdToIdentifier[licensedBundleWithUnLicensedApps.LicensedTenantId];
                            if (!licenseTenantIdentifiers.Exists(id => id == licenseIdentifier))
                                licenseTenantIdentifiers.Add(licenseIdentifier);
                            
                            if (!appIdentifiers.Exists(detail => detail == currentAppIdToLicense.Identifier))
                                appIdentifiers.Add(currentAppIdToLicense.Identifier);
                            
                            var bundle = new LicensedBundleCreateInput
                            {
                                Status = licensedBundleWithUnLicensedApps.Status,
                                BundleId = licensedBundleWithUnLicensedApps.BundleId,
                                LicensedTenantId = licensedBundleWithUnLicensedApps.LicensedTenantId,
                                NumberOfLicenses = licensedBundleWithUnLicensedApps.NumberOfLicenses
                            };
                            alreadyLicensedAppsByLicensedTenantId.Add(new AlreadyLicensedApp{ AppId = currentAppIdToLicense.Id, LicensedTenantId = licensedBundleWithUnLicensedApps.LicensedTenantId});
                            await _licensedTenantService.AddBundledAppsToLicense(currentAppIdToLicense.Id, bundle);
                        }
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                await _batchOperationLoggerServiceService.LogInsertAppsFromBundlesInLicenses(appIdentifiers, licenseTenantIdentifiers);
                await _cacheService.InvalidateCacheForTenants(licenseTenantIdentifiers);
                await _licensedTenantService.PublishLicensingDetailsUpdatedEvents(licenseTenantIdentifiers);
                await _unitOfWork.CompleteAsync();
            }
        }
        
        public async Task RemoveAppFromBundleInLicenses(Guid bundleId, Guid appId)
        {
            if (!await _licenseRepository.CheckIfLicensedAppIsDefault(appId))
            {
                var licensedBundlesWithAppsLicensed = await _licenseBatchRepository.GetLicensedBundlesWithAppsLicensed(bundleId, appId);
                
                if (licensedBundlesWithAppsLicensed.Count > 0)
                {
                    var licenseTenantIds = licensedBundlesWithAppsLicensed.Select(r => r.LicensedTenantId).ToList();
                    var licensedTenantIdToIdentifier = await _licenseBatchRepository.GetLicensedTenantToIdentifierDictionary(licenseTenantIds);
                    var licensedAppToDeleteFromLicenses = await _appRepository.GetLicensedAppsForLicenses(licenseTenantIds, appId);
                    var appIdentifier = await _appRepository.GetAppIdentifiersByAppIds(new List<Guid> { appId });
            
                    using (_unitOfWork.Begin())
                    {
                        var licenseTenantIdentifiers = await _licensedTenantService.RemoveAppsFromLicenses(licensedAppToDeleteFromLicenses, licensedTenantIdToIdentifier);
                        await _batchOperationLoggerServiceService.LogRemoveAppFromBundleInLicenses(appIdentifier[0].Identifier, licenseTenantIdentifiers);
                        await _unitOfWork.CompleteAsync();
                    }
                }
            }
        }
        
        public async Task InsertBundlesInLicenses(BatchOperationsInput input)
        {
            var bundles = await GetBundlesById(input.IdsToInsert);
            var licensedTenantIds = await GetLicenseTenantsById(input.IdsWhereTheyWillBeInserted);

            var bundleIds = bundles.Select(b => b.Id).ToList();

            var alreadyLicensedBundle = await _bundleRepository.GetBundlesAlreadyInLicenses(bundleIds, licensedTenantIds);
            if (CheckIfAllBundlesAreAlreadyLicensed(bundles, licensedTenantIds, alreadyLicensedBundle))
            {
                return;
            }

            var alreadyLicensedBundlesDictionary = AlreadyBundlesInLicensesDictionary(alreadyLicensedBundle);
            var bundledApps = await _bundledApp.Where(a => bundleIds.Contains(a.BundleId)).Select(b => new {b.BundleId, b.AppId}).ToListAsync();
            var alreadyLicensedApp = await _appRepository.GetAppsAlreadyLicensed(bundledApps.Select(a => a.AppId).ToList(), licensedTenantIds);
            var licenseTenantIdentifiers = new List<Guid>();
            var bundleIdentifiers = new List<string>();
            var licensedTenantIdToIdentifierDictionary = await _licenseBatchRepository.GetLicensedTenantToIdentifierDictionary(licensedTenantIds);
            
            using (_unitOfWork.Begin())
            {
                foreach (var bundle in bundles)
                {
                    foreach (var licensedTenantId in licensedTenantIds)
                    {
                        var alreadyContainsCurrentBundleForCurrentTenant = alreadyLicensedBundlesDictionary.TryGetValue(licensedTenantId, out var b) && b.Select(l => l.BundleId).ToList().Contains(bundle.Id);
                        if(!alreadyContainsCurrentBundleForCurrentTenant)
                        {
                            var licenseIdentifier = licensedTenantIdToIdentifierDictionary[licensedTenantId];
                            
                            if (!bundleIdentifiers.Exists(detail => detail == bundle.Identifier))
                                bundleIdentifiers.Add(bundle.Identifier);
                            
                            if (!licenseTenantIdentifiers.Exists(id => id == licenseIdentifier))
                                licenseTenantIdentifiers.Add(licenseIdentifier);
                            
                            var newBundleLicensed = new LicensedBundleCreateInput
                            {
                                Status = LicensedBundleStatus.BundleActive,
                                BundleId = bundle.Id,
                                LicensedTenantId = licensedTenantId,
                                NumberOfLicenses = input.NumberOfLicenses
                            };
                            
                            await _licensedTenantService.CreateLicensedBundle(newBundleLicensed);
                           
                            var bundlesById = bundledApps.Where(b => b.BundleId == bundle.Id).ToList();

                            var alreadyLicensedApps = alreadyLicensedApp.Where(l => l.LicensedTenantId == licensedTenantId).Select(l => l.AppId).ToList();

                            var validBundleAppToAdd = bundlesById.Where(a => !alreadyLicensedApps.Contains(a.AppId)).ToList();

                            foreach (var bundledApp in validBundleAppToAdd)
                            { 
                               alreadyLicensedApp.Add(new AlreadyLicensedApp{ AppId = bundledApp.AppId, LicensedTenantId = newBundleLicensed.LicensedTenantId});
                               await _licensedTenantService.AddBundledAppsToLicense(bundledApp.AppId, newBundleLicensed);
                            }    
                        }
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                await _batchOperationLoggerServiceService.LogInsertBundleInLicenses(input.NumberOfLicenses, bundleIdentifiers, licenseTenantIdentifiers);
                await _cacheService.InvalidateCacheForTenants(licenseTenantIdentifiers);
                await _licensedTenantService.PublishLicensingDetailsUpdatedEvents(licenseTenantIdentifiers);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task InsertAppsInLicenses(BatchOperationsInput input)
        {
            var apps = await GetAppsById(input.IdsToInsert);
            var licensedTenantIds = await GetLicenseTenantsById(input.IdsWhereTheyWillBeInserted);
            
            var appIds = apps.Select(b => b.Id).ToList();
            
            var alreadyLicensedApps = await _appRepository.GetAppsAlreadyLicensed(appIds, licensedTenantIds);
            if (CheckIfAllAppsAreAlreadyLicensed(apps, licensedTenantIds, alreadyLicensedApps))
            {
                return;
            }
            
            var alreadyLicensedAppsDictionary = AlreadyAppsInLicensesDictionary(alreadyLicensedApps);
            var licenseTenantIdentifiers = new List<Guid>();
            var appIdentifiers = new List<string>();
            var licensedTenantIdToIdentifierDictionary = await _licenseBatchRepository.GetLicensedTenantToIdentifierDictionary(licensedTenantIds);

            using (_unitOfWork.Begin())
            {
                foreach (var app in apps)
                {
                    foreach (var licensedTenantId in licensedTenantIds)
                    {
                        var alreadyContainsCurrentAppForCurrentTenant = alreadyLicensedAppsDictionary.TryGetValue(licensedTenantId, out var a) && a.Select(l => l.AppId).ToList().Contains(app.Id);
                        if (!alreadyContainsCurrentAppForCurrentTenant)
                        {
                            var licenseIdentifier = licensedTenantIdToIdentifierDictionary[licensedTenantId];
                            
                            if (!appIdentifiers.Exists(detail => detail == app.Identifier))
                                appIdentifiers.Add(app.Identifier);
                            
                            if (!licenseTenantIdentifiers.Exists(id => id == licenseIdentifier))
                                licenseTenantIdentifiers.Add(licenseIdentifier);

                            var newLicensedApp = new LicensedAppCreateInput
                            {
                                LicensedTenantId = licensedTenantId,
                                AppId = app.Id,
                                Status = LicensedAppStatus.AppActive,
                                NumberOfLicenses = input.NumberOfLicenses
                            };
                            
                            await _licensedTenantService.CreateNewLicensedApp(newLicensedApp);
                        }
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                await _batchOperationLoggerServiceService.LogAppsInLicenses(input.NumberOfLicenses, appIdentifiers, licenseTenantIdentifiers);
                await _cacheService.InvalidateCacheForTenants(licenseTenantIdentifiers);
                await _licensedTenantService.PublishLicensingDetailsUpdatedEvents(licenseTenantIdentifiers);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<List<LicensedBundleApp>> InsertAppsInBundles(BatchOperationsInput input)
        {
            var apps = await GetAppsById(input.IdsToInsert);
            var bundles = await GetBundlesById(input.IdsWhereTheyWillBeInserted);
            
            var appIds = apps.Select(b => b.Id).ToList();
            var bundleIds = bundles.Select(b => b.Id).ToList();

            var alreadyLicensedBundle = await _bundleRepository.GetBundlesIdsAlreadyLicensed(bundleIds);
            var alreadyLicensedBundleApp = new List<LicensedBundleApp>();

            var alreadyBundledApp = await _bundleRepository.GetBundledApps(appIds, bundleIds);
            if (CheckIfAllAppsAreAlreadyBundled(apps, bundles, alreadyBundledApp))
            {
                return alreadyLicensedBundleApp;
            }
            var alreadyBundledAppDictionary = AlreadyAppsInBundleDictionary(alreadyBundledApp);

            var bundlesIdentifiers = new List<string>();
            var appIdentifiers = new List<string>();

            using (_unitOfWork.Begin())
            {
                foreach (var bundle in bundles)
                {
                    var currentBundleIsLicensed = alreadyLicensedBundle.Contains(bundle.Id);
                    foreach (var app in apps)
                    {
                        var alreadyContainsCurrentAppForCurrentBundle = alreadyBundledAppDictionary.TryGetValue(bundle.Id, out var a) && a.Select(l => l.AppId).ToList().Contains(app.Id);
                        if (!alreadyContainsCurrentAppForCurrentBundle)
                        {
                            if (!appIdentifiers.Exists(detail => detail == app.Identifier))
                                appIdentifiers.Add(app.Identifier);
                            
                            if (!bundlesIdentifiers.Exists(id => id == bundle.Identifier))
                                bundlesIdentifiers.Add(bundle.Identifier);
                            
                            var newBundledApp = new BundledApp
                            {
                                Id = new Guid(),
                                BundleId = bundle.Id,
                                AppId = app.Id
                            };

                            await _bundledApp.InsertAsync(newBundledApp);

                            if (currentBundleIsLicensed)
                                alreadyLicensedBundleApp.Add(new LicensedBundleApp { AppId = app.Id, BundleId = bundle.Id});
                        }
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                await _batchOperationLoggerServiceService.LogInsertAppsInBundles(appIdentifiers, bundlesIdentifiers);
                await _unitOfWork.CompleteAsync();
            }
            return alreadyLicensedBundleApp;
        }

        private List<LicensedBundleApps> GroupAppsByBundle(List<LicensedBundleApp> licensedBundleApps)
        {
            return licensedBundleApps
                .GroupBy(l => l.BundleId)
                .Select(l => new LicensedBundleApps
                {
                    BundleId = l.Key,
                    AppIds = l.Select(l => l.AppId).ToList()
                })
                .ToList();
        }

        private async Task<List<AppsGetForBatchOperations>> GetAppsById(InsertBatchOperationsInput appsInsertOperation)
        {
            if (!appsInsertOperation.AllSelected)
            {
                return await _appRepository.GetAppsByIdsForBatchOperations(appsInsertOperation.UnselectedList, appsInsertOperation.Ids);
            }

            return await _appRepository.GetAllAppsForBatchOperation(appsInsertOperation.AdvancedFilter);
        }

        private async Task<List<BundlesGetForBatchOperations>> GetBundlesById(InsertBatchOperationsInput bundlesInsertOperation)
        {
            if (!bundlesInsertOperation.AllSelected)
            {
                return await _bundleRepository.GetBundlesByIdsForBatchOperations(bundlesInsertOperation.UnselectedList, bundlesInsertOperation.Ids);
            }

            return await _bundleRepository.GetAllBundlesForBatchOperation(bundlesInsertOperation.AdvancedFilter);
        }
        
        private async Task<List<Guid>> GetLicenseTenantsById(InsertBatchOperationsInput licensesInsertOperation)
        {
            if (!licensesInsertOperation.AllSelected)
            {
                return await _licenseRepository.GetLicensesByIdsForBatchOperations(licensesInsertOperation.UnselectedList, licensesInsertOperation.Ids);
            }

            return await _licenseRepository.GetAllLicensesForBatchOperations(licensesInsertOperation.AdvancedFilter);
        }

        private bool CheckIfAllBundlesAreAlreadyLicensed(List<BundlesGetForBatchOperations> bundles, List<Guid> licensedTenantIds, List<LicensedBundleGetForBatchOperation> alreadyLicensedBundle)
        {
            return alreadyLicensedBundle.Count == bundles.Count * licensedTenantIds.Count;
        }
        
        private bool CheckIfAllAppsAreAlreadyLicensed(List<AppsGetForBatchOperations> apps, List<Guid> licensedTenantIds, List<AlreadyLicensedApp> alreadyLicensedApps)
        {
            return alreadyLicensedApps.Count == apps.Count * licensedTenantIds.Count;
        }
        
        private bool CheckIfAllAppsAreAlreadyBundled(List<AppsGetForBatchOperations> apps, List<BundlesGetForBatchOperations> bundles, List<BundledApp> alreadyBundledApps)
        {
            return alreadyBundledApps.Count == apps.Count * bundles.Count;
        }

        private Dictionary<Guid, List<LicensedBundleGetForBatchOperation>> AlreadyBundlesInLicensesDictionary(List<LicensedBundleGetForBatchOperation> alreadyLicensedBundle)
        {
            return alreadyLicensedBundle
                .Select(l => new LicensedBundleGetForBatchOperation
                {
                    BundleId = l.BundleId,
                    LicensedTenantId = l.LicensedTenantId
                })
                .GroupBy(l => l.LicensedTenantId)
                .ToList()
                .ToDictionary(arg => arg.Key, arg => arg.ToList());
        }
        
        private Dictionary<Guid, List<AlreadyLicensedApp>> AlreadyAppsInLicensesDictionary(List<AlreadyLicensedApp> alreadyLicensedApp)
        {
            return alreadyLicensedApp
                .Select(l => new AlreadyLicensedApp
                {
                    AppId = l.AppId,
                    LicensedTenantId = l.LicensedTenantId
                })
                .GroupBy(l => l.LicensedTenantId)
                .ToList()
                .ToDictionary(arg => arg.Key, arg => arg.ToList());
        }
        
        private Dictionary<Guid, List<BundledApp>> AlreadyAppsInBundleDictionary(List<BundledApp> alreadyBundledApp)
        {
            return alreadyBundledApp
                .Select(b => new BundledApp
                    {
                        AppId = b.AppId,
                        BundleId = b.BundleId
                    })
                .GroupBy(b => b.BundleId)
                .ToList()
                .ToDictionary(arg => arg.Key, arg => arg.ToList());
        }
        
        private async Task<List<LicensedBundlesWithUnLicensedAppsForBatchOperations>> GetLicensedBundlesWithUnLicensedApps(List<LicensedBundleApps> licensedBundleApps)
        {
            var licensedBundlesWithUnLicensedApps = new List<LicensedBundlesWithUnLicensedAppsForBatchOperations>();
            foreach (var appsByBundle in licensedBundleApps)
            {
                var licensedBundlesWithUnlicensedAppsByCurrentBundle = await _licenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations(appsByBundle.BundleId, appsByBundle.AppIds);
                licensedBundlesWithUnLicensedApps.AddRange(licensedBundlesWithUnlicensedAppsByCurrentBundle);
            }
            return licensedBundlesWithUnLicensedApps;
        }
    }
}