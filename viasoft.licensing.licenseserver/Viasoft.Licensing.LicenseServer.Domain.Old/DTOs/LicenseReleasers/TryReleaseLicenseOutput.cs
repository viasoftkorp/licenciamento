using System;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers
{
    public class TryReleaseLicenseOutput
    {
        public TryReleaseLicenseOutput(ReleaseAppLicenseStatusOld releaseAppLicenseStatus, string appName, 
            string softwareIdentifier, string softwareName, string cnpj = null, 
            DateTime? licenseUsageStartTime = null, DateTime? licenseUsageEndTime = null,
            LicenseUsageAdditionalInformationOld licenseUsageAdditionalInformation = null)
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
        
        public ReleaseAppLicenseStatusOld ReleaseAppLicenseStatus { get; }
        public string Cnpj { get; }
        public string AppName { get; }
        public string SoftwareName { get; }
        public string SoftwareIdentifier { get; }
        public DateTime? LicenseUsageStartTime { get; }
        public DateTime? LicenseUsageEndTime { get; }
        public LicenseUsageAdditionalInformationOld LicenseUsageAdditionalInformation { get; }
    }
}