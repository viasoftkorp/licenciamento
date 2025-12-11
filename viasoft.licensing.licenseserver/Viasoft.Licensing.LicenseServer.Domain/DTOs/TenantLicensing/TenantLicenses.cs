using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.TenantLicensing
{
    public class TenantLicenses
    {
        public LicensedTenantSettingsOutput LicensedTenantSettings { get; set; }
        public LicensedTenantOutput LicensedTenant { get; set; }
        public LicensedAccountDetails AccountDetails { get; set; }
        public List<OwnedBundleDetails> OwnedBundles { get; set; }
        public List<LicensedAppDetails> OwnedApps { get; set; }
        public List<NamedUserAppLicenseOutput> NamedUserAppLicenses { get; set; }
        public List<NamedUserBundleLicenseOutput> NamedUserBundleLicenses { get; set; }
    }
}