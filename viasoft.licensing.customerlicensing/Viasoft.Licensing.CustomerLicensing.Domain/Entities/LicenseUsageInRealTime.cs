using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Viasoft.Core.DDD.Entities;
using Viasoft.Licensing.CustomerLicensing.Domain.Contracts.LicensingManagement;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Entities
{
    [Table("LicenseUsageInRealTime")]
    public class LicenseUsageInRealTime: Entity
    {
        public Guid LicensingIdentifier { get; set; }

        [Required]
        public string AppIdentifier { get; set; }
        [Required]
        public string AppName { get; set; }

        public string BundleIdentifier { get; set; }
        public string BundleName { get; set; }
        
        [Required]
        public string SoftwareName { get; set; }
        [Required]
        public string SoftwareIdentifier { get; set; }

        public int AppLicenses { get; set; }
        public int AppLicensesConsumed { get; set; }

        public int AdditionalLicenses { get; set; }
        public int AdditionalLicensesConsumed { get; set; }
        
        public bool AdditionalLicense { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public string User { get; set; }
        
        public string Cnpj { get; set; }
        
        public DateTime LastUpdate { get; set; }
        
        public LicensingStatus LicensingStatus { get; set; }
        
        public LicensedAppStatus AppStatus { get; set; }
        
        public string SoftwareVersion { get; set; }
        
        public string HostName { get; set; }
        
        public string HostUser { get; set; }
        
        public string LocalIpAddress { get; set; }
        
        public string Language { get; set; }
        
        public string OsInfo { get; set; }
        
        public string BrowserInfo { get; set; }
        
        public string DatabaseName { get; set; }
        
        public Guid AccountId { get; set; }
        
        public string AccountName { get; set; }
        
        public Domains Domain { get; set; }
        
        [Required]
        public LicensingModels LicensingModel { get; set; }
        
        public LicensingModes? LicensingMode { get; set; }

        [NotMapped]
        public TimeSpan AccessDuration => DateTime.UtcNow - StartTime;
    }
}