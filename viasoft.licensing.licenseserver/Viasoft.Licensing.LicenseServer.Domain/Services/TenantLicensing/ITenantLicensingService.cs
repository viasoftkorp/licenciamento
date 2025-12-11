using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.TenantLicensing;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.TenantLicensing
{
    public interface ITenantLicensingService
    {
        Task<TenantLicenses> GetTenantLicensing(Guid tenantId);
    }
}