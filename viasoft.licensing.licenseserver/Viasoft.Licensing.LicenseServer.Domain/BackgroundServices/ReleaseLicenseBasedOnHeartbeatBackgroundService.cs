using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Viasoft.Licensing.LicenseServer.Domain.Catalogs;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Domain.BackgroundServices
{
    public class ReleaseLicenseBasedOnHeartbeatBackgroundService: BackgroundService
    {
        private readonly ITenantCatalog _tenantCatalog;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReleaseLicenseBasedOnHeartbeatBackgroundService> _logger;

        public ReleaseLicenseBasedOnHeartbeatBackgroundService(IConfiguration configuration, ITenantCatalog tenantCatalog, 
            ILogger<ReleaseLicenseBasedOnHeartbeatBackgroundService> logger)
        {
            _configuration = configuration;
            _tenantCatalog = tenantCatalog;
            _logger = logger;
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
                try
                {
                    await _tenantCatalog.ReleaseLicenseBasedOnHeartbeat();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Could release licensed based on heartbeat");
                }
                await Task.Delay(recurFrequencyInMilliseconds, cancellationToken);
            }
        }
    }
}