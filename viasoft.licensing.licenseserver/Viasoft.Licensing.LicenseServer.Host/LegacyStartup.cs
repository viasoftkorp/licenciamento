using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Viasoft.Core.Environment;
using Viasoft.Core.Gateway;
using Viasoft.Core.WebHost;
using Viasoft.Core.WebHost.Logging;
using Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Host
{
    public static class LegacyStartup
    {
        public static async Task LegacyMain<TStartup>(string[] args) where TStartup : class
        {
            var assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine("Version {0}", assembly.GetName().Version);
            var settings = LicenseServerSettingsExtension.LoadSettings();

            args = new[] {$"http://*:{settings.HttpPort.ToString()}"};
            
            var host = CreateWebHostBuilder<TStartup>(args).Build();
            await host.RunAsync();
        }
        
        public static IHostBuilder CreateWebHostBuilder<TStartup>(string[] args) where TStartup : class
        {
            var config = GetConfiguration();
            var serviceConfiguration = ConventionBasedServiceConfigurationProvider.GetServiceConfigurationFrom<TStartup>();

            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureLogging(serviceConfiguration)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(config);
                    webBuilder.UseUrls(args);
                    webBuilder.ConfigureServices((context, collection) =>
                    {
                        if (!context.HostingEnvironment.IsDevelopment())
                        {
                            collection.AddSingleton<IHostAddressResolver, ProductionHostAddressResolver>();
                        }
                        else
                        {
                            collection.AddSingleton<IHostAddressResolver, DevelopmentHostAddressResolver>();
                        }
                        
                        collection.AddTransient<IGatewayUriProvider, GatewayUriProvider>();
                    });
                    webBuilder.UseStartup<TStartup>();
                });
        }
        
        private static string EnsureTrailingSlash(string path)
        {
            if (path.EndsWith("/"))
                return path;
            return path + "/";
        }
        
        private static IConfiguration GetConfiguration()
        {
            var loggerSettings = ReadLoggerConfigurations();
            var settings = LicenseServerSettingsExtension.LoadSettings();

            var data = new List<KeyValuePair<string, string>>
            {
                new("CORS", settings.Cors),
                new("Authentication:Enabled", "false"),
                new("Environment:GatewayAuthority", settings.UrlGateway),
                new("Authentication:IntrospectionSecret", "false"),
                new("Authentication:Authority", $"{EnsureTrailingSlash(settings.UrlGateway)}oauth"),
                new("LicensingManagementSecret", settings.LicensingManagementSecret),
                new("CustomerLicensingSecret", settings.CustomerLicensingSecret),
                new("CustomServiceNameUpdater", settings.CustomServiceNameUpdater),
                new("ASPNETCORE_ENVIRONMENT", $"{settings.AspNetCoreEnvironment}"),
                new("LICENSE_USAGE_IN_REAL_TIME_UPLOAD_FREQUENCY_IN_MINUTES", $"{settings.LicenseUsageInRealTimeUploadFrequencyInMinutes}"),
                new("LICENSE_USAGE_BEHAVIOUR_UPLOAD_FREQUENCY_IN_DAYS", $"{settings.LicenseUsageBehaviourUploadFrequencyInDays}"),
                new("MINIMUM_ALLOWED_HEARTBEAT_IN_SECONDS", $"{settings.MinimumAllowedHeartbeatInSeconds}"),
                new("TENANT_LEGACY_DATABASE_MAPPING_CONFIGURATION", JsonConvert.SerializeObject(settings.TenantLegacyDatabaseMapping)),
                new ($"{nameof(settings.AutoUpdateSettings)}:{nameof(settings.AutoUpdateSettings.UpdateTime)}", settings.AutoUpdateSettings.UpdateTime),
                new ($"{nameof(settings.AutoUpdateSettings)}:{nameof(settings.AutoUpdateSettings.PublicKey)}", settings.AutoUpdateSettings.PublicKey),
                new ($"{nameof(settings.AutoUpdateSettings)}:{nameof(settings.AutoUpdateSettings.AppCastUrl)}", settings.AutoUpdateSettings.AppCastUrl),
                new ($"{nameof(settings.AutoUpdateSettings)}:{nameof(settings.AutoUpdateSettings.SkipAutoUpdate)}", settings.AutoUpdateSettings.SkipAutoUpdate.ToString()),
                new (EnvironmentVariableConsts.LegacyWithBroker, DefaultConfigurationConsts.IsRunningAsLegacyWithBroker ? "True" : "False"),
                new("Environment:ConsulAuthority", ""),
                new("Environment:ServicePort", settings.HttpPort.ToString())
            };
            data.AddRange(loggerSettings);

            var source = new MemoryConfigurationSource { InitialData = data };

            var configuration = new ConfigurationBuilder()
                .Add(source)
                .Build();
            
            return configuration;
        }

        private static List<KeyValuePair<string, string>> ReadLoggerConfigurations()
        {
            try
            {
                var loggerSettings = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), $"{nameof(LicenseServerSettings)}.json"), optional: true)
                    .Build();
                
                var serilogSettingsSection = loggerSettings.GetSection("Serilog");
                return serilogSettingsSection.AsEnumerable()
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Problema ao ler seção do Serilog" + e);
                return new List<KeyValuePair<string, string>>();
            }
        }
    }
}