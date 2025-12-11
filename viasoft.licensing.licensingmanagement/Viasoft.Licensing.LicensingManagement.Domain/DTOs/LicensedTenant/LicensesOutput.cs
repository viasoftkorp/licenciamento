using System.Collections.Generic;
using Viasoft.Licensing.LicensingManagement.Domain.Account.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.OwnedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.OwnedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant
{
    public class LicensesOutput
    {
        public LicensedTenantSettingsOutput LicensedTenantSettings { get; set; }
        public LicensedTenantOutput LicensedTenant { get; set; }
        public AccountOutput AccountDetails { get; set; }
        public List<OwnedBundleOutput> OwnedBundles { get; set; }
        public List<OwnedAppOutput> OwnedApps { get; set; }
        public List<NamedUserAppLicenseOutput> NamedUserAppLicenses { get; set; }
        public List<NamedUserBundleLicenseOutput> NamedUserBundleLicenses { get; set; }
    }
}