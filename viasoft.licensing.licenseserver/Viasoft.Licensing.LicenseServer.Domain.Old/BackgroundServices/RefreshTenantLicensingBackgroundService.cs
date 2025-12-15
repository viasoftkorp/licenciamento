using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicensedTenantOrchestrator;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.BackgroundServices
{
    public class RefreshTenantLicensingBackgroundService: BackgroundService
    {
        private readonly ILicensedTenantOrchestratorService _licensedTenantOrchestratorService;
        private readonly ILogger<RefreshTenantLicensingBackgroundService> _logger;

        public RefreshTenantLicensingBackgroundService(ILicensedTenantOrchestratorService licensedTenantOrchestratorService, ILogger<RefreshTenantLicensingBackgroundService> logger)
        {
            _licensedTenantOrchestratorService = licensedTenantOrchestratorService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (DefaultConfigurationConsts.IsRunningAsLegacy)
                await DoWork(stoppingToken);
        }
        
        private async Task DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _licensedTenantOrchestratorService.RefreshAllTenantsLicensing();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "[LEGACY] - Could not refresh all tenants licensing");
                }
                await Task.Delay(GetTaskDelayToNextExecution(), cancellationToken);
            }
        }
        
        private static int GetTaskDelayToNextExecution()
        {
            // This task must be executed at 05:00 a.m. UTC
            var now = DateTime.UtcNow;
            var nextDay = now.AddDays(1);
            var nextExecutionDateTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 5, 0, 0, DateTimeKind.Utc);
            var timeDifference = nextExecutionDateTime - now;
            return Convert.ToInt32(timeDifference.TotalMilliseconds);
        }

    }
}