namespace Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models
{
    public class LicenseUsageAdditionalInformationOutput
    {
        public string LocalIpAddress { get; set; }
        
        public string SoftwareVersion { get; set; }
        
        public string HostName { get; set; }
        
        public string HostUser { get; set; }
        
        public string Language { get; set; }
        
        public string OsInfo { get; set; }
        
        public string BrowserInfo { get; set; }
        
        public string DatabaseName { get; set; }
    }
}