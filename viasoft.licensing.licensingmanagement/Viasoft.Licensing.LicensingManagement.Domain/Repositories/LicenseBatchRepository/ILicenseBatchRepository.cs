using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.LicenseBatchRepository
{
    public interface ILicenseBatchRepository
    {
        Task<List<LicensedBundlesWithUnLicensedAppsForBatchOperations>> GetLicensedBundlesWithUnLicensedAppsForBatchOperations(Guid bundleId, List<Guid> appIds);
        Task<List<LicensedBundle>> GetLicensedBundlesWithAppsLicensed(Guid bundleId, Guid appId);
        Task<Dictionary<Guid, Guid>> GetLicensedTenantToIdentifierDictionary(List<Guid> licenseTenantIds);
    }
}