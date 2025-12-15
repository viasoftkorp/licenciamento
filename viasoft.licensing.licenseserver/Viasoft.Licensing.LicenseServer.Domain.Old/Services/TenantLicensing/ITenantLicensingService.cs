using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.TenantLicensing
{
    public interface ITenantLicensingService
    {
        Task<LicenseByTenantIdOld> GetTenantLicensing(Guid tenantId);
        
        Task<TenantLicenseStatusRefreshInfo> GetLastRefreshInfo(Guid tenantId);
    }
}