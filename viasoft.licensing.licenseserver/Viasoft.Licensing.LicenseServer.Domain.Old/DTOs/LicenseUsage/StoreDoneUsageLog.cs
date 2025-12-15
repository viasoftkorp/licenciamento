using System;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage
{
    public class StoreDoneUsageLog
    {
        public StoreDoneUsageLog(Guid tenantId, string user, string cnpj, string appIdentifier, string appName, 
            string bundleIdentifier, string bundleName, DateTime startTime, DateTime endTime, bool additionalLicense, 
            string softwareIdentifier, string softwareName, LicenseUsageAdditionalInformationOld licenseUsageAdditionalInformationOld)
        {
            TenantId = tenantId;
            Cnpj = cnpj;
            User = user;
            AppIdentifier = appIdentifier;
            BundleIdentifier = bundleIdentifier;
            StartTime = startTime;
            EndTime = endTime;
            AdditionalLicense = additionalLicense;
            SoftwareIdentifier = softwareIdentifier;
            SoftwareName = softwareName;
            AppName = appName;
            BundleName = bundleName;
            if (licenseUsageAdditionalInformationOld == null)
                return;
            SoftwareVersion = licenseUsageAdditionalInformationOld.SoftwareVersion;
            HostName = licenseUsageAdditionalInformationOld.HostName;
            HostUser = licenseUsageAdditionalInformationOld.HostUser;
            LocalIpAddress = licenseUsageAdditionalInformationOld.LocalIpAddress;
            Language = licenseUsageAdditionalInformationOld.Language;
            OsInfo = licenseUsageAdditionalInformationOld.OsInfo;
            BrowserInfo = licenseUsageAdditionalInformationOld.BrowserInfo;
            DatabaseName = licenseUsageAdditionalInformationOld.DatabaseName;
        }
        
        public Guid TenantId { get; }
        public string Cnpj { get; }
        public string User { get; }
        public bool AdditionalLicense { get; }
        public string AppIdentifier { get; }
        public string AppName { get; }
        public string BundleIdentifier { get; }
        public string BundleName { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public string SoftwareIdentifier { get; }
        public string SoftwareName { get; }
        public string SoftwareVersion { get; }
        public string HostName { get; }
        public string HostUser { get; }
        public string LocalIpAddress { get; }
        public string Language { get; }
        public string OsInfo { get; }
        public string BrowserInfo { get; }
        public string DatabaseName { get; }
    }
}