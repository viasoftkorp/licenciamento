using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.DataUploader;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicensedTenantOrchestrator;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.BackgroundServices
{
    public class LicenseUsageInRealTimeBackgroundService: BackgroundService
    {
        private readonly ILicensedTenantOrchestratorService _licensedTenantOrchestratorService;
        private readonly IDataUploaderService _dataUploaderService;
        private readonly ILicenseUsageService _licenseUsageService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LicenseUsageInRealTimeBackgroundService> _logger;

        public LicenseUsageInRealTimeBackgroundService(ILicensedTenantOrchestratorService licensedTenantOrchestratorService, 
            IDataUploaderService dataUploaderService, ILicenseUsageService licenseUsageService, IConfiguration configuration, ILogger<LicenseUsageInRealTimeBackgroundService> logger)
        {
            _licensedTenantOrchestratorService = licensedTenantOrchestratorService;
            _dataUploaderService = dataUploaderService;
            _licenseUsageService = licenseUsageService;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var uploadFrequencyInMinutes = _configuration[EnvironmentVariableConsts.LicenseUsageInRealTimeUploadFrequencyInMinutes];
            var uploadFrequency = !string.IsNullOrEmpty(uploadFrequencyInMinutes) 
                ? TimeSpan.FromMinutes(Convert.ToInt32(uploadFrequencyInMinutes))
                : TimeSpan.FromMinutes(DefaultConfigurationConsts.LicenseUsageInRealTimeUploadFrequencyInMinutes);
            
            await DoWork(stoppingToken, Convert.ToInt32(uploadFrequency.TotalMilliseconds));
        }
        
        private async Task DoWork(CancellationToken cancellationToken, int recurFrequencyInMilliseconds)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var tenantsLicensesUsageInRealTime = _licensedTenantOrchestratorService.GetTenantsLicensesUsageInRealTime()
                        .ToList();
                    
                    _logger.LogInformation("[LEGACY] - Got {Count} tenants to report in real time", tenantsLicensesUsageInRealTime.Count);
                    
                    foreach (var newUploadData in tenantsLicensesUsageInRealTime)
                    {
                        List<string> softwareIdentifierDifference = null;
                        var lastUploadedData = _licenseUsageService.GetLastUploadedLicenseUsageInRealTime(newUploadData.TenantId);

                        if (lastUploadedData != null)
                        {
                            var newUploadDataAsString  = JsonConvert.SerializeObject(newUploadData);
                            var lastUploadDataAsString = JsonConvert.SerializeObject(lastUploadedData);
                            // If two uploads are the exact same, there is no need to upload the new data.
                            if (string.Equals(newUploadDataAsString, lastUploadDataAsString, StringComparison.OrdinalIgnoreCase))
                            {
                                _logger.LogInformation("[LEGACY] - Skipping license usage in real time report because no new licenses were consumed, tenant {TenantId}", newUploadData.TenantId);
                                continue;
                            }

                            softwareIdentifierDifference = lastUploadedData.SoftwareUtilized.Except(newUploadData.SoftwareUtilized).ToList();
                            if (softwareIdentifierDifference.Any())
                            {
                                // Here we must store the newUploadData only with the SoftwareUtilized it already has.
                                _licenseUsageService.StoreLastUploadedLicenseUsageInRealTime(newUploadData);
                                // But the upload must have the softwareIdentifierDifference too,
                                // as it needs to upload the previously used Software, in order to correctly show the licenses usage in real time
                                newUploadData.SoftwareUtilized.AddRange(softwareIdentifierDifference);
                            }
                        }
                        
                        await _dataUploaderService.UploadLicenseUsageInRealTime(newUploadData);
                        _logger.LogInformation("[LEGACY] - Reported license usage in real time for tenant {TenantId}", newUploadData.TenantId);
                        
                        // If the newUploadData has been stored, we must not store it again.
                        if (softwareIdentifierDifference == null || !softwareIdentifierDifference.Any())
                            _licenseUsageService.StoreLastUploadedLicenseUsageInRealTime(newUploadData);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "[LEGACY] - Could not upload license usage in real time");
                }
                await Task.Delay(recurFrequencyInMilliseconds, cancellationToken);
            }
        }
    }
}