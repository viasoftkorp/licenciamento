using System;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers
{
    public class ReleaseConsumedLicenseOutput
    {
        public DateTime LicenseUsageStartTime { get; }
        public DateTime LicenseUsageEndTime { get; }
        public string AppName { get; }
        public string Cnpj { get; }
        public LicenseUsageAdditionalInformationOld LicenseUsageAdditionalInformation { get; }

        public ReleaseConsumedLicenseOutput(DateTime licenseUsageEndTime, AppLicenseConsumer appLicenseConsumer)
        {
            LicenseUsageEndTime = licenseUsageEndTime;
            LicenseUsageStartTime = appLicenseConsumer.AccessDateTime;
            Cnpj = appLicenseConsumer.Cnpj;
            AppName = appLicenseConsumer.AppName;
            LicenseUsageAdditionalInformation = appLicenseConsumer.LicenseUsageAdditionalInformation;
        }
    }
}