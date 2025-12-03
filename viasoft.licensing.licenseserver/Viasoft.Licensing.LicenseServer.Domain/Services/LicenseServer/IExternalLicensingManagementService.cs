using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.TenantLicensing;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.HardwareId;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.LicenseServer
{
    public interface IExternalLicensingManagementService
    {
        Task<TenantLicenses> GetLicenseByTenantId(Guid tenantId);
        Task<UpdateHardwareIdOutput> UpdateHardwareId(UpdateHardwareIdInput input);
        Task<UpdateNamedUserAppLicenseOutput> UpdateNamedUserApp(UpdateNamedUserAppLicenseInput input, Guid hostTenantId, Guid licensedTenant, Guid licensedApp, Guid namedUserAppId);
        Task<UpdateNamedUserBundleLicenseOutput> UpdateNamedUserBundle(UpdateNamedUserBundleLicenseInput input, Guid hostTenantId, Guid licensedTenant, Guid licensedBundle, Guid namedUserBundleId);
    }
}