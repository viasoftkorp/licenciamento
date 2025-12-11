using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant
{
    public class LicensedTenantDetails
    {
        public LicensedTenantSettingsOutput LicensedTenantSettings { get; set; }
        public LicensedAccountDetails AccountDetails { get; set; }
        public List<OwnedBundleDetails> OwnedBundles { get; set; }
        public List<LicensedAppDetails> OwnedApps { get; set; }
        public List<NamedUserAppLicenseOutput> NamedUserAppLicenses { get; set; } = new();
        public List<NamedUserBundleLicenseOutput> NamedUserBundleLicenses { get; set; } = new();

        public LicensedTenantDetails()
        {
        }
    }
}