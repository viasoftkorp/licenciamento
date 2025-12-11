using System.Collections.Generic;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Contracts.LicensingManagement
{
    public class LicensedTenantDetails
    {
        public List<LicensedBundleDetails> OwnedBundles { get; set; }
        public List<LicensedAppDetails> OwnedApps { get; set; }
    }
}