using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.CustomerLicensing;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Extensions;
using Viasoft.Licensing.LicenseServer.Domain.Services.LicenseServer;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;

namespace Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus
{
    public class LicenseTenantStatusCurrent
    {
        public Dictionary<string, LicenseTenantStatusUsedBundle> Bundles { get; private set; }
        public Dictionary<string, LicenseTenantStatusApp> LooseApps { get; private set; } 
        public LicenseByTenantId TenantDetails { get; private set; }
        private LicensedTenantSettingsOutput LicensedTenantSettings { get; }
        private LicensedAccountDetails AccountDetails { get; }
        private readonly IProvideHardwareIdService _provideHardwareIdService;
        private readonly ILogger<LicenseTenantStatusCurrent> _logger;

        public LicenseTenantStatusCurrent(LicenseByTenantId tenantDetails, IProvideHardwareIdService provideHardwareIdService, 
            IExternalLicensingManagementService externalLicensingManagementService, ILogger<LicenseTenantStatusCurrent> logger)
        {
            Bundles = tenantDetails.GetBundleByIdentifier(externalLicensingManagementService);
            LooseApps = tenantDetails.GetLooseAppsByIdentifier(externalLicensingManagementService);
            AccountDetails = tenantDetails.LicensedTenantDetails.AccountDetails;
            LicensedTenantSettings = tenantDetails.LicensedTenantDetails.LicensedTenantSettings;
            TenantDetails = tenantDetails;
            _provideHardwareIdService = provideHardwareIdService;
            _logger = logger;
        }
        
        private bool IsCnpjLicensed(string cnpj) => TenantDetails.Cnpjs.Contains(cnpj);

        private bool DoesHardwareIdMatch(string hardwareId, bool useSimpleHardwareId) => _provideHardwareIdService.ProvideHardwareId(useSimpleHardwareId) == hardwareId;
        
        private bool IsTenantLicenseBlocked() => TenantDetails.Status == LicensingStatus.Blocked;

        private string TryGetAppFromBundle(string appIdentifier)
        {
            return Bundles.SingleOrDefault(pair => pair.Value.OwnsApp(appIdentifier)).Key;
        }

        public async Task<ConsumeLicenseOutput> ConsumeLicense(ConsumeLicenseInput input)
        {
            // Removed the hardwareId verification in web mode, due to docker being unable to get hardware information
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && DefaultConfigurationConsts.IsRunningAsLegacy)
            {
                var validBoolean = bool.TryParse(LicensedTenantSettings.Value, out var useSimpleHardwareId);
                if (!validBoolean)
                {
                    _logger.LogWarning("Não foi possível parsear a configuração de utilização de hardware id simples, e portando utilizaremos como false, o valor é {Value}", LicensedTenantSettings.Value);
                    useSimpleHardwareId = false;
                }
                if (!DoesHardwareIdMatch(TenantDetails.HardwareId, useSimpleHardwareId))
                {
                    return new ConsumeLicenseOutput(ConsumeAppLicenseStatus.HardwareIdDoesNotMatch, input.AppIdentifier);
                }
            }

            if (IsTenantLicenseBlocked())
                return new ConsumeLicenseOutput(ConsumeAppLicenseStatus.TenantBlocked, input.AppIdentifier);
            
            // alguns apps web não terão seleção de company, portanto nesse caso não validaremos o CNPJ
            // para ser sincero, esse validação nem deveria estar no servidor de licenças, e sim no cadastro de empresa
            if (!string.IsNullOrEmpty(input.Cnpj) && !IsCnpjLicensed(input.Cnpj))
                return new ConsumeLicenseOutput(ConsumeAppLicenseStatus.CnpjNotLicensed, input.AppIdentifier);

            var app = TryGetAppFromBundle(input.AppIdentifier);
            if (!string.IsNullOrEmpty(app) && Bundles.TryGetValue(app, out var bundle))
            {
                var tryConsumeLicenseOutput = await bundle.TryConsumeLicense(input, TenantDetails.LicenseConsumeType);
                return new ConsumeLicenseOutput(tryConsumeLicenseOutput.ConsumeAppLicenseStatus, input.AppIdentifier, tryConsumeLicenseOutput.AppName, 
                    tryConsumeLicenseOutput.SoftwareIdentifier, tryConsumeLicenseOutput.SoftwareName, 
                    tryConsumeLicenseOutput.LicenseUsageStartTime, bundle.BundleIdentifier, bundle.BundleName);
            }

            if (LooseApps.TryGetValue(input.AppIdentifier, out var looseApp))
            {
                var tryConsumeLicenseOutput =  await looseApp.TryConsumeLicense(input, false, TenantDetails.LicenseConsumeType);
                return new ConsumeLicenseOutput(tryConsumeLicenseOutput.ConsumeAppLicenseStatus, input.AppIdentifier, tryConsumeLicenseOutput.AppName,
                    tryConsumeLicenseOutput.SoftwareIdentifier, tryConsumeLicenseOutput.SoftwareName, 
                    tryConsumeLicenseOutput.LicenseUsageStartTime);
            }
            return new ConsumeLicenseOutput(ConsumeAppLicenseStatus.AppNotLicensed, input.AppIdentifier);
        }

        public ReleaseLicenseOutput ReleaseLicense(ReleaseLicenseInput input)
        {
            var app = TryGetAppFromBundle(input.AppIdentifier);
            if (!string.IsNullOrEmpty(app) && Bundles.TryGetValue(app, out var bundle))
            {
                var releaseLicenseOutput = bundle.TryReleaseLicense(input.AppIdentifier, input.User, TenantDetails.LicenseConsumeType, input.Token, input.IsTerminalServer);
                return new ReleaseLicenseOutput(
                    releaseLicenseOutput.ReleaseAppLicenseStatus, input.AppIdentifier, releaseLicenseOutput.Cnpj,
                    releaseLicenseOutput.AppName, releaseLicenseOutput.SoftwareIdentifier, releaseLicenseOutput.SoftwareName,
                    releaseLicenseOutput.LicenseUsageStartTime, releaseLicenseOutput.LicenseUsageEndTime, releaseLicenseOutput.LicenseUsageAdditionalInformation,
                    bundle.BundleIdentifier, bundle.BundleName);
            }

            if (LooseApps.TryGetValue(input.AppIdentifier, out var looseApp))
            {
                var releaseLicenseOutput =  looseApp.TryReleaseLicense(input.User, false, TenantDetails.LicenseConsumeType, input.Token, input.IsTerminalServer);
                return new ReleaseLicenseOutput(
                    releaseLicenseOutput.ReleaseAppLicenseStatus, input.AppIdentifier, releaseLicenseOutput.Cnpj, 
                    releaseLicenseOutput.AppName, releaseLicenseOutput.SoftwareIdentifier, releaseLicenseOutput.SoftwareName,  
                    releaseLicenseOutput.LicenseUsageStartTime, releaseLicenseOutput.LicenseUsageEndTime, releaseLicenseOutput.LicenseUsageAdditionalInformation);
            }
            
            return new ReleaseLicenseOutput(ReleaseAppLicenseStatus.AppNotLicensed, input.AppIdentifier, input.Cnpj);
        }

        public List<LicenseUsageInRealTimeDetails> GetLicenseUsageInRealTime()
        {
            var licensesUsageInRealTime = new List<LicenseUsageInRealTimeDetails>();
            
            foreach (var looseApp in LooseApps.Values.Where(app => app.AppLicenseConsumers.Any()))
            {
                foreach (var (key, value) in looseApp.AppLicenseConsumers)
                {
                    licensesUsageInRealTime.AddRange(value.Select(consumedLicense => new LicenseUsageInRealTimeDetails
                    (
                        TenantDetails.Identifier,
                        consumedLicense.User,
                        consumedLicense.AppIdentifier,
                        consumedLicense.AppName, 
                        null,
                        null,
                        consumedLicense.AccessDateTime,
                        looseApp.AppLicenses,
                        looseApp.AppLicensesConsumed,
                        consumedLicense.Cnpj,
                        TenantDetails.Status,
                        looseApp.Status,
                        looseApp.SoftwareName,
                        looseApp.SoftwareIdentifier,
                        consumedLicense.LicenseUsageAdditionalInformation,
                        key,
                        looseApp.LicensingModel,
                        looseApp.LicensingMode,
                        consumedLicense.LastHeartbeatDateTime
                    )).ToList());
                }
            }

            foreach (var bundle in Bundles.Values)
            {
                foreach (var bundledApp in bundle.OwnedApps.Values.Where(app => app.AppLicenseConsumers.Any()))
                {

                    foreach (var (key, value) in bundledApp.AppLicenseConsumers)
                    {
                        licensesUsageInRealTime.AddRange(
                            value
                                .Select(consumedLicense => new LicenseUsageInRealTimeDetails
                                    (
                                        TenantDetails.Identifier,
                                        consumedLicense.User,
                                        consumedLicense.AppIdentifier,
                                        consumedLicense.AppName,
                                        bundle.BundleIdentifier,
                                        bundle.BundleName,
                                        consumedLicense.AccessDateTime,
                                        bundle.BundleLicenseCount,
                                        bundle.BundleConsumedLicenseCount,
                                        consumedLicense.Cnpj,
                                        TenantDetails.Status,
                                        bundledApp.Status,
                                        bundledApp.SoftwareName,
                                        bundledApp.SoftwareIdentifier,
                                        consumedLicense.LicenseUsageAdditionalInformation,
                                        key,
                                        bundledApp.LicensingModel,
                                        bundledApp.LicensingMode,
                                        consumedLicense.LastHeartbeatDateTime
                                    )
                                )
                                .ToList()); 
                    }
                }
            }
            
            return licensesUsageInRealTime;
        }
        
        public IEnumerable<ReleaseLicenseInput> GetLicensesToReleaseBasedOnHeartbeat(DateTime minimumAllowedHeartbeat)
        {
            var licenseConsumers = Bundles
                .SelectMany(x => x.Value.OwnedApps.SelectMany(c => c.Value.AppLicenseConsumers.Select(s =>
                {
                    var output = s.Value.Where(appLicenseConsumer =>
                        appLicenseConsumer.LastHeartbeatDateTime <= minimumAllowedHeartbeat).ToList();
                    return new LicenseConsumersToRelease
                    {
                        Token = s.Key,
                        Consumers = output
                    };
                })))
                .ToList();

            licenseConsumers.AddRange(LooseApps
                .SelectMany(x => x.Value.AppLicenseConsumers.Select(s =>
                {
                    var output = s.Value.Where(consumer => consumer.LastHeartbeatDateTime <= minimumAllowedHeartbeat).ToList();
                    return new LicenseConsumersToRelease
                    {
                        Token = s.Key,
                        Consumers = output
                    };
                })));
            
            return GenerateReleaseLicenseInputsFromLicenseConsumers(licenseConsumers);
        }

        public bool RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInput input)
        {
            var licensesToUpdateHeartbeat = Bundles
                .SelectMany(x =>
                {
                    var (_, bundleStatus) = x;
                    var appsInBundle = bundleStatus.OwnedApps;
                    return appsInBundle.SelectMany(c =>
                    {
                        return c.Value.AppLicenseConsumers.ContainsKey(input.Token)
                            ? c.Value.AppLicenseConsumers[input.Token].Where(s =>
                                s.User.Equals(input.User, StringComparison.CurrentCultureIgnoreCase) &&
                                s.AppIdentifier.Equals(input.AppIdentifier, StringComparison.CurrentCultureIgnoreCase))
                            : new List<AppLicenseConsumer>();
                    });
                })
                .ToList();
            
            licensesToUpdateHeartbeat.AddRange( LooseApps
                .SelectMany(x => x.Value.AppLicenseConsumers.ContainsKey(input.Token) ? x.Value.AppLicenseConsumers[input.Token].Where(s => s.User.Equals(input.User, StringComparison.CurrentCultureIgnoreCase) && s.AppIdentifier.Equals(input.AppIdentifier, StringComparison.CurrentCultureIgnoreCase)): new List<AppLicenseConsumer>())
                .ToList());

            foreach (var licenseToUpdateHeartbeat in licensesToUpdateHeartbeat)
                licenseToUpdateHeartbeat.UpdateHeartbeat();

            return licensesToUpdateHeartbeat.Any();
        }

        private IEnumerable<ReleaseLicenseInput> GenerateReleaseLicenseInputsFromLicenseConsumers(List<LicenseConsumersToRelease> licenseConsumers)
        {
            foreach (var licenseConsumersToRelease in licenseConsumers)
            {
                foreach (var licenseConsumer in licenseConsumersToRelease.Consumers)
                    for (var i = 0; i < licenseConsumer.TimesUsedByUser; i++)
                        yield return new ReleaseLicenseInput
                        {
                            TenantId = TenantDetails.Identifier,
                            AppIdentifier = licenseConsumer.AppIdentifier,
                            User = licenseConsumer.User,
                            Cnpj = licenseConsumer.Cnpj,
                            Token = licenseConsumersToRelease.Token
                        }; 
            }
        }

        public List<KeyValuePair<string, List<AppLicenseConsumer>>> GetAllLicensesInUse()
        {
            var licenseConsumers = Bundles
                .SelectMany(x => x.Value.OwnedApps.SelectMany(c => c.Value.AppLicenseConsumers))
                .ToList();
            
            licenseConsumers.AddRange(LooseApps.SelectMany(x => x.Value.AppLicenseConsumers));

            return licenseConsumers
                .OrderBy(x => x.Key)
                .ToList();
        }

        public async Task RestoreLicensesInUse(List<KeyValuePair<string, List<AppLicenseConsumer>>> licensesInUse)
        {
            foreach (var (token, appLicenseConsumers) in licensesInUse)
            {
                foreach (var licenseInUse in appLicenseConsumers)
                {
                    var input = new ConsumeLicenseInput
                    {
                        TenantId = TenantDetails.Identifier,
                        AppIdentifier = licenseInUse.AppIdentifier,
                        User = licenseInUse.User,
                        Cnpj = licenseInUse.Cnpj,
                        Token = token,
                        IsTerminalServer = false,
                        DeviceId = licenseInUse.NamedUserLicense?.DeviceId,
                        LicenseUsageAdditionalInformation = licenseInUse.LicenseUsageAdditionalInformation != null 
                            ? new LicenseUsageAdditionalInformation 
                            {
                                SoftwareVersion = licenseInUse.LicenseUsageAdditionalInformation.SoftwareVersion,
                                HostName = licenseInUse.LicenseUsageAdditionalInformation.HostName,
                                HostUser = licenseInUse.LicenseUsageAdditionalInformation.HostUser,
                                Language = licenseInUse.LicenseUsageAdditionalInformation.Language,
                                OsInfo = licenseInUse.LicenseUsageAdditionalInformation.OsInfo,
                                BrowserInfo = licenseInUse.LicenseUsageAdditionalInformation.BrowserInfo,
                                DatabaseName = licenseInUse.LicenseUsageAdditionalInformation.DatabaseName,
                                LocalIpAddress = licenseInUse.LicenseUsageAdditionalInformation.LocalIpAddress
                            } : null
                    };

                    await ConsumeLicense(input);
                }
            }
        }
    }
}