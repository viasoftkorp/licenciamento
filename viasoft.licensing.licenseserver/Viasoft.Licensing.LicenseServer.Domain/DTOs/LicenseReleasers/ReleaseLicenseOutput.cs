using System;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers
{
    public class ReleaseLicenseOutput
    {
        public ReleaseAppLicenseStatus ReleaseAppLicenseStatus { get; }
        public string ReleaseAppLicenseStatusDescription => ReleaseAppLicenseStatus.ToString();
        public string BundleIdentifier { get; }
        public string BundleName { get; }
        public string AppIdentifier { get; }
        public string AppName { get; }
        public DateTime? LicenseUsageStartTime { get; }
        public DateTime? LicenseUsageEndTime { get; }
        public string Cnpj { get; }
        public string SoftwareName { get; }
        public string SoftwareIdentifier { get; }
        public LicenseUsageAdditionalInformation LicenseUsageAdditionalInformation { get; }

        public ReleaseLicenseOutput(ReleaseAppLicenseStatus releaseAppLicenseStatus, string appIdentifier, string cnpj, 
            string appName = null, string softwareIdentifier = null, string softwareName = null, 
            DateTime? licenseUsageStartTime = null, DateTime? licenseUsageEndTime = null, 
            LicenseUsageAdditionalInformation licenseUsageAdditionalInformation = null, string bundleIdentifier = null, string bundleName = null)
        {
            ReleaseAppLicenseStatus = releaseAppLicenseStatus;
            LicenseUsageStartTime = licenseUsageStartTime;
            LicenseUsageEndTime = licenseUsageEndTime;
            BundleIdentifier = bundleIdentifier;
            AppIdentifier = appIdentifier;
            Cnpj = cnpj;
            SoftwareIdentifier = softwareIdentifier;
            SoftwareName = softwareName;
            AppName = appName;
            BundleName = bundleName;
            LicenseUsageAdditionalInformation = licenseUsageAdditionalInformation;
        }
    }
}