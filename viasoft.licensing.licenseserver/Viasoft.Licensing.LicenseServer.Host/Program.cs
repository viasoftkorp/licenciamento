using System;
using System.Threading.Tasks;
using Viasoft.Core.WebHost;
using Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Host
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {            
            LicenseServerSettingsExtension.LoadSettings();

            if (DefaultConfigurationConsts.IsRunningAsLegacy)
            {
                Console.WriteLine($"We are running in legacy mode WITHOUT broker");
                await LegacyStartup.LegacyMain<Startup>(args);   
            }
            else
            {
                Console.WriteLine(DefaultConfigurationConsts.IsRunningAsLegacyWithBroker
                    ? "We are running in legacy mode WITH broker"
                    : "We are NOT running in legacy mode");
                await ViasoftCoreWebHost.Main<Startup>(args);
            }
        }
    }
}