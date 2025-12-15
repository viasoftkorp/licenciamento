using System;
using System.Collections.Generic;
using System.Linq;
using Viasoft.Licensing.LicenseServer.Domain.Old.Extensions;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus
{
    public class LicenseTenantStatusApp
    {
        public string AppIdentifier { get; }
        public string AppName { get; }
        public int AppLicenses { get; }
        public int AppLicensesConsumed { get; private set; }
        public int AdditionalLicenses { get; }
        public int AdditionalLicensesConsumed { get; private set; }
        public LicensedAppStatus Status { get; }
        public string StatusDescription => Status.ToString();
        public readonly List<AppLicenseConsumer> AppLicenseConsumers;
        
        public string SoftwareIdentifier { get; set; }
        public string SoftwareName { get; set; }
        
        public LicenseTenantStatusApp(int licenseCount, int additionalNumberOfLicenses, string appIdentifier, string appName, LicensedAppStatus status, string softwareName, string softwareIdentifier)
        {
            AppLicenseConsumers = new List<AppLicenseConsumer>();
            AppLicenses = licenseCount >= 0 ? licenseCount : 0;
            AdditionalLicenses = additionalNumberOfLicenses >= 0 ? additionalNumberOfLicenses : 0;
            AppIdentifier = appIdentifier;
            AppName = appName;
            Status = status;
            AdditionalLicensesConsumed = 0;
            AppLicensesConsumed = 0;
            SoftwareIdentifier = softwareIdentifier;
            SoftwareName = softwareName;
        }

        private LicenseTenantStatusApp(LicenseTenantStatusApp licenseTenantStatusApp, List<AppLicenseConsumer> appLicenseConsumers)
        {
            AppLicenses = licenseTenantStatusApp.AppLicenses;
            AdditionalLicenses = licenseTenantStatusApp.AdditionalLicenses;
            AppIdentifier = licenseTenantStatusApp.AppIdentifier;
            AppName = licenseTenantStatusApp.AppName;
            Status = licenseTenantStatusApp.Status; 
            AppLicenseConsumers = appLicenseConsumers;
            AdditionalLicensesConsumed = licenseTenantStatusApp.AdditionalLicensesConsumed;
            AppLicensesConsumed = licenseTenantStatusApp.AppLicensesConsumed;
            SoftwareIdentifier = licenseTenantStatusApp.SoftwareIdentifier;
            SoftwareName = licenseTenantStatusApp.SoftwareName;
        }
        
        private AppLicenseConsumer GetUserConsumedLicense(string user) => AppLicenseConsumers.First(license => license.User.EqualsIgnoringCase(user) && license.AppIdentifier.EqualsIgnoringCase(AppIdentifier));
        
        public bool UserAlreadyConsumingLicense(string user) => AppLicenseConsumers.Any(license => license.User.EqualsIgnoringCase(user) && license.AppIdentifier.EqualsIgnoringCase(AppIdentifier) && !license.AdditionalLicense);
        
        public bool UserAlreadyConsumingAdditionalLicense(string user) => AppLicenseConsumers.Any(license => license.User.EqualsIgnoringCase(user) && license.AppIdentifier.EqualsIgnoringCase(AppIdentifier) && license.AdditionalLicense);
        
        private bool AppIsBlocked() => Status == LicensedAppStatus.AppBlocked;
        
        public bool HasAdditionalConsumedLicense(string user) => AppLicenseConsumers.Any(license => license.User.EqualsIgnoringCase(user) && license.AppIdentifier.EqualsIgnoringCase(AppIdentifier) && license.AdditionalLicense);
        
        private bool HasAdditionalAvailableLicense() => AdditionalLicenses - AdditionalLicensesConsumed > 0;
        
        private bool HasConsumedLicense(string user) => AppLicenseConsumers.Any(license => license.User.EqualsIgnoringCase(user) && license.AppIdentifier.EqualsIgnoringCase(AppIdentifier) && !license.AdditionalLicense);
        
        private bool HasAvailableLicense() => AppLicenses - AppLicensesConsumed > 0;

        public TryConsumeLicenseOutput TryConsumeLicense(ConsumeLicenseInput input, bool isBundled, LicenseConsumeType licenseConsumeType)
        {
            var appName = !string.IsNullOrEmpty(input.CustomAppName) ? input.CustomAppName : AppName;
            
            if (AppIsBlocked())
                return new TryConsumeLicenseOutput(ConsumeAppLicenseStatusOld.AppBlocked, appName, SoftwareIdentifier, SoftwareName);

            if (UserAlreadyConsumingLicense(input.User) && licenseConsumeType == LicenseConsumeType.Connection)
            {
                var repeatedLicenseConsumer = GetUserConsumedLicense(input.User)
                    .IncreaseTimesUsedByUser()
                    .UpdateHeartbeat();
                return new TryConsumeLicenseOutput(ConsumeAppLicenseStatusOld.LicenseAlreadyInUseByUser, appName, SoftwareIdentifier, SoftwareName, repeatedLicenseConsumer.AccessDateTime);
            }
            
            if (!isBundled && !HasAvailableLicense())
                return new TryConsumeLicenseOutput(ConsumeAppLicenseStatusOld.NotEnoughLicenses, appName, SoftwareIdentifier, SoftwareName);
            
            var newLicenseConsumer = ConsumeAvailableLicense(appName, input, isBundled);
            return new TryConsumeLicenseOutput(ConsumeAppLicenseStatusOld.LicenseConsumed, appName, SoftwareIdentifier, SoftwareName, newLicenseConsumer.AccessDateTime);
        }

        public TryConsumeLicenseOutput TryConsumeAdditionalLicense(ConsumeLicenseInput input, LicenseConsumeType licenseConsumeType)
        {
            var appName = !string.IsNullOrEmpty(input.CustomAppName) ? input.CustomAppName : AppName;
            
            if (AppIsBlocked())
                return new TryConsumeLicenseOutput(ConsumeAppLicenseStatusOld.AppBlocked, appName, SoftwareIdentifier, SoftwareName);

            if (UserAlreadyConsumingAdditionalLicense(input.User) && licenseConsumeType == LicenseConsumeType.Connection)
            {
                var repeatedLicenseConsumer = GetUserConsumedLicense(input.User);
                if (!repeatedLicenseConsumer.AdditionalLicense) 
                    return new TryConsumeLicenseOutput(ConsumeAppLicenseStatusOld.NotEnoughLicenses, appName, SoftwareIdentifier, SoftwareName);
                
                repeatedLicenseConsumer
                    .IncreaseTimesUsedByUser()
                    .UpdateHeartbeat();
                return new TryConsumeLicenseOutput(ConsumeAppLicenseStatusOld.AdditionalLicenseAlreadyInUseByUser, appName, SoftwareIdentifier, SoftwareName, repeatedLicenseConsumer.AccessDateTime);
            }
            
            if (!HasAdditionalAvailableLicense())
                return new TryConsumeLicenseOutput(ConsumeAppLicenseStatusOld.NotEnoughLicenses, appName, SoftwareIdentifier, SoftwareName);
            
            var newLicenseConsumer = ConsumeAdditionalAvailableLicense(appName, input);
            return new TryConsumeLicenseOutput(ConsumeAppLicenseStatusOld.AdditionalLicenseConsumed, appName, SoftwareIdentifier, SoftwareName, newLicenseConsumer.AccessDateTime);
        }

        private AppLicenseConsumer ConsumeAvailableLicense(string appName, ConsumeLicenseInput consumeLicenseInput, bool isBundled)
        {
            var access = DateTime.UtcNow;
            var consumedLicense = new AppLicenseConsumer(appName, AppIdentifier, access, access, false, consumeLicenseInput);
            AppLicenseConsumers.Add(consumedLicense);
            if (!isBundled)
                AppLicensesConsumed++;
            return consumedLicense;
        }
        
        private AppLicenseConsumer ConsumeAdditionalAvailableLicense(string appName, ConsumeLicenseInput consumeLicenseInput)
        {
            var access = DateTime.UtcNow;
            var consumedLicense = new AppLicenseConsumer(appName, AppIdentifier, access, access, true, consumeLicenseInput);
            AppLicenseConsumers.Add(consumedLicense);
            AdditionalLicensesConsumed++;
            return consumedLicense;
        }

        public TryReleaseLicenseOutput TryReleaseLicense(string user, bool isBundled, LicenseConsumeType licenseConsumeType)
        {
            if (AppIsBlocked())
                return new TryReleaseLicenseOutput(ReleaseAppLicenseStatusOld.AppBlocked, AppName, SoftwareIdentifier, SoftwareName);

            if (UserAlreadyConsumingLicense(user) && licenseConsumeType == LicenseConsumeType.Connection)
            {
                var userConsumedLicense = GetUserConsumedLicense(user)
                                            .DecreaseTimesUsedByUser();
                if (userConsumedLicense.TimesUsedByUser > 0)
                    return new TryReleaseLicenseOutput(ReleaseAppLicenseStatusOld.LicenseStillInUseByUser, userConsumedLicense.AppName, SoftwareIdentifier, SoftwareName, 
                        userConsumedLicense.Cnpj, userConsumedLicense.AccessDateTime, null, userConsumedLicense.LicenseUsageAdditionalInformation);
            }
            
            if (!HasConsumedLicense(user))
                return new TryReleaseLicenseOutput(ReleaseAppLicenseStatusOld.NoConsumedLicenseToRelease, AppName, SoftwareIdentifier, SoftwareName);
            
            var releaseConsumedLicenseOutput = ReleaseConsumedLicense(user, isBundled);
            return new TryReleaseLicenseOutput
            (
                ReleaseAppLicenseStatusOld.LicenseReleased,
                releaseConsumedLicenseOutput.AppName,
                SoftwareIdentifier, SoftwareName,
                releaseConsumedLicenseOutput.Cnpj,
                releaseConsumedLicenseOutput.LicenseUsageStartTime,
                releaseConsumedLicenseOutput.LicenseUsageEndTime,
                releaseConsumedLicenseOutput.LicenseUsageAdditionalInformation
            );
        }
        
        public TryReleaseLicenseOutput TryReleaseAdditionalLicense(string user, LicenseConsumeType licenseConsumeType)
        {
            if (AppIsBlocked())
                return new TryReleaseLicenseOutput(ReleaseAppLicenseStatusOld.AppBlocked, AppName, SoftwareIdentifier, SoftwareName);

            if (UserAlreadyConsumingAdditionalLicense(user) && licenseConsumeType == LicenseConsumeType.Connection)
            {
                var userConsumedLicense = GetUserConsumedLicense(user)
                                            .DecreaseTimesUsedByUser();
                if (userConsumedLicense.TimesUsedByUser > 0)
                    return new TryReleaseLicenseOutput(ReleaseAppLicenseStatusOld.AdditionalLicenseStillInUseByUser, userConsumedLicense.AppName, SoftwareIdentifier, SoftwareName, userConsumedLicense.Cnpj, userConsumedLicense.AccessDateTime);
            }
            
            if (!HasAdditionalConsumedLicense(user))
                return new TryReleaseLicenseOutput(ReleaseAppLicenseStatusOld.NoConsumedLicenseToRelease, SoftwareIdentifier, SoftwareName, AppName);
            
            var releaseConsumedLicenseOutput = ReleaseAdditionalConsumedLicense(user);
            return new TryReleaseLicenseOutput
            (
                ReleaseAppLicenseStatusOld.AdditionalLicenseReleased,
                releaseConsumedLicenseOutput.AppName, 
                SoftwareIdentifier, SoftwareName,
                releaseConsumedLicenseOutput.Cnpj,
                releaseConsumedLicenseOutput.LicenseUsageStartTime,
                releaseConsumedLicenseOutput.LicenseUsageEndTime,
                releaseConsumedLicenseOutput.LicenseUsageAdditionalInformation
            );
        }

        private ReleaseConsumedLicenseOutput ReleaseAdditionalConsumedLicense(string user)
        {
            var consumedLicense = GetUserConsumedLicense(user);
            AppLicenseConsumers.Remove(consumedLicense);
            AdditionalLicensesConsumed--;
            return new ReleaseConsumedLicenseOutput(DateTime.UtcNow, consumedLicense);
        }

        private ReleaseConsumedLicenseOutput ReleaseConsumedLicense(string user, bool isBundled)
        {
            var consumedLicense = GetUserConsumedLicense(user);
            AppLicenseConsumers.Remove(consumedLicense);
            if (!isBundled)
                AppLicensesConsumed--;
            return new ReleaseConsumedLicenseOutput(DateTime.UtcNow, consumedLicense);
        }

        public LicenseTenantStatusApp Clone()
        {
            var appLicenseConsumersCloned = AppLicenseConsumers.Select(c => c.Clone()).ToList();
            var licenseTenantStatusApp = new LicenseTenantStatusApp(this, appLicenseConsumersCloned);
            return licenseTenantStatusApp;
        }

        public bool TryRestoreLicenseInUse(AppLicenseConsumer licenseInUse, bool isBundled)
        {
            if (AppIsBlocked())
                return false;

            if (!isBundled && !HasAvailableLicense())
                return false;
            
            AppLicenseConsumers.Add(licenseInUse);
            if (licenseInUse.AdditionalLicense)
                AdditionalLicensesConsumed++;
            else if (!isBundled)
                AppLicensesConsumed++;
            
            return true;
        }
    }
}