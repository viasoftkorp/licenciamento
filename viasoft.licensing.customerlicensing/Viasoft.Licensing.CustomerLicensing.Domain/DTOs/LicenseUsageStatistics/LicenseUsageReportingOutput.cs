using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageStatistics
{
    public class LicenseUsageReportingOutput
    {
        public Guid LicensingIdentifier {get; set;}
        
        public int UsageCount {get; set;}
    }
}