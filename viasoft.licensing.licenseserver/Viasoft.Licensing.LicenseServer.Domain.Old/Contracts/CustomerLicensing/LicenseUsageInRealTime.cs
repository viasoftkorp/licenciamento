using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing
{
    public class LicenseUsageInRealTime
    {
        // ReSharper disable once EmptyConstructor
        // The comment above disables IDE warning.
        // This needs to exists because of mapper deserialization from LiteDB.
        public LicenseUsageInRealTime()
        {
        }
        
        public Guid TenantId { get; set; }
        
        public List<string> SoftwareUtilized { get; set; }

        public List<LicenseUsageInRealTimeDetails> LicenseUsageInRealTimeDetails { get; set; }
    }

}