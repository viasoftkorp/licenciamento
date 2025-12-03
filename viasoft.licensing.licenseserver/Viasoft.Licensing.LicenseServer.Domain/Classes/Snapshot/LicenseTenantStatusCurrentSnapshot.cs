using System.Collections.Generic;
using System.Linq;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;

namespace Viasoft.Licensing.LicenseServer.Domain.Classes.Snapshot
{
    public class LicenseTenantStatusCurrentSnapshot
    {
        public Dictionary<string, LicenseTenantStatusBundleSnapshot> Bundles { get; }
        public Dictionary<string, LicenseTenantStatusAppSnapshot> LooseApps { get; } 
        
        public LicenseTenantStatusCurrentSnapshot()
        {
        }

        public LicenseTenantStatusCurrentSnapshot(LicenseTenantStatusCurrent currentStatus)
        {
            Bundles = currentStatus.Bundles.ToDictionary(pair => pair.Key, pair => new LicenseTenantStatusBundleSnapshot(pair.Value));
            LooseApps = currentStatus.LooseApps.ToDictionary(pair => pair.Key, pair => new LicenseTenantStatusAppSnapshot(pair.Value));
        }
    }
}