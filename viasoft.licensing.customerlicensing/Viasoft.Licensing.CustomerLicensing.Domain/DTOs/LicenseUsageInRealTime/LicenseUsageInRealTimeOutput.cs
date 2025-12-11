using System;
using Viasoft.Licensing.CustomerLicensing.Domain.Contracts.LicensingManagement;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageInRealTime
{
    public class LicenseUsageInRealTimeOutput
    {
        public Guid TenantId { get; set; }
        
        public string AppIdentifier { get; set; }
        
        public string AppName { get; set; }

        public string BundleIdentifier { get; set; }
        
        public string BundleName { get; set; }
        
        public int AppLicenses { get; set; }
        
        public int AppLicensesConsumed { get; set; }

        public int AdditionalLicenses { get; set; }
        
        public int AdditionalLicensesConsumed { get; set; }
        
        public bool AdditionalLicense { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public string User { get; set; }
        
        public string Cnpj { get; set; }
        
        public LicensedAppStatus Status { get; set; }
        
        public string StatusDescription => Status.ToString();
    }
}