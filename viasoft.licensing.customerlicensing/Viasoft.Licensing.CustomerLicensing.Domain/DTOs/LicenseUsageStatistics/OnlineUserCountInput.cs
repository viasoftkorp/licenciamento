using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageStatistics
{
    public class OnlineUserCountInput
    {
        public Guid? LicensingIdentifier { get; set; }
        
        public string AdvancedFilter { get; set; }
    }
}