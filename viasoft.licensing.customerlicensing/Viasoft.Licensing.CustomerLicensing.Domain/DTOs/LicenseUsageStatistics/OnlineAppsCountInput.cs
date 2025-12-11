using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageStatistics
{
    public class OnlineAppsCountInput
    {
        public Guid? LicensingIdentifier { get; set; }
        
        public string AdvancedFilter { get; set; }
    }
}