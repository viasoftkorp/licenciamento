using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.BatchOperationServices
{
    public interface ILicensedTenantBatchOperationsService
    {
        Task RemoveAppFromBundleInLicenses(Guid bundleId, Guid appId);

        Task InsertBundlesInLicenses(BatchOperationsInput input);
        
        Task InsertAppsInLicenses(BatchOperationsInput input);
        
        Task<List<LicensedBundleApp>> InsertAppsInBundles(BatchOperationsInput input);

        Task InsertAppsFromBundlesInLicenses(List<LicensedBundleApp> licensedBundleApps);
    }
}