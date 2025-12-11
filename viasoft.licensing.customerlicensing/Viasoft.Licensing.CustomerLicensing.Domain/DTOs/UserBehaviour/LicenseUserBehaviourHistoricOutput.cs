using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.UserBehaviour
{
    public class LicenseUserBehaviourHistoricOutput
    {
        public Guid TenantId { get; set; }

        public string Cnpj { get; set; }
        
        public string User { get; set; }
        
        public string AppIdentifier { get; set; }
        
        public string AppName { get; set; }

        public string BundleIdentifier { get; set; }
        
        public string BundleName { get; set; }
        
        public string SoftwareName { get; set; }
        
        public string SoftwareIdentifier { get; set; }
        
        public bool AdditionalLicense { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public int DurationInMinutes { get; set; }

    }
}