using System;
using System.Collections.Generic;
using System.Linq;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus
{
    public class LicenseTenantStatusUsedBundle
    {
        public readonly Dictionary<string, LicenseTenantStatusApp> OwnedApps;

        public LicenseTenantStatusUsedBundle(LicensedBundleDetails ownedBundle, IEnumerable<LicensedAppDetails> ownedApps)
        {
            BundleIdentifier = ownedBundle.Identifier;
            BundleName = ownedBundle.Name;
            BundleLicenseCount = ownedBundle.NumberOfLicenses >= 0 ? ownedBundle.NumberOfLicenses : 0;
            BundleConsumedLicenseCount = 0;
            OwnedApps = ownedApps.ToDictionary(details => details.Identifier,
                details => new LicenseTenantStatusApp(details.NumberOfLicenses, details.AdditionalNumberOfLicenses, details.Identifier,
                    details.Name, details.Status, details.SoftwareName, details.SoftwareIdentifier), 
                StringComparer.OrdinalIgnoreCase);
        }

        private LicenseTenantStatusUsedBundle(LicenseTenantStatusUsedBundle licenseTenantStatusUsedBundle, Dictionary<string, LicenseTenantStatusApp> ownedApps)
        {
            BundleIdentifier = licenseTenantStatusUsedBundle.BundleIdentifier;
            BundleLicenseCount = licenseTenantStatusUsedBundle.BundleLicenseCount;
            BundleConsumedLicenseCount = licenseTenantStatusUsedBundle.BundleConsumedLicenseCount;
            BundleName = licenseTenantStatusUsedBundle.BundleName;
            OwnedApps = ownedApps;
        }

        public string BundleIdentifier { get; }
        
        public string BundleName { get; }
        
        public int BundleLicenseCount { get; }
        
        public int BundleConsumedLicenseCount { get; private set; }

        public bool OwnsApp(string appIdentifier) => OwnedApps.ContainsKey(appIdentifier);
        
        private bool HasAvailableLicense() => BundleLicenseCount - BundleConsumedLicenseCount > 0;

        public TryConsumeLicenseOutput TryConsumeLicense(ConsumeLicenseInput input, LicenseConsumeType licenseConsumeType)
        {
            
            var app = OwnedApps[input.AppIdentifier];
            TryConsumeLicenseOutput tryConsumeLicenseOutput;

            if (app.HasAdditionalConsumedLicense(input.User))
            {
                tryConsumeLicenseOutput = app.TryConsumeAdditionalLicense(input, licenseConsumeType);
                return tryConsumeLicenseOutput;
            }
            
            var userAlreadyConsumingBundleLicense = OwnedApps.Values.Any(s => s.UserAlreadyConsumingLicense(input.User));
            if (userAlreadyConsumingBundleLicense && licenseConsumeType == LicenseConsumeType.Connection)
            {
                tryConsumeLicenseOutput = app.TryConsumeLicense(input, true, licenseConsumeType);
            }
            else if (HasAvailableLicense())
            {
                tryConsumeLicenseOutput = app.TryConsumeLicense(input, true, licenseConsumeType);
                if (tryConsumeLicenseOutput.ConsumeAppLicenseStatus == ConsumeAppLicenseStatusOld.LicenseConsumed)
                    BundleConsumedLicenseCount++;
            }
            else
                tryConsumeLicenseOutput = app.TryConsumeAdditionalLicense(input, licenseConsumeType);

            return tryConsumeLicenseOutput;
        }

        public TryReleaseLicenseOutput TryReleaseLicense(string appIdentifier, string user, LicenseConsumeType licenseConsumeType)
        {
            var app = OwnedApps[appIdentifier];

            var tryReleaseLicense = app.HasAdditionalConsumedLicense(user) 
                ? app.TryReleaseAdditionalLicense(user, licenseConsumeType) 
                : app.TryReleaseLicense(user, true, licenseConsumeType);

            if (tryReleaseLicense.ReleaseAppLicenseStatus == ReleaseAppLicenseStatusOld.LicenseReleased)
            {
                if (licenseConsumeType == LicenseConsumeType.Access)
                {
                    BundleConsumedLicenseCount--;
                }
                else
                {
                    var userStillUsingAppInCurrentBundle = OwnedApps.Values.Any(s => s.UserAlreadyConsumingLicense(user));
                    if (!userStillUsingAppInCurrentBundle)
                        BundleConsumedLicenseCount--; 
                }
            }

            return tryReleaseLicense;
        }

        public int GetAppAvailableAdditionalLicense(string appIdentifier)
        {
            var app = OwnedApps[appIdentifier];
            return app.AdditionalLicenses - app.AdditionalLicensesConsumed;
        }

        public LicenseTenantStatusUsedBundle Clone()
        {
           var clonedOwnedApps = OwnedApps.ToDictionary(o => o.Key, o => o.Value.Clone(), StringComparer.OrdinalIgnoreCase);
           var licenseTenantStatusUsedBundle = new LicenseTenantStatusUsedBundle(this, clonedOwnedApps);
           return licenseTenantStatusUsedBundle;
        }

        public bool TryRestoreLicenseInUse(AppLicenseConsumer licenseInUse, LicenseConsumeType licenseConsumeType)
        {
            var app = OwnedApps[licenseInUse.AppIdentifier];

            if (licenseInUse.AdditionalLicense)
                return app.TryRestoreLicenseInUse(licenseInUse, true);

            if (HasAvailableLicense() && app.TryRestoreLicenseInUse(licenseInUse, true))
            {
                if (licenseConsumeType == LicenseConsumeType.Access)
                {
                    BundleConsumedLicenseCount++;
                    return true;
                }
                var userAlreadyConsumingBundleLicense = OwnedApps.Values.Any(s => s.UserAlreadyConsumingLicense(licenseInUse.User));
                if (!userAlreadyConsumingBundleLicense)
                    BundleConsumedLicenseCount++;
                return true;
            }
            
            return false;
        }
    }
}