using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Viasoft.Licensing.LicenseServer.Shared.Attributes;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration
{
    public class LicenseServerSettings
    {
        public int HttpPort { get; set; }
        public int? LicenseUsageBehaviourUploadFrequencyInDays { get; set; }
        public int? LicenseUsageInRealTimeUploadFrequencyInMinutes { get; set; }
        public int? MinimumAllowedHeartbeatInSeconds { get; set; }
        public List<TenantLegacyDatabaseMappingConfiguration> TenantLegacyDatabaseMapping { get; set; }
        public string UrlGateway { get; set; }
        public string AspNetCoreEnvironment { get; set; }
        public string Cors { get; set; }
        public AutoUpdateSettings AutoUpdateSettings { get; set; } = new();
        public string CustomServiceNameUpdater { get; set; }
        public string CustomerLicensingSecret {get;set;}
        public string LicensingManagementSecret {get;set;}
    }

    public static class LicenseServerSettingsExtension
    {
        private static bool _firstTime = true;

        private static readonly Lazy<LicenseServerSettings> LazyLicenseServerSettings = new(InitLazyLicenseServerSettings);

        public static LicenseServerSettings LoadSettings()
        {
            var settings = LazyLicenseServerSettings.Value;
            
            if (!_firstTime) 
                return settings;

            if (DefaultConfigurationConsts.IsRunningAsLegacyWithBroker || settings == null) // Ou seja, não tem arquivo de configuração...
            {
                _firstTime = false;
                return null;
            }

            settings.ValidateSettings();
            DefaultConfigurationConsts.IsRunningAsLegacy = true;
            SetEnvironmentVariable(EnvironmentVariableConsts.HttpPort, settings.HttpPort);
            SetEnvironmentVariable(EnvironmentVariableConsts.UrlGateway, settings.UrlGateway);
            _firstTime = false;

            return settings;
        }
        
        public static List<TenantLegacyDatabaseMappingConfiguration> GetTenantLegacyDatabases()
        {
            var settings = LazyLicenseServerSettings.Value;
            return settings.TenantLegacyDatabaseMapping;
        }

        private static void SetEnvironmentVariable(string key, object value)
        {
            if (value == null) 
                return;
            Console.WriteLine($"{key}={value}");
            Environment.SetEnvironmentVariable(key, value.ToString());
        }

        private static LicenseServerSettings InitLazyLicenseServerSettings()
        {
            var withBroker = Environment.GetEnvironmentVariable(EnvironmentVariableConsts.LegacyWithBroker);
            DefaultConfigurationConsts.IsRunningAsLegacyWithBroker = !string.IsNullOrEmpty(withBroker) && withBroker.ToUpper().Equals("TRUE");
            
            return DefaultConfigurationConsts.IsRunningAsLegacyWithBroker ? null : GetSettingsFromFile();
        }
        

        private static void ValidateSettings(this LicenseServerSettings settings)
        {
            if (settings == null)
                return;

            if (!settings.AutoUpdateSettings.SkipAutoUpdate)
            {
                if (string.IsNullOrEmpty(settings.AutoUpdateSettings.PublicKey) || string.IsNullOrEmpty(settings.AutoUpdateSettings.AppCastUrl))
                {
                    throw new Exception($"The AutoUpdateSettings object must have the following properties {nameof(settings.AutoUpdateSettings.PublicKey)} and {nameof(settings.AutoUpdateSettings.AppCastUrl)}) filled out while building and publishing the project");
                }
            }

            if (settings.TenantLegacyDatabaseMapping == null || settings.TenantLegacyDatabaseMapping.Count == 0)
                throw new Exception($"Tenant/Database mapping must be added to {nameof(LicenseServerSettings)}.json.");
            
            if (settings.MinimumAllowedHeartbeatInSeconds.HasValue && settings.MinimumAllowedHeartbeatInSeconds.Value <= 0 || 
                settings.LicenseUsageBehaviourUploadFrequencyInDays.HasValue && settings.LicenseUsageBehaviourUploadFrequencyInDays.Value <= 0 ||
                settings.LicenseUsageInRealTimeUploadFrequencyInMinutes.HasValue && settings.LicenseUsageInRealTimeUploadFrequencyInMinutes.Value <= 0)
                throw new Exception($"Settings on {nameof(LicenseServerSettings)}.json must not have zero or negative values.");
            
            if (string.IsNullOrEmpty(settings.UrlGateway))
                throw new Exception($"Settings on {nameof(LicenseServerSettings)}.json must have a valid UrlGateway property. Contact the administrator.");

            var aspNetCoreEnvironment = settings.AspNetCoreEnvironment ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.IsNullOrEmpty(aspNetCoreEnvironment))
            {
                settings.AspNetCoreEnvironment = char.ToUpperInvariant(aspNetCoreEnvironment[0]) + aspNetCoreEnvironment.Substring(1).ToLower();
                var environments = new [] {"Production", "Staging", "Development"};
                if (!environments.Contains(settings.AspNetCoreEnvironment))
                {
                    var acceptedEnvironments = environments.Aggregate((a, b) => $"{a}, {b}");
                    throw new ArgumentException($"{nameof(settings.AspNetCoreEnvironment)} '{settings.AspNetCoreEnvironment}' not within accepted values: '{acceptedEnvironments}'.", nameof(settings.AspNetCoreEnvironment));
                }
            }

            if (string.IsNullOrEmpty(settings.Cors))
            {
                Console.WriteLine("No CORS was specified, using default.");
                settings.Cors = "https://portal.korp.com.br";
            }
        }

        private static LicenseServerSettings GetSettingsFromFile()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{nameof(LicenseServerSettings)}.json");
            if (!File.Exists(filePath)) 
                return null;
            
            var fileAsText = File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<LicenseServerSettings>(fileAsText);

            if (result != null)
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly is null)
                {
                    throw new InvalidOperationException();
                }
                
                var customServiceName = entryAssembly.GetCustomAttribute<CustomServiceNameUpdaterAttribute>()?.CustomServiceNameUpdater;
                result.CustomServiceNameUpdater = customServiceName;

                var appCastUrl = entryAssembly.GetCustomAttribute<AppCastUrlAttribute>()?.AppCastUrl;
                var publicKey = entryAssembly.GetCustomAttribute<AutoUpdatePublicKeyAttribute>()?.AutoUpdatePublicKey;
                var skipAutoUpdate = entryAssembly.GetCustomAttribute<SkipAutoUpdateAttribute>()?.ParsedSkipAutoUpdate;
                var licensingManagementSecret = entryAssembly.GetCustomAttribute<LicensingManagementSecretAttribute>()?.LicensingManagementSecret;
                var customerLicensingSecret = entryAssembly.GetCustomAttribute<CustomerLicensingSecretAttribute>()?.CustomerLicensingSecret;

                result.AutoUpdateSettings.SkipAutoUpdate = skipAutoUpdate.GetValueOrDefault(false);
                result.AutoUpdateSettings.AppCastUrl = appCastUrl;
                result.AutoUpdateSettings.PublicKey = publicKey;
                
                if (string.IsNullOrEmpty(result.CustomerLicensingSecret))
                {
                    result.CustomerLicensingSecret = customerLicensingSecret;
                }
                if (string.IsNullOrEmpty(result.LicensingManagementSecret))
                {
                    result.LicensingManagementSecret = licensingManagementSecret;
                }
            }

            return result;
        }
    }
}