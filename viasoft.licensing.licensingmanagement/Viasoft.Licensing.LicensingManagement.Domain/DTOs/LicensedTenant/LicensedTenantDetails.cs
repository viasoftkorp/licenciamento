using System;
using System.Collections.Generic;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Account;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSettings;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant
{
    public class LicensedTenantDetails
    {
        public Guid LicenseIdentifier { get; set; }
        public AccountDetails AccountDetails { get; set; }
        public List<LicensedBundleDetails> OwnedBundles { get; }
        public List<LicensedAppDetails> OwnedApps { get; }
        public LicensedTenantSettingsOutput LicensedTenantSettings { get; }

        public LicensedTenantDetails(List<LicensedBundleDetails> ownedBundles, List<LicensedAppDetails> ownedApps, LicensedTenantSettingsOutput settings)
        {
            OwnedBundles = ownedBundles;
            OwnedApps = ownedApps;
            LicensedTenantSettings = settings;
        }
    }
}