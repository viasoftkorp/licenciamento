using System;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Classes
{
    public class AppLicenseConsumer
    {
        public string User { get; }
        public string AppName { get; }
        public string AppIdentifier { get; }
        public DateTime AccessDateTime { get; }
        public DateTime LastHeartbeatDateTime { get; private set; }
        public bool AdditionalLicense { get; }
        public int TimesUsedByUser { get; private set; }
        public string Cnpj { get; }
        
        public LicenseUsageAdditionalInformationOld LicenseUsageAdditionalInformation { get; }

        public AppLicenseConsumer(string appName, string appIdentifier, DateTime accessDateTime,
            DateTime lastHeartbeatDateTime, bool additionalLicense, ConsumeLicenseInput consumeLicenseInput)
        {
            AppName = appName;
            User = consumeLicenseInput.User;
            AppIdentifier = appIdentifier;
            AccessDateTime = accessDateTime;
            LastHeartbeatDateTime = lastHeartbeatDateTime;
            AdditionalLicense = additionalLicense;
            TimesUsedByUser = 1;
            Cnpj = consumeLicenseInput.Cnpj;
            LicenseUsageAdditionalInformation = consumeLicenseInput.LicenseUsageAdditionalInformation;
        }

        private AppLicenseConsumer(AppLicenseConsumer appLicenseConsumer)
        {
            User = appLicenseConsumer.User;
            AppIdentifier = appLicenseConsumer.AppIdentifier;
            AccessDateTime = appLicenseConsumer.AccessDateTime;
            LastHeartbeatDateTime = appLicenseConsumer.LastHeartbeatDateTime;
            AdditionalLicense = appLicenseConsumer.AdditionalLicense;
            TimesUsedByUser = appLicenseConsumer.TimesUsedByUser;
            Cnpj = appLicenseConsumer.Cnpj;
            AppName = appLicenseConsumer.AppName;
            LicenseUsageAdditionalInformation = appLicenseConsumer.LicenseUsageAdditionalInformation?.Clone();
        }

        public AppLicenseConsumer IncreaseTimesUsedByUser()
        {
            TimesUsedByUser++;
            return this;
        }
        
        public AppLicenseConsumer DecreaseTimesUsedByUser()
        {
            TimesUsedByUser--;
            return this;
        }

        public AppLicenseConsumer UpdateHeartbeat()
        {
            LastHeartbeatDateTime = DateTime.UtcNow;
            return this;
        }

        public AppLicenseConsumer Clone()
        {
            var consumedAppLicense = new AppLicenseConsumer(this);
            return consumedAppLicense;
        }
    }
}
