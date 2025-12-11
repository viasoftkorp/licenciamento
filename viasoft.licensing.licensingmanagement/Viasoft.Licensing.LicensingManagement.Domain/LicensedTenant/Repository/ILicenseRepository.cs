using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository
{
    public interface ILicenseRepository
    {
        Task<List<string>> GetLicensedAppsIdentifiersFromTenant(Guid tenantId);

        Task<QuotaAppDetails> GetQuotaAppDetailsByAppIdentifier(Guid appId, Guid licensedTenantId);
        
        Task<bool> IsAppBeingUsedByLicense(Guid appId);
        
        Task<bool> IsBundleBeingUsedByLicense(Guid bundleId);

        Task<LicensedTenantDetails> GetLicenseDetailsByIdentifier(Entities.LicensedTenant licensedTenantIdentifier);
        
        Task<List<LicensedTenantDetails>> GetLicenseDetailsByIdentifiers(List<Guid> licencedTenantIdentifiers);

        Task<bool> CheckLicenseTenantIdExistence(Guid tenantId);
        
        Task<Guid> GetIdentifierFromLicenseTenantIdExistence(Guid licensedTenantId);

        Task<List<LicensedApp>> GetAllLicensedAppsFromTenantId(Guid id);

        Task<List<LicensedBundle>> GetAllLicensedBundlesFromTenantId(Guid id);

        Task<bool> CheckIfLicensedAppIsDefault(Guid licensedAppId);

        Task<bool> CheckIfLicensedTenantExists(Guid id);

        bool IsAccountBeingUsedByLicense(Guid accountId);

        Task<List<Guid>> GetAllLicensesForBatchOperations(string advancedFilter);

        Task<List<Guid>> GetLicensesByIdsForBatchOperations(List<Guid> excludedLicenses, List<Guid> includedLicenses);
    }
}