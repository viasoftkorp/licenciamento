using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageInRealTime
{
    public class LicenseUsageInRealTimeImportInput
    {
        public Guid TenantId { get; set; }
        
        public List<string> SoftwareUtilized { get; set; }

        public List<LicenseUsageInRealTimeDetails> LicenseUsageInRealTimeDetails { get; set; }
    }
}