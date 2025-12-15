using System;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers
{
    public class TryConsumeLicenseOutput
    {
        public TryConsumeLicenseOutput(ConsumeAppLicenseStatusOld consumeAppLicenseStatus, string appName, string softwareIdentifier, string softwareName, 
            DateTime? licenseUsageStartTime = null)
        {
            ConsumeAppLicenseStatus = consumeAppLicenseStatus;
            LicenseUsageStartTime = licenseUsageStartTime;
            AppName = appName;
            SoftwareIdentifier = softwareIdentifier;
            SoftwareName = softwareName;
        }
        
        public ConsumeAppLicenseStatusOld ConsumeAppLicenseStatus { get; }
        public string AppName { get; }
        public DateTime? LicenseUsageStartTime { get; }
        public string SoftwareName { get; }
        public string SoftwareIdentifier { get; }
    }
}