using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseCache
{
    public interface ILicenseCacheService
    {
        Task StoreLicenseByTenantId(LicenseByTenantIdOld licenseByTenantIdOld);
        Task<LicenseByTenantIdOld> GetLicenseByTenantId(Guid tenantId);
    }
}