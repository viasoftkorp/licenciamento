using System;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.UserBehaviour
{
    public class LicenseUsageBehaviourDetails
    {
        public Guid Id { get; set; }
        
        public Guid TenantId { get; set; }
        
        public string Cnpj { get; set; }
        
        public string User { get; set; }
        
        public string AppIdentifier { get; set; }
        
        public string AppName { get; set; }
        
        public string BundleIdentifier { get; set; }
        
        public string BundleName { get; set; }
        
        public bool AdditionalLicense { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public int DurationInSeconds { get; set; }
        
        public DateTime LogDateTime { get; set; }
        
        public string SoftwareName { get; set; }
        
        public string SoftwareIdentifier { get; set; }
        
        public string SoftwareVersion { get; set; }
        public string HostName { get; set; }
        public string HostUser { get; set; }
        public string LocalIpAddress { get; set; }
        public string Language { get; set; }
        public string OsInfo { get; set; }
        public string BrowserInfo { get; set; }
        public string DatabaseName { get; set; }
    }
}