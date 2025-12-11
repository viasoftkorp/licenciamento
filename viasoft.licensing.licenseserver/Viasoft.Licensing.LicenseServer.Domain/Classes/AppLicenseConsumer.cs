using System;
using Viasoft.Licensing.LicenseServer.Domain.Abstractions.NamedUserLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.Domain.Classes
{
    public class AppLicenseConsumer
    {
        public string User { get; }
        public string AppName { get; }
        public string AppIdentifier { get; }
        public DateTime AccessDateTime { get; }
        public DateTime LastHeartbeatDateTime { get; private set; }
        public int TimesUsedByUser { get; private set; }
        public string Cnpj { get; }
        public INamedUserLicense NamedUserLicense { get; }
        public LicenseUsageAdditionalInformation LicenseUsageAdditionalInformation { get; }

        public AppLicenseConsumer(string appName, string appIdentifier, DateTime accessDateTime, DateTime lastHeartbeatDateTime, ConsumeLicenseInput consumeLicenseInput, 
            INamedUserLicense namedUserLicense)
        {
            AppName = appName;
            User = consumeLicenseInput.User;
            AppIdentifier = appIdentifier;
            AccessDateTime = accessDateTime;
            LastHeartbeatDateTime = lastHeartbeatDateTime;
            TimesUsedByUser = 1;
            Cnpj = consumeLicenseInput.Cnpj;
            LicenseUsageAdditionalInformation = consumeLicenseInput.LicenseUsageAdditionalInformation;
            NamedUserLicense = namedUserLicense;
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
    }
}
