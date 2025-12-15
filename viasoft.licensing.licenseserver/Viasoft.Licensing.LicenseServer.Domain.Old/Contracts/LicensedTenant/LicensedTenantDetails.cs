using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant
{
    public class LicensedTenantDetails
    {
        public LicensedAccountDetails AccountDetails { get; set; }
        public List<LicensedBundleDetails> OwnedBundles { get; set; }
        public List<LicensedAppDetails> OwnedApps { get; set; }

        public LicensedTenantDetails(List<LicensedBundleDetails> ownedBundles, List<LicensedAppDetails> ownedApps)
        {
            OwnedBundles = ownedBundles;
            OwnedApps = ownedApps;
        }

        public LicensedTenantDetails()
        {
        }
    }
}