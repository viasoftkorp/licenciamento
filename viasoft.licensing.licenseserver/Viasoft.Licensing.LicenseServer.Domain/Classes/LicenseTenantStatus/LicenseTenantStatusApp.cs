using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Abstractions.NamedUserLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicenseServer.Domain.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Extensions;
using Viasoft.Licensing.LicenseServer.Domain.Services.LicenseServer;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus
{
    public class LicenseTenantStatusApp
    {
        public string AppIdentifier { get; }
        public string AppName { get; }
        public int AppLicenses { get; }
        public int AppLicensesConsumed { get; private set; }
        public LicensedAppStatus Status { get; }
        public string StatusDescription => Status.ToString();        
        public readonly Dictionary<string, List<AppLicenseConsumer>> AppLicenseConsumers;
        public LicensingModels LicensingModel { get; }
        public LicensingModes? LicensingMode { get; }
        public readonly List<INamedUserLicense> NamedUserLicenses = new();
        public string SoftwareIdentifier { get; set; }
        public string SoftwareName { get; set; }
        
        private readonly IExternalLicensingManagementService _externalLicensingManagementService;

        private bool AppIsBlocked => Status == LicensedAppStatus.AppBlocked;

        private bool IsNamed => LicensingModel == LicensingModels.Named;

        private bool IsOnline => LicensingMode == LicensingModes.Online;
        
        private bool IsOffline => LicensingMode == LicensingModes.Offline;

        private INamedUserLicense GetNamedUserLicense(string user) => NamedUserLicenses.First(u => u.NamedUserEmail.Equals(user, StringComparison.CurrentCultureIgnoreCase));

        private bool HasAvailableNamedLicense(string user) => NamedUserLicenses.Any(a => a.NamedUserEmail.Equals(user, StringComparison.CurrentCultureIgnoreCase));

        private bool HasAvailableLicense() => AppLicenses - AppLicensesConsumed > 0;
        
        public LicenseTenantStatusApp(int licenseCount, string appIdentifier, string appName, LicensedAppStatus status, string softwareName, string softwareIdentifier, 
            List<NamedUserAppLicenseOutput> namedUserAppLicenses, LicensingModels licensingModel, LicensingModes? licensingMode, List<NamedUserBundleLicenseOutput> namedUserBundleLicenseOutputs,
            IExternalLicensingManagementService externalLicensingManagementService)
        {
            _externalLicensingManagementService = externalLicensingManagementService;
            AppLicenseConsumers = new Dictionary<string, List<AppLicenseConsumer>>();
            AppLicenses = licenseCount >= 0 ? licenseCount : 0;
            AppIdentifier = appIdentifier;
            AppName = appName;
            Status = status;
            AppLicensesConsumed = 0;
            SoftwareIdentifier = softwareIdentifier;
            SoftwareName = softwareName;
            LicensingMode = licensingMode;
            LicensingModel = licensingModel;
            NamedUserLicenses.AddRange(namedUserAppLicenses);
            NamedUserLicenses.AddRange(namedUserBundleLicenseOutputs);
        }

        //refactor this and use chain of responsibility
        public async Task<TryConsumeLicenseOutput> TryConsumeLicense(ConsumeLicenseInput input, bool isBundled, LicenseConsumeType licenseConsumeType)
        {
            var appName = !string.IsNullOrEmpty(input.CustomAppName) ? input.CustomAppName : AppName;
            
            if (AppIsBlocked)
                return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.AppBlocked, appName, SoftwareIdentifier, SoftwareName);

            if (TryGetConsumedLicense(input.User, input.Token, out var consumedLicense) && licenseConsumeType == LicenseConsumeType.Connection && !input.IsTerminalServer)
            {
                var repeatedLicenseConsumer = consumedLicense.IncreaseTimesUsedByUser().UpdateHeartbeat();
                return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.LicenseAlreadyInUseByUser, appName, SoftwareIdentifier, SoftwareName, repeatedLicenseConsumer.AccessDateTime);
            }
            
            if (!HasAvailableLicense() && !isBundled)
            {
                return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.NotEnoughLicenses, appName, SoftwareIdentifier, SoftwareName);
            }

            if (IsNamed)
            {
                if (!HasAvailableNamedLicense(input.User))
                    return new TryConsumeLicenseOutput(isBundled ? ConsumeAppLicenseStatus.NamedBundleLicenseNotAvailable : ConsumeAppLicenseStatus.NamedAppLicenseNotAvailable, appName, SoftwareIdentifier, SoftwareName);

                if (IsOnline && !AppLicenseConsumers.ContainsKey(input.Token))
                {
                    foreach (var (_, consumers) in AppLicenseConsumers)
                    {
                        if (consumers.Any(consumer => consumer.NamedUserLicense.NamedUserEmail.Equals(input.User, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.CantConsumeOnlineNamedLicenseInDifferentComputers, appName, SoftwareIdentifier, SoftwareName);
                        }
                    }
                }

                if (IsOffline)
                {
                    if (string.IsNullOrEmpty(input.DeviceId))
                    {
                        return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.EmptyDeviceId, appName, SoftwareIdentifier, SoftwareName);
                    }
                    
                    var userNamedLicense = GetNamedUserLicense(input.User);

                    if (string.IsNullOrEmpty(userNamedLicense.DeviceId))
                    {
                        if (userNamedLicense.IsNamedUserAppLicense(out var appId))
                        {
                            var updateNamedUserAppResult = await _externalLicensingManagementService.UpdateNamedUserApp(new UpdateNamedUserAppLicenseInput
                            {
                                DeviceId = input.DeviceId,
                                NamedUserEmail = userNamedLicense.NamedUserEmail,
                                NamedUserId = userNamedLicense.NamedUserId
                            }, userNamedLicense.TenantId, userNamedLicense.LicensedTenantId, appId, userNamedLicense.Id);

                            if (updateNamedUserAppResult.ValidationCode != UpdateNamedUserAppLicenseValidationCode.NoError)
                            {
                                return updateNamedUserAppResult.ToConsumeLicenseOutput(this);
                            }
                        }
                        else if (userNamedLicense.IsNamedUserBundleLicense(out var bundleId))
                        {
                            var updateNamedUserBundleLicenseOutput = await _externalLicensingManagementService.UpdateNamedUserBundle(new UpdateNamedUserBundleLicenseInput
                            {
                                DeviceId = input.DeviceId,
                                NamedUserEmail = userNamedLicense.NamedUserEmail,
                                NamedUserId = userNamedLicense.NamedUserId
                            }, userNamedLicense.TenantId, userNamedLicense.LicensedTenantId, bundleId, userNamedLicense.Id);

                            if (updateNamedUserBundleLicenseOutput.ValidationCode != UpdateNamedUserBundleLicenseValidationCode.NoError)
                            {
                                return updateNamedUserBundleLicenseOutput.ToConsumeLicenseOutput(this);
                            }
                        }

                        userNamedLicense.DeviceId = input.DeviceId;
                    }

                    if (userNamedLicense.DeviceId != input.DeviceId)
                    {
                        return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.NamedUserLicenseDeviceIdDoesNotMatch, appName, SoftwareIdentifier, SoftwareName);
                    }
                }
            }
            
            var newLicenseConsumer = ConsumeAvailableLicense(appName, input, isBundled);
            return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.LicenseConsumed, appName, SoftwareIdentifier, SoftwareName, newLicenseConsumer.AccessDateTime);
        }
        
        public TryReleaseLicenseOutput TryReleaseLicense(string user, bool isBundled, LicenseConsumeType licenseConsumeType, string sid, bool isTerminalServer)
        {
            if (AppIsBlocked)
                return new TryReleaseLicenseOutput(ReleaseAppLicenseStatus.AppBlocked, AppName, SoftwareIdentifier, SoftwareName);

            if (!TryGetConsumedLicense(user, sid, out var consumedLicense))
            {
                return new TryReleaseLicenseOutput(ReleaseAppLicenseStatus.NoConsumedLicenseToRelease, AppName, SoftwareIdentifier, SoftwareName);
            }

            if (licenseConsumeType == LicenseConsumeType.Connection && !isTerminalServer)
            {
                var userConsumedLicense = consumedLicense.DecreaseTimesUsedByUser();
                if (userConsumedLicense.TimesUsedByUser > 0)
                    return new TryReleaseLicenseOutput(ReleaseAppLicenseStatus.LicenseStillInUseByUser, userConsumedLicense.AppName, SoftwareIdentifier, SoftwareName, 
                        userConsumedLicense.Cnpj, userConsumedLicense.AccessDateTime, null, userConsumedLicense.LicenseUsageAdditionalInformation);
            }

            RemoveConsumerFromConsumedLicenses(sid, consumedLicense);
            
            if (!isBundled)
                AppLicensesConsumed--;
            
            return new TryReleaseLicenseOutput
            (
                ReleaseAppLicenseStatus.LicenseReleased,
                consumedLicense.AppName,
                SoftwareIdentifier, SoftwareName,
                consumedLicense.Cnpj,
                consumedLicense.AccessDateTime,
                DateTime.UtcNow,
                consumedLicense.LicenseUsageAdditionalInformation
            );
        }
        
        private AppLicenseConsumer ConsumeAvailableLicense(string appName, ConsumeLicenseInput consumeLicenseInput, bool isBundled)
        {
            var access = DateTime.UtcNow;
            var namedUserLicense = NamedUserLicenses.FirstOrDefault(u => u.NamedUserEmail.Equals(consumeLicenseInput.User, StringComparison.CurrentCultureIgnoreCase));

            var consumedLicense = new AppLicenseConsumer(appName, AppIdentifier, access, access, consumeLicenseInput, namedUserLicense);
            
            if (!AppLicenseConsumers.TryGetValue(consumeLicenseInput.Token, out var consumers))
            {
                consumers = new List<AppLicenseConsumer>();
                AppLicenseConsumers.Add(consumeLicenseInput.Token, consumers);
            }
            
            consumers.Add(consumedLicense);
            
            if (!isBundled)
                AppLicensesConsumed++;
            
            return consumedLicense;
        }

        private void RemoveConsumerFromConsumedLicenses(string sid, AppLicenseConsumer consumedLicense)
        {
            var consumers = AppLicenseConsumers[sid];
            consumers.Remove(consumedLicense);
            if (consumers.Count == 0)
            {
                AppLicenseConsumers.Remove(sid);
            }
        }
        
        public bool HasConsumedLicense(string user, string sid)
        {
            return TryGetConsumedLicense(user, sid, out _);
        }

        private bool TryGetConsumedLicense(string user, string sid, out AppLicenseConsumer license)
        {
            license = null;
            
            if (AppLicenseConsumers.TryGetValue(sid, out var consumers))
            {
                license = consumers.FirstOrDefault(consumer => consumer.User.Equals(user, StringComparison.CurrentCultureIgnoreCase));
            }

            return license != null;
        }
    }
}