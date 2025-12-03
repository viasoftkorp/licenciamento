using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Viasoft.Licensing.LicenseServer.Shared.Consul;

namespace Viasoft.Licensing.LicenseServer.Host.HostedServices
{
    public class ConsulProvider: IHostedService
    {
        private readonly IConsulSettingsProvider _consulSettingsProvider;

        public ConsulProvider(IConsulSettingsProvider consulSettingsProvider)
        {
            _consulSettingsProvider = consulSettingsProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consulSettingsProvider.LoadSettingsFromConsul();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}