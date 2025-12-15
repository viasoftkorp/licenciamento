namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage
{
    public class LicenseUsageAdditionalInformationOld
    {

        public LicenseUsageAdditionalInformationOld()
        {
        }
        
        private LicenseUsageAdditionalInformationOld(LicenseUsageAdditionalInformationOld licenseUsageAdditionalInformationOld)
        {
            SoftwareVersion = licenseUsageAdditionalInformationOld.SoftwareVersion;
            HostName = licenseUsageAdditionalInformationOld.HostName;
            HostUser = licenseUsageAdditionalInformationOld.HostUser;
            LocalIpAddress = licenseUsageAdditionalInformationOld.LocalIpAddress;
            Language = licenseUsageAdditionalInformationOld.Language;
            OsInfo = licenseUsageAdditionalInformationOld.OsInfo;
            BrowserInfo = licenseUsageAdditionalInformationOld.BrowserInfo;
            DatabaseName = licenseUsageAdditionalInformationOld.DatabaseName;
        }

        public string SoftwareVersion { get; set; }
        
        public string HostName { get; set; }
        
        public string HostUser { get; set; }
        
        public string LocalIpAddress { get; set; }
        
        public string Language { get; set; }
        
        public string OsInfo { get; set; }
        
        public string BrowserInfo { get; set; }
        
        public string DatabaseName { get; set; }

        public LicenseUsageAdditionalInformationOld Clone()
        {
            return new LicenseUsageAdditionalInformationOld(this);    
        }
    }
}