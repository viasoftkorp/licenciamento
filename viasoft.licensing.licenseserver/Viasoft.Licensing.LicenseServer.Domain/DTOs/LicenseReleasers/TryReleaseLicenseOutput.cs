using System;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers
{
    public class TryReleaseLicenseOutput
    {
        public TryReleaseLicenseOutput(ReleaseAppLicenseStatus releaseAppLicenseStatus, string appName, 
            string softwareIdentifier, string softwareName, string cnpj = null, 
            DateTime? licenseUsageStartTime = null, DateTime? licenseUsageEndTime = null,
            LicenseUsageAdditionalInformation licenseUsageAdditionalInformation = null)
        {
            ReleaseAppLicenseStatus = releaseAppLicenseStatus;
            Cnpj = cnpj;
            LicenseUsageStartTime = licenseUsageStartTime;
            LicenseUsageEndTime = licenseUsageEndTime;
            AppName = appName;
            SoftwareIdentifier = softwareIdentifier;
            SoftwareName = softwareName;
            LicenseUsageAdditionalInformation = licenseUsageAdditionalInformation;
        }
        
        public ReleaseAppLicenseStatus ReleaseAppLicenseStatus { get; }
        public string Cnpj { get; }
        public string AppName { get; }
        public string SoftwareName { get; }
        public string SoftwareIdentifier { get; }
        public DateTime? LicenseUsageStartTime { get; }
        public DateTime? LicenseUsageEndTime { get; }
        public LicenseUsageAdditionalInformation LicenseUsageAdditionalInformation { get; }
    }
}