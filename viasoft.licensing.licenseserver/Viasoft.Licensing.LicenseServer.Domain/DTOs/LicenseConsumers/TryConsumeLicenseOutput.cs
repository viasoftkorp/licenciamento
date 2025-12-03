using System;
using Viasoft.Licensing.LicenseServer.Domain.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers
{
    public class TryConsumeLicenseOutput
    {
        public TryConsumeLicenseOutput(ConsumeAppLicenseStatus consumeAppLicenseStatus, string appName, string softwareIdentifier, string softwareName, 
            DateTime? licenseUsageStartTime = null)
        {
            ConsumeAppLicenseStatus = consumeAppLicenseStatus;
            LicenseUsageStartTime = licenseUsageStartTime;
            AppName = appName;
            SoftwareIdentifier = softwareIdentifier;
            SoftwareName = softwareName;
        }
        
        public ConsumeAppLicenseStatus ConsumeAppLicenseStatus { get; }
        public string AppName { get; }
        public DateTime? LicenseUsageStartTime { get; }
        public string SoftwareName { get; }
        public string SoftwareIdentifier { get; }
    }
}