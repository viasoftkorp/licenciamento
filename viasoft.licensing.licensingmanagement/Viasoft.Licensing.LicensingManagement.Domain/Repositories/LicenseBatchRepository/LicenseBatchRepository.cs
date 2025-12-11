using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.App;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.LicenseBatchRepository
{
    public class LicenseBatchRepository: ILicenseBatchRepository, ITransientDependency
    {
        private readonly IBundleRepository _bundleRepository;

        private readonly IAppRepository _appRepository;
        
        private readonly IRepository<LicensedApp> _licensedApp;
        
        private readonly IRepository<Entities.LicensedTenant> _licensedTenant;
        

        public LicenseBatchRepository(IBundleRepository bundleRepository, IRepository<LicensedApp> licensedApp, IRepository<Entities.LicensedTenant> licensedTenant, IAppRepository appRepository)
        {
            _bundleRepository = bundleRepository;
            _licensedApp = licensedApp;
            _licensedTenant = licensedTenant;
            _appRepository = appRepository;
        }

        public async Task<List<LicensedBundlesWithUnLicensedAppsForBatchOperations>> GetLicensedBundlesWithUnLicensedAppsForBatchOperations(Guid bundleId, List<Guid> appIds)
        {
            var output = new List<LicensedBundlesWithUnLicensedAppsForBatchOperations>();
            var licensedBundles = await _bundleRepository.GetLicensedBundles(bundleId);
            var licenseTenantsWithCurrentBundle = licensedBundles.Select(licensed => licensed.LicensedTenantId).ToList();
            var appsAlreadyLicensed = await _licensedApp.Where(l => licenseTenantsWithCurrentBundle.Contains(l.LicensedTenantId) && appIds.Contains(l.AppId))
                .Select(l => new AlreadyLicensedApp{AppId = l.AppId, LicensedTenantId = l.LicensedTenantId})
                .ToListAsync();

            foreach (var licensedBundle in licensedBundles)
            {
                var appsLicensedForCurrentTenant  = appsAlreadyLicensed.Where(l => l.LicensedTenantId == licensedBundle.LicensedTenantId).Select(l => l.AppId).ToList();
                var appsToImportForCurrentTenant = await _appRepository.GetAppIdentifiersByAppIds(appIds.Except(appsLicensedForCurrentTenant).ToList());
                if (appsToImportForCurrentTenant.Count > 0)
                {
                    output.Add(new LicensedBundlesWithUnLicensedAppsForBatchOperations
                    {
                        BundleId = licensedBundle.BundleId,
                        LicensedTenantId = licensedBundle.LicensedTenantId,
                        AppsGetForBatchOperations = appsToImportForCurrentTenant,
                        Status = licensedBundle.Status,
                        NumberOfLicenses = licensedBundle.NumberOfLicenses
                    }); 
                }
            }
            return output;
        }
        
        public async Task<List<LicensedBundle>> GetLicensedBundlesWithAppsLicensed(Guid bundleId, Guid appId)
        {
            var licensedBundles = await _bundleRepository.GetLicensedBundles(bundleId);

            var licensedTenantIdFromBundleIdList = licensedBundles.Select(licensed => licensed.LicensedTenantId).ToList();

            var alreadyLicensedTenantIdsOfApps = await LicensedTenantIdOfAppsAlreadyLicensedInBundles(licensedTenantIdFromBundleIdList, appId, bundleId);

            var licensedBundlesWithAppsLicensed = licensedBundles.Where(licensed => alreadyLicensedTenantIdsOfApps.Contains(licensed.LicensedTenantId))
                .ToList();

            return licensedBundlesWithAppsLicensed;
        }

        public async Task<Dictionary<Guid, Guid>> GetLicensedTenantToIdentifierDictionary(List<Guid> licenseTenantIds)
        {
            return await _licensedTenant.Where(l => licenseTenantIds.Contains(l.Id))
                .Select(l => new
                {
                    l.Id, l.Identifier
                })
                .ToDictionaryAsync(arg => arg.Id, arg => arg.Identifier);
        }
        
        private async Task<List<Guid>> LicensedTenantIdOfAppsAlreadyLicensed(List<Guid> licensedTenantIds, Guid appId)
        {
            return await _licensedApp.Where(la => licensedTenantIds.Contains(la.LicensedTenantId) && la.AppId == appId).Select(la => la.LicensedTenantId).ToListAsync();
        }
        
        private async Task<List<Guid>> LicensedTenantIdOfAppsAlreadyLicensedInBundles(List<Guid> licensedTenantIds, Guid appId, Guid bundleId)
        {
            return await _licensedApp.Where(la => licensedTenantIds.Contains(la.LicensedTenantId) && la.AppId == appId && la.LicensedBundleId == bundleId).Select(la => la.LicensedTenantId).ToListAsync();
        }
        
    }
}