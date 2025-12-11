using System;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageInRealTimeTreeTableData
{
    public class LicenseUsageInRealTimeTreeTableData
    {
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public LicenseUsageInRealTimeTreeTableTypes Type { get; set; }

        public string TypeDescription => Type.ToString();
        
        public int Total { get; set; }
        
        public int Additional { get; set; }
        
        public string Parent { get; set; }
        
        public int Consumed { get; set; }
    }
}