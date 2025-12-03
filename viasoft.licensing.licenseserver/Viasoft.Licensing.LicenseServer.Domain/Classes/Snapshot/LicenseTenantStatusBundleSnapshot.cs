using System.Collections.Generic;
using System.Linq;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;

namespace Viasoft.Licensing.LicenseServer.Domain.Classes.Snapshot
{
    public class LicenseTenantStatusBundleSnapshot
    {
        public string BundleIdentifier { get; }
        public string BundleName { get; }
        public int BundleLicenseCount { get; }
        public int BundleConsumedLicenseCount { get; }
        public Dictionary<string, LicenseTenantStatusAppSnapshot> OwnedApps { get; }
        
        public LicenseTenantStatusBundleSnapshot()
        {
            
        }

        public LicenseTenantStatusBundleSnapshot(LicenseTenantStatusUsedBundle bundle)
        {
            BundleIdentifier = bundle.BundleIdentifier;
            BundleName = bundle.BundleName;
            BundleLicenseCount = bundle.BundleLicenseCount;
            BundleConsumedLicenseCount = bundle.BundleConsumedLicenseCount;
            OwnedApps = bundle.OwnedApps.ToDictionary(pair => pair.Key, pair => new LicenseTenantStatusAppSnapshot(pair.Value));
        }
    }
}