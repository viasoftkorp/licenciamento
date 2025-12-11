using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle
{
    public interface IBundleRepository
    {
        Task<Dictionary<Guid, string>> GetBundleNamesFromIdList(List<Guid> id);
        
        Task<bool> IsSoftwareBeingUsedByBundles(Guid softwareId);

        Task<bool> IsAppBeingUsedByBundles(Guid appId);
        
        Task<bool> CheckIfBundleIsLicensedInAnyLicensing(Guid bundleId);
        
        Task<List<LicensedBundle>> GetLicensedBundles(Guid bundleId);

        Task<List<BundlesGetForBatchOperations>> GetAllBundlesForBatchOperation(string advancedFilter);

        Task<List<BundlesGetForBatchOperations>> GetBundlesByIdsForBatchOperations(List<Guid> excludedBundles, List<Guid> includedBundles);

        Task<List<Entities.BundledApp>> GetBundledApps(List<Guid> appIds, List<Guid> bundleIds);

        Task<List<Guid>> GetBundlesIdsAlreadyLicensed(List<Guid> bundleIds);

        Task<List<LicensedBundleGetForBatchOperation>> GetBundlesAlreadyInLicenses(List<Guid> bundlesId, List<Guid> licenseTenantIds);
    }
}