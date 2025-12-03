using System;
using Microsoft.Extensions.Configuration;
using Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration;

namespace Viasoft.Licensing.LicenseServer.Domain.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool SkipAutoUpdate(this IConfiguration configuration)
        {
            var skipAutoUpdate = configuration[$"{nameof(LicenseServerSettings.AutoUpdateSettings)}:{nameof(LicenseServerSettings.AutoUpdateSettings.SkipAutoUpdate)}"];
            return bool.TryParse(skipAutoUpdate, out var parsed) && parsed;
        }
        
        public static int GetAutoUpdateTimeHour(this IConfiguration configuration)
        {
            var updateTimeHourString = configuration[$"{nameof(LicenseServerSettings.AutoUpdateSettings)}:{nameof(LicenseServerSettings.AutoUpdateSettings.UpdateTime)}"];
            var isDateTime = DateTime.TryParse(updateTimeHourString, out var dateTime);
            if (isDateTime)
            {
                return dateTime.Hour;
            }

            return 2;
        }

        public static string GetAutoUpdateAppCastUrl(this IConfiguration configuration)
        {
            var appCastUrl = configuration[$"{nameof(LicenseServerSettings.AutoUpdateSettings)}:{nameof(LicenseServerSettings.AutoUpdateSettings.AppCastUrl)}"];

            return appCastUrl;
        }

        public static string GetAutoUpdatePublicKey(this IConfiguration configuration)
        {
            var publicKey = configuration[$"{nameof(LicenseServerSettings.AutoUpdateSettings)}:{nameof(LicenseServerSettings.AutoUpdateSettings.PublicKey)}"];

            return publicKey;
        }

        public static bool GetForceLegacyDatabaseEngine(this IConfiguration configuration) => bool.TryParse(configuration["ForceLegacyDatabaseEngine"], out var result) && result;
    }
}