using System;
using System.Collections.Generic;
using System.Linq;
using Viasoft.Licensing.LicenseServer.Domain.Old.Extensions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus
{
    public class LicenseTenantStatusCurrentOld
    {
        public LicensedAccountDetails AccountDetails { get; }
        public Dictionary<string, LicenseTenantStatusUsedBundle> Bundles { get; private set; }
        public Dictionary<string, LicenseTenantStatusApp> LooseApps { get; private set; } 
        public LicenseByTenantIdOld TenantDetails { get; private set; }
        private readonly IProvideHardwareIdService _provideHardwareIdService;

        public LicenseTenantStatusCurrentOld(LicenseByTenantIdOld tenantDetails, IProvideHardwareIdService provideHardwareIdService)
        {
            Bundles = tenantDetails.LicensedTenantDetails.OwnedBundles.ToDictionary(
                details => details.Identifier,
                details =>
                    {
                        var appsInBundle = tenantDetails.LicensedTenantDetails.OwnedApps
                            .Where(appDetails => appDetails.LicensedBundleId.HasValue && appDetails.LicensedBundleId == details.BundleId)
                            .ToList(); 
                        return new LicenseTenantStatusUsedBundle(details, appsInBundle);
                    }, 
                StringComparer.OrdinalIgnoreCase);
            
            LooseApps = tenantDetails.LicensedTenantDetails.OwnedApps
                .Where(details => !details.LicensedBundleId.HasValue)
                .ToDictionary(
                    details => details.Identifier,
                    details => new LicenseTenantStatusApp(details.NumberOfLicenses, details.AdditionalNumberOfLicenses, details.Identifier,
                        details.Name, details.Status, details.SoftwareName, details.SoftwareIdentifier), 
                    StringComparer.OrdinalIgnoreCase);
            AccountDetails = tenantDetails.LicensedTenantDetails.AccountDetails;
            tenantDetails.LicensedTenantDetails = null;
            TenantDetails = tenantDetails;
            _provideHardwareIdService = provideHardwareIdService;
        }

        private LicenseTenantStatusCurrentOld(Dictionary<string, LicenseTenantStatusUsedBundle> bundles,
            Dictionary<string, LicenseTenantStatusApp> looseApps, LicenseByTenantIdOld tenantDetails, LicensedAccountDetails accountDetails, IProvideHardwareIdService provideHardwareIdService)
        {
            AccountDetails = accountDetails;
            _provideHardwareIdService = provideHardwareIdService;
            Bundles = bundles;
            LooseApps = looseApps;
            TenantDetails = tenantDetails;
        }
        
        private bool IsCnpjLicensed(string cnpj) => TenantDetails.Cnpjs.Contains(cnpj);

        private bool DoesHardwareIdMatch(string hardwareId) => _provideHardwareIdService.ProvideHardwareId(false) == hardwareId;
        
        private bool IsTenantLicenseBlocked() => TenantDetails.Status == LicensingStatus.Blocked;

        private string TryGetAppFromBundle(string appIdentifier)
        {
            return Bundles.SingleOrDefault(pair => pair.Value.OwnsApp(appIdentifier)).Key;
        }

        public ConsumeLicenseOutputOld ConsumeLicense(ConsumeLicenseInput input)
        {
            // Removed the hardwareId verification in web mode, due to docker being unable to get hardware information
            if (DefaultConfigurationConsts.IsRunningAsLegacy && !DoesHardwareIdMatch(TenantDetails.HardwareId))
                return new ConsumeLicenseOutputOld(ConsumeAppLicenseStatusOld.HardwareIdDoesNotMatch, input.AppIdentifier);
            
            if (IsTenantLicenseBlocked())
                return new ConsumeLicenseOutputOld(ConsumeAppLicenseStatusOld.TenantBlocked, input.AppIdentifier);
            
            if (!IsCnpjLicensed(input.Cnpj))
                return new ConsumeLicenseOutputOld(ConsumeAppLicenseStatusOld.CnpjNotLicensed, input.AppIdentifier);

            var app = TryGetAppFromBundle(input.AppIdentifier);
            if (!string.IsNullOrEmpty(app) && Bundles.TryGetValue(app, out var bundle))
            {
                var tryConsumeLicenseOutput = bundle.TryConsumeLicense(input, TenantDetails.LicenseConsumeType);
                return new ConsumeLicenseOutputOld(tryConsumeLicenseOutput.ConsumeAppLicenseStatus, input.AppIdentifier, tryConsumeLicenseOutput.AppName, 
                    tryConsumeLicenseOutput.SoftwareIdentifier, tryConsumeLicenseOutput.SoftwareName, 
                    tryConsumeLicenseOutput.LicenseUsageStartTime, bundle.BundleIdentifier, bundle.BundleName);
            }

            if (LooseApps.TryGetValue(input.AppIdentifier, out var looseApp))
            {
                var tryConsumeLicenseOutput =  looseApp.TryConsumeLicense(input, false, TenantDetails.LicenseConsumeType);
                return new ConsumeLicenseOutputOld(tryConsumeLicenseOutput.ConsumeAppLicenseStatus, input.AppIdentifier, tryConsumeLicenseOutput.AppName,
                    tryConsumeLicenseOutput.SoftwareIdentifier, tryConsumeLicenseOutput.SoftwareName, 
                    tryConsumeLicenseOutput.LicenseUsageStartTime);
            }
            return new ConsumeLicenseOutputOld(ConsumeAppLicenseStatusOld.AppNotLicensed, input.AppIdentifier);
        }

        public ReleaseLicenseOutputOld ReleaseLicense(ReleaseLicenseInput input)
        {
            var app = TryGetAppFromBundle(input.AppIdentifier);
            if (!string.IsNullOrEmpty(app) && Bundles.TryGetValue(app, out var bundle))
            {
                var releaseLicenseOutput = bundle.TryReleaseLicense(input.AppIdentifier, input.User, TenantDetails.LicenseConsumeType);
                return new ReleaseLicenseOutputOld(
                    releaseLicenseOutput.ReleaseAppLicenseStatus, input.AppIdentifier, releaseLicenseOutput.Cnpj,
                    releaseLicenseOutput.AppName, releaseLicenseOutput.SoftwareIdentifier, releaseLicenseOutput.SoftwareName,
                    releaseLicenseOutput.LicenseUsageStartTime, releaseLicenseOutput.LicenseUsageEndTime, releaseLicenseOutput.LicenseUsageAdditionalInformation,
                    bundle.BundleIdentifier, bundle.BundleName);
            }

            if (LooseApps.TryGetValue(input.AppIdentifier, out var looseApp))
            {
                var releaseLicenseOutput =  looseApp.TryReleaseLicense(input.User, false, TenantDetails.LicenseConsumeType);
                return new ReleaseLicenseOutputOld(
                    releaseLicenseOutput.ReleaseAppLicenseStatus, input.AppIdentifier, releaseLicenseOutput.Cnpj, 
                    releaseLicenseOutput.AppName, releaseLicenseOutput.SoftwareIdentifier, releaseLicenseOutput.SoftwareName,  
                    releaseLicenseOutput.LicenseUsageStartTime, releaseLicenseOutput.LicenseUsageEndTime, releaseLicenseOutput.LicenseUsageAdditionalInformation);
            }
            
            return new ReleaseLicenseOutputOld(ReleaseAppLicenseStatusOld.AppNotLicensed, input.AppIdentifier, input.Cnpj);
        }

        public LicenseTenantStatusCurrentOld Clone()
        {                                    
            var clonedBundles = Bundles.ToDictionary(s => s.Key, s => s.Value.Clone(), StringComparer.OrdinalIgnoreCase);
            var clonedLooseApps = LooseApps.ToDictionary(s => s.Key, s => s.Value.Clone(), StringComparer.OrdinalIgnoreCase);
            var clonedTenantDetails = TenantDetails.Clone();
            var accountDetails = AccountDetails.Clone();
            
            var licenseTenantStatusCurrent = new LicenseTenantStatusCurrentOld(clonedBundles, clonedLooseApps, clonedTenantDetails, accountDetails, _provideHardwareIdService);
            return licenseTenantStatusCurrent;
        }
        
        public void RestoreFromMemento(LicenseTenantStatusCurrentOld memento)
        {
            Bundles = memento.Bundles;
            LooseApps = memento.LooseApps;
            TenantDetails = memento.TenantDetails;
        }
        
        public int GetAvailableLicense(string appIdentifier)
        {
            var app = TryGetAppFromBundle(appIdentifier);
            if (!string.IsNullOrEmpty(app) && Bundles.TryGetValue(app, out var bundle))
                return bundle.BundleLicenseCount - bundle.BundleConsumedLicenseCount;

            return LooseApps.TryGetValue(appIdentifier, out var looseApp) ? looseApp.AppLicenses - looseApp.AppLicensesConsumed: 0;
        }
        
        public int GetAvailableAdditionalLicense(string appIdentifier)
        {
            var app = TryGetAppFromBundle(appIdentifier);
            if (!string.IsNullOrEmpty(app) && Bundles.TryGetValue(app, out var bundle))
                return bundle.GetAppAvailableAdditionalLicense(appIdentifier);
            
            return 0;
        }

        public List<LicenseUsageInRealTimeDetails> GetLicenseUsageInRealTime()
        {
            var licensesUsageInRealTime = new List<LicenseUsageInRealTimeDetails>();

            foreach (var looseApp in LooseApps.Values.Where(app => app.AppLicenseConsumers.Any()))
            {
                licensesUsageInRealTime.AddRange(
                    looseApp.AppLicenseConsumers
                        .Select(consumedLicense => new LicenseUsageInRealTimeDetails
                            (
                                TenantDetails.Identifier,
                                consumedLicense.User,
                                consumedLicense.AppIdentifier,
                                consumedLicense.AppName, 
                                null,
                                null,
                                consumedLicense.AdditionalLicense,
                                consumedLicense.AccessDateTime,
                                looseApp.AppLicenses,
                                looseApp.AppLicensesConsumed,
                                looseApp.AdditionalLicenses,
                                looseApp.AdditionalLicensesConsumed,
                                consumedLicense.Cnpj,
                                TenantDetails.Status,
                                looseApp.Status,
                                looseApp.SoftwareName,
                                looseApp.SoftwareIdentifier,
                                consumedLicense.LicenseUsageAdditionalInformation
                            )
                        )
                        .ToList());
            }

            foreach (var bundle in Bundles.Values)
            {
                foreach (var bundledApp in bundle.OwnedApps.Values.Where(app => app.AppLicenseConsumers.Any()))
                {
                    licensesUsageInRealTime.AddRange(
                        bundledApp.AppLicenseConsumers
                            .Select(consumedLicense => new LicenseUsageInRealTimeDetails
                                (
                                    TenantDetails.Identifier,
                                    consumedLicense.User,
                                    consumedLicense.AppIdentifier,
                                    consumedLicense.AppName,
                                    bundle.BundleIdentifier,
                                    bundle.BundleName,
                                    consumedLicense.AdditionalLicense,
                                    consumedLicense.AccessDateTime,
                                    bundle.BundleLicenseCount,
                                    bundle.BundleConsumedLicenseCount,
                                    bundledApp.AdditionalLicenses,
                                    bundledApp.AdditionalLicensesConsumed,
                                    consumedLicense.Cnpj,
                                    TenantDetails.Status,
                                    bundledApp.Status,
                                    bundledApp.SoftwareName,
                                    bundledApp.SoftwareIdentifier,
                                    consumedLicense.LicenseUsageAdditionalInformation
                                )
                            )
                            .ToList()); 
                }
            }

            return licensesUsageInRealTime;
        }
        
        public IEnumerable<ReleaseLicenseInput> GetLicensesToReleaseBasedOnHeartbeat(DateTime minimumAllowedHeartbeat)
        {
            var licenseConsumers = Bundles
                .SelectMany(x => x.Value.OwnedApps.SelectMany(c => c.Value.AppLicenseConsumers.Where(s => s.LastHeartbeatDateTime <= minimumAllowedHeartbeat)))
                .ToList();
            
            licenseConsumers.AddRange( LooseApps
                .SelectMany(x => x.Value.AppLicenseConsumers.Where(s => s.LastHeartbeatDateTime <= minimumAllowedHeartbeat))
                .ToList());

            return GenerateReleaseLicenseInputsFromLicenseConsumers(licenseConsumers);
        }

        public bool RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInputOld inputOld)
        {
            var licensesToUpdateHeartbeat = Bundles
                .SelectMany(x => x.Value.OwnedApps.SelectMany(c => c.Value.AppLicenseConsumers.Where(s => s.User.EqualsIgnoringCase(inputOld.User) && s.AppIdentifier.EqualsIgnoringCase(inputOld.AppIdentifier))))
                .ToList();
            
            licensesToUpdateHeartbeat.AddRange( LooseApps
                .SelectMany(x => x.Value.AppLicenseConsumers.Where(s => s.User.EqualsIgnoringCase(inputOld.User) && s.AppIdentifier.EqualsIgnoringCase(inputOld.AppIdentifier)))
                .ToList());

            foreach (var licenseToUpdateHeartbeat in licensesToUpdateHeartbeat)
                licenseToUpdateHeartbeat.UpdateHeartbeat();

            return licensesToUpdateHeartbeat.Any();
        }

        public List<string> GetTenantLicensedApps()
        {
            var appsIdentifiers = Bundles
                .SelectMany(x => x.Value.OwnedApps.Select(m => m.Value.AppIdentifier))
                .ToList();
            
            appsIdentifiers.AddRange(
                LooseApps.Select(x => x.Value.AppIdentifier));

            appsIdentifiers = appsIdentifiers.Distinct().ToList();

            return appsIdentifiers;
        }

        private IEnumerable<ReleaseLicenseInput> GenerateReleaseLicenseInputsFromLicenseConsumers(IEnumerable<AppLicenseConsumer> licenseConsumers)
        {
            foreach (var licenseConsumer in licenseConsumers)
                for (var i = 0; i < licenseConsumer.TimesUsedByUser; i++)
                    yield return new ReleaseLicenseInput
                    {
                        TenantId = TenantDetails.Identifier,
                        AppIdentifier = licenseConsumer.AppIdentifier,
                        User = licenseConsumer.User,
                        Cnpj = licenseConsumer.Cnpj
                    }; 
        }

        public IEnumerable<AppLicenseConsumer> GetAllLicensesInUse()
        {
            var licenseConsumers = Bundles
                .SelectMany(x => x.Value.OwnedApps.SelectMany(c => c.Value.AppLicenseConsumers))
                .ToList();
            
            licenseConsumers.AddRange(LooseApps.SelectMany(x => x.Value.AppLicenseConsumers));

            return licenseConsumers
                .OrderBy(x => x.AccessDateTime)
                .ToList();
        }

        public bool RestoreLicensesInUse(IEnumerable<AppLicenseConsumer> licensesInUse)
        {
            var result = false;
            foreach (var licenseInUse in licensesInUse)
            {
                var app = TryGetAppFromBundle(licenseInUse.AppIdentifier);

                if (!string.IsNullOrEmpty(app) && Bundles.TryGetValue(app, out var bundle))
                {
                    if (bundle.TryRestoreLicenseInUse(licenseInUse, TenantDetails.LicenseConsumeType))
                        result = true;
                    continue;
                }

                if (LooseApps.TryGetValue(licenseInUse.AppIdentifier, out var looseApp))
                    if (looseApp.TryRestoreLicenseInUse(licenseInUse, false))
                        result = true;
            }

            return result;
        }
    }
}