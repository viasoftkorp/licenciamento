using System;
using Viasoft.Licensing.LicenseServer.Domain.Abstractions.NamedUserLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense;

namespace Viasoft.Licensing.LicenseServer.Domain.Extensions
{
    public static class NamedUserExtensions
    {
        //this is a code smell, LicenseTenantStatusApp should be refactored to use chain of responsibility
        public static bool IsNamedUserAppLicense(this INamedUserLicense userNamedLicense, out Guid appId)
        {
            appId = Guid.Empty;
            if (userNamedLicense is NamedUserAppLicenseOutput namedUserAppLicense)
            {
                appId = namedUserAppLicense.LicensedAppId;
                return true;
            }

            return false;
        }

        public static bool IsNamedUserBundleLicense(this INamedUserLicense userNamedLicense, out Guid bundleId)
        {
            bundleId = Guid.Empty;
            if (userNamedLicense is NamedUserBundleLicenseOutput namedUserBundleLicense)
            {
                bundleId = namedUserBundleLicense.LicensedBundleId;
                return true;
            }

            return false;
        }
    }
}