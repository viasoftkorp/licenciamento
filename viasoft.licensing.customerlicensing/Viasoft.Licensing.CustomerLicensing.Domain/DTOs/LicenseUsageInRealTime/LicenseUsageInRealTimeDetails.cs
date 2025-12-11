using System;
using Viasoft.Data.Attributes;
using Viasoft.Licensing.CustomerLicensing.Domain.Contracts.LicensingManagement;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageInRealTime
{
    public class LicenseUsageInRealTimeDetails
    {
        [StrictRequired]
        public Guid TenantId { get; set; }

        [StrictRequired(AllowEmptyStrings = false)]
        public string AppIdentifier { get; set; }
        [StrictRequired(AllowEmptyStrings = false)]
        public string AppName { get; set; }

        public string BundleIdentifier { get; set; }
        public string BundleName { get; set; }
        
        [StrictRequired(AllowEmptyStrings = false)]
        public string SoftwareName { get; set; }
        [StrictRequired(AllowEmptyStrings = false)]
        public string SoftwareIdentifier { get; set; }
        
        
        public int AppLicenses { get; set; }
        
        public int AppLicensesConsumed { get; set; }
        
        public int AdditionalLicenses { get; set; }
        
        public int AdditionalLicensesConsumed { get; set; }
        
        public bool AdditionalLicense { get; set; }
        
        [StrictRequired]
        public DateTime StartTime  { get; set; }
        
        [StrictRequired(AllowEmptyStrings = false)]
        public string User { get; set; }
        
        [StrictRequired(AllowEmptyStrings = false)]
        public string Cnpj { get; set; }
        
        public LicensingStatus LicensingStatus { get; set; }
        
        public LicensedAppStatus AppStatus { get; set; }
        
        public LicenseUsageAdditionalInformation LicenseUsageAdditionalInformation { get; set; }
        
        public LicensingModels LicensingModel { get; set; }
        
        public LicensingModes? LicensingMode { get; set; }
    }
}