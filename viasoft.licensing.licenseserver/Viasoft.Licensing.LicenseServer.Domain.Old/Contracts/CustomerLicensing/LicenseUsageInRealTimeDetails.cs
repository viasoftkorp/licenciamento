using System;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing
{
    public class LicenseUsageInRealTimeDetails
    {
        private DateTime _startTime;

        public LicenseUsageInRealTimeDetails()
        {
        }
        
        public LicenseUsageInRealTimeDetails(Guid tenantId, string user, string appIdentifier, string appName, string bundleIdentifier, string bundleName, bool additionalLicense,
            DateTime startTime, int appLicenses, int appLicensesConsumed, int additionalLicenses, int additionalLicensesConsumed, string cnpj, LicensingStatus licensingStatus,
            LicensedAppStatus appStatus, string softwareName, string softwareIdentifier, LicenseUsageAdditionalInformationOld licenseUsageAdditionalInformation)
        {
            TenantId = tenantId;
            User = user;
            AppIdentifier = appIdentifier;
            BundleIdentifier = bundleIdentifier;
            AdditionalLicense = additionalLicense;
            StartTime = startTime;
            AppLicenses = appLicenses;
            AppLicensesConsumed = appLicensesConsumed;
            AdditionalLicenses = additionalLicenses;
            AdditionalLicensesConsumed = additionalLicensesConsumed;
            Cnpj = cnpj;
            LicensingStatus = licensingStatus;
            AppStatus = appStatus;
            SoftwareName = softwareName;
            SoftwareIdentifier = softwareIdentifier;
            AppName = appName;
            BundleName = bundleName;
            LicenseUsageAdditionalInformation = licenseUsageAdditionalInformation;
        }
        
        public Guid TenantId { get; set; }
        
        public string User { get; set; }
        
        public string AppIdentifier { get; set; }
        public string AppName { get; set; }

        public string BundleIdentifier { get; set; }
        public string BundleName { get; set; }

        public bool AdditionalLicense { get; set; }

        public DateTime StartTime
        {
            get => _startTime;
            set => _startTime = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, 0, DateTimeKind.Utc);
        }

        public int AppLicenses { get; set; }
        
        public int AppLicensesConsumed { get; set; }
        
        public int AdditionalLicenses { get; set; }
        
        public int AdditionalLicensesConsumed { get; set; }

        public string Cnpj { get; set; }
        
        public LicensingStatus LicensingStatus { get; set; }
        
        public LicensedAppStatus AppStatus { get; set; }

        public string StatusDescription => LicensingStatus.ToString();
        
        public string SoftwareName { get; set; } 
        public string SoftwareIdentifier { get; set; }
        public LicenseUsageAdditionalInformationOld LicenseUsageAdditionalInformation { get; set; }
    }
}