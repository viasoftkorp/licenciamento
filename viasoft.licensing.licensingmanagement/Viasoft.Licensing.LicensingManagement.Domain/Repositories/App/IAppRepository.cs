using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.App
{
    public interface IAppRepository
    {
        Task<bool> IsSoftwareBeingUsedByApps(Guid appId);

        Task<List<Entities.App>> GetAllDefaultApps();

        Task<List<LicensedApp>> GetLicensedAppsForLicenses(List<Guid> licensedTenantsId, Guid appId);

        Task<List<AlreadyLicensedApp>> GetAppsAlreadyLicensed(List<Guid> appId, List<Guid> licensedTenantIds);
        
        Task<List<AppsGetForBatchOperations>> GetAppsByIdsForBatchOperations(List<Guid> excludedApps, List<Guid> includedApps);

        Task<List<AppsGetForBatchOperations>> GetAllAppsForBatchOperation(string advancedFilter);

        Task<List<AppsGetForBatchOperations>> GetAppIdentifiersByAppIds(List<Guid> appIds);

        Task<Dictionary<string, Enums.Domain>> GetDomainsByAppIdentifiers(List<string> appIds);
    }
}    