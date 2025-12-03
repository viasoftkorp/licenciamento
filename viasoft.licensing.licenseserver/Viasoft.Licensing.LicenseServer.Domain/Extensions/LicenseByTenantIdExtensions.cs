using System;
using System.Collections.Generic;
using System.Linq;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicenseServer.Domain.Services.LicenseServer;

namespace Viasoft.Licensing.LicenseServer.Domain.Extensions
{
    public static class LicenseByTenantIdExtensions
    {
        public static Dictionary<string, LicenseTenantStatusApp> GetLooseAppsByIdentifier(this LicenseByTenantId tenantDetails, IExternalLicensingManagementService externalLicensingManagementService)
        {
            return tenantDetails.LicensedTenantDetails.OwnedApps.Where(details => !details.LicensedBundleId.HasValue)
                .ToDictionary(
                    details => details.Identifier,
                    details =>
                    {
                        var namedUserAppLicenses = tenantDetails.LicensedTenantDetails.NamedUserAppLicenses.Where(a => a.LicensedAppId == details.LicensedAppId)
                            .ToList();
                        
                        return new LicenseTenantStatusApp(details.NumberOfLicenses,  details.Identifier, details.Name, details.Status, details.SoftwareName, 
                            details.SoftwareIdentifier, namedUserAppLicenses, details.LicensingModel, details.LicensingMode, new List<NamedUserBundleLicenseOutput>(), 
                            externalLicensingManagementService);
                    }, 
                    StringComparer.OrdinalIgnoreCase);
        }

        public static Dictionary<string, LicenseTenantStatusUsedBundle> GetBundleByIdentifier(this LicenseByTenantId tenantDetails, IExternalLicensingManagementService externalLicensingManagementService)
        {
            return tenantDetails.LicensedTenantDetails.OwnedBundles.ToDictionary(
                details => details.Identifier,
                details =>
                {
                    var appsInBundle = tenantDetails.LicensedTenantDetails.OwnedApps
                        .Where(appDetails => appDetails.LicensedBundleId.HasValue && appDetails.LicensedBundleId == details.BundleId)
                        .ToList();
                    var namedUserBundle = tenantDetails.LicensedTenantDetails.NamedUserBundleLicenses
                        .Where(b => b.LicensedBundleId == details.LicensedBundleId)
                        .ToList();
                    return new LicenseTenantStatusUsedBundle(details, appsInBundle, namedUserBundle, externalLicensingManagementService);
                }, 
                StringComparer.OrdinalIgnoreCase);
        }
    }
}