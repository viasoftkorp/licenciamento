using System;
using Viasoft.Licensing.LicenseServer.Domain.Enums;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models
{
    public class LicenseUsageInRealTimeDetailsOutput
    {
        public Guid TenantId { get; set; }
        
        public string User { get; set; }
        
        public string AppIdentifier { get; set; }
        public string AppName { get; set; }

        public string BundleIdentifier { get; set; }
        public string BundleName { get; set; }

        public bool AdditionalLicense { get; set; }

        public DateTime StartTime { get; set; }
        
        public int AppLicenses { get; set; }
        
        public int AppLicensesConsumed { get; set; }
        
        public int AdditionalLicenses { get; set; }
        
        public int AdditionalLicensesConsumed { get; set; }

        public string Cnpj { get; set; }
        
        public LicensingStatus LicensingStatus { get; set; }
        
        public LicensedAppStatus AppStatus { get; set; }

        public string StatusDescription => LicensingStatus.ToString();
        
        public string SoftwareName { get; set; } 
        public string SoftwareIdentifier { get; set; }
        public LicenseUsageAdditionalInformationOutput LicenseUsageAdditionalInformation { get; set; }
        public LicensingModels LicensingModel { get; set; }
        public LicensingModes? LicensingMode { get; set; }
    }
}