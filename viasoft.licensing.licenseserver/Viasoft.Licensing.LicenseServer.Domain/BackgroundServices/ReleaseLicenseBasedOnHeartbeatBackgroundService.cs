using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Viasoft.Licensing.LicenseServer.Domain.Catalogs;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Domain.BackgroundServices
{
    public class ReleaseLicenseBasedOnHeartbeatBackgroundService: BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReleaseLicenseBasedOnHeartbeatBackgroundService> _logger;

        public ReleaseLicenseBasedOnHeartbeatBackgroundService(IConfiguration configuration,
            ILogger<ReleaseLicenseBasedOnHeartbeatBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var minimumAllowedHeartbeatInSeconds = _configuration[EnvironmentVariableConsts.MinimumAllowedHeartbeatInSeconds];

            var minimumAllowedHeartbeat = string.IsNullOrEmpty(minimumAllowedHeartbeatInSeconds)
                ? TimeSpan.FromSeconds(DefaultConfigurationConsts.MinimumAllowedHeartbeatInSeconds)
                : TimeSpan.FromSeconds(Convert.ToDouble(minimumAllowedHeartbeatInSeconds));
            
            await DoWork(stoppingToken, Convert.ToInt32(minimumAllowedHeartbeat.TotalMilliseconds));
        }
        
        private async Task DoWork(CancellationToken cancellationToken, int recurFrequencyInMilliseconds)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await using (var scope = _serviceProvider.CreateAsyncScope())
                {   
                    try
                    {
                        var tenantCatalog = scope.ServiceProvider.GetRequiredService<ITenantCatalog>();
                        await tenantCatalog.ReleaseLicenseBasedOnHeartbeat();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Could release licensed based on heartbeat");
                    }
                }
                await Task.Delay(recurFrequencyInMilliseconds, cancellationToken);
            }
        }
    }
}