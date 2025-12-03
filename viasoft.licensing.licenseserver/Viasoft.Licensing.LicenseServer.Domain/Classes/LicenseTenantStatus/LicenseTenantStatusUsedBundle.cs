using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicenseServer.Domain.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Services.LicenseServer;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus
{
    public class LicenseTenantStatusUsedBundle
    {
        public readonly Dictionary<string, LicenseTenantStatusApp> OwnedApps;

        public LicenseTenantStatusUsedBundle(OwnedBundleDetails ownedBundle, IEnumerable<LicensedAppDetails> ownedApps, List<NamedUserBundleLicenseOutput> namedUserBundleLicenses, 
            IExternalLicensingManagementService externalLicensingManagementService)
        {
            BundleIdentifier = ownedBundle.Identifier;
            BundleName = ownedBundle.Name;
            BundleLicenseCount = ownedBundle.NumberOfLicenses >= 0 ? ownedBundle.NumberOfLicenses : 0;
            BundleConsumedLicenseCount = 0;
            OwnedApps = ownedApps.ToDictionary(details => details.Identifier,
                details => new LicenseTenantStatusApp(details.NumberOfLicenses, details.Identifier, details.Name, details.Status, details.SoftwareName, 
                    details.SoftwareIdentifier, new List<NamedUserAppLicenseOutput>(), ownedBundle.LicensingModel, ownedBundle.LicensingMode, namedUserBundleLicenses, 
                    externalLicensingManagementService), 
                StringComparer.OrdinalIgnoreCase);
        }

        public string BundleIdentifier { get; }
        
        public string BundleName { get; }
        
        public int BundleLicenseCount { get; }
        
        public int BundleConsumedLicenseCount { get; private set; }

        public bool OwnsApp(string appIdentifier) => OwnedApps.ContainsKey(appIdentifier);
        
        private bool HasAvailableLicense() => BundleLicenseCount - BundleConsumedLicenseCount > 0;

        public async Task<TryConsumeLicenseOutput> TryConsumeLicense(ConsumeLicenseInput input, LicenseConsumeType licenseConsumeType)
        {
            var app = OwnedApps[input.AppIdentifier];
            
            var tryConsumeLicenseOutput = new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.NotEnoughLicenses, app.AppName, app.SoftwareIdentifier, app.SoftwareName);

            var userAlreadyConsumingBundleLicense = OwnedApps.Values.Any(s => s.HasConsumedLicense(input.User, input.Token));
            if (userAlreadyConsumingBundleLicense && licenseConsumeType == LicenseConsumeType.Connection)
            {
                tryConsumeLicenseOutput = await app.TryConsumeLicense(input, true, licenseConsumeType);
            }
            else if (HasAvailableLicense())
            {
                tryConsumeLicenseOutput = await app.TryConsumeLicense(input, true, licenseConsumeType);
                if (tryConsumeLicenseOutput.ConsumeAppLicenseStatus == ConsumeAppLicenseStatus.LicenseConsumed)
                    BundleConsumedLicenseCount++;
            }

            return tryConsumeLicenseOutput;
        }

        public TryReleaseLicenseOutput TryReleaseLicense(string appIdentifier, string user, LicenseConsumeType licenseConsumeType, string sid, bool isTerminalServer)
        {
            var app = OwnedApps[appIdentifier];

            var tryReleaseLicense = app.TryReleaseLicense(user, true, licenseConsumeType, sid, isTerminalServer);

            if (tryReleaseLicense.ReleaseAppLicenseStatus == ReleaseAppLicenseStatus.LicenseReleased)
            {
                if (licenseConsumeType == LicenseConsumeType.Access)
                {
                    BundleConsumedLicenseCount--;
                }
                else
                {
                    var userStillUsingAppInCurrentBundle = OwnedApps.Values.Any(s => s.HasConsumedLicense(user, sid));
                    if (!userStillUsingAppInCurrentBundle)
                        BundleConsumedLicenseCount--; 
                }
            }

            return tryReleaseLicense;
        }
    }
}