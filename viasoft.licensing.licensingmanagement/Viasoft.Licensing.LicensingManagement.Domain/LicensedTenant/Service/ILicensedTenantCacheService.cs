using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service
{
    public interface ILicensedTenantCacheService
    {
        Task InvalidateCacheForTenant(Guid identifier);
        Task InvalidateCacheForTenants(List<Guid> identifiers);

        Task<List<string>> GetOrCreateCacheForLicensedAppsIdentifiersFromTenant(Guid tenantId, Func<Task<List<string>>> func);
    }
}