using System;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers
{
    public class ConsumeLicenseOutputOld
    {
        public ConsumeAppLicenseStatusOld ConsumeAppLicenseStatus { get; }

        public string ConsumeAppLicenseStatusDescription => ConsumeAppLicenseStatus.ToString();
        
        public DateTime? LicenseUsageStartTime { get; }
        
        public string BundleIdentifier { get; }
        
        public string BundleName { get; }
        
        public string AppIdentifier { get; }
        
        public string AppName { get; }
        
        public string SoftwareIdentifier { get; }
        
        public string SoftwareName { get; }

        public ConsumeLicenseOutputOld(ConsumeAppLicenseStatusOld consumeAppLicenseStatus, string appIdentifier, 
            string appName = null, string softwareIdentifier = null, string softwareName = null, DateTime? licenseUsageStartTime = null, 
            string bundleIdentifier = null, string bundleName = null)
        {
            ConsumeAppLicenseStatus = consumeAppLicenseStatus;
            LicenseUsageStartTime = licenseUsageStartTime;
            BundleIdentifier = bundleIdentifier;
            AppIdentifier = appIdentifier;
            SoftwareIdentifier = softwareIdentifier;
            SoftwareName = softwareName;
            BundleName = bundleName;
            AppName = appName;
        }
        
    }
}