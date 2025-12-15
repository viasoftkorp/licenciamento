using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Viasoft.Licensing.LicenseServer.Domain.Catalogs;
using Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models;
using Viasoft.Licensing.LicenseServer.Domain.Repositories;
using Viasoft.Licensing.LicenseServer.Domain.Services.DataUploader;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Domain.BackgroundServices
{
    public class LicenseUsageInRealTimeBackgroundService: BackgroundService
    {
        private readonly ITenantCatalog _tenantCatalog;
        private readonly ILicenseServerRepository _licenseServerRepository;
        private readonly ILogger<LicenseUsageInRealTimeBackgroundService> _logger;
        private readonly IDataUploaderService _dataUploaderService;
        private readonly IConfiguration _configuration;

        public LicenseUsageInRealTimeBackgroundService(IDataUploaderService dataUploaderService,IConfiguration configuration, 
            ITenantCatalog tenantCatalog, ILogger<LicenseUsageInRealTimeBackgroundService> logger, ILicenseServerRepository licenseServerRepository)
        {
            _dataUploaderService = dataUploaderService;
            _configuration = configuration;
            _tenantCatalog = tenantCatalog;
            _logger = logger;
            _licenseServerRepository = licenseServerRepository;
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
                    var outputs = new List<LicenseUsageInRealTimeOutput>();
                    var tenantsLicensesUsageInRealTime = await _tenantCatalog.GetTenantsLicensesUsageInRealTime();
                    
                    _logger.LogInformation("Got {Count} tenants to report in real time", tenantsLicensesUsageInRealTime.Count);
                    
                    foreach (var license in tenantsLicensesUsageInRealTime)
                    {
                        var realTimeOutput = new LicenseUsageInRealTimeOutput(license);
                        outputs.Add(realTimeOutput);
                    }
                    
                    foreach (var newUploadData in outputs)
                    {
                        List<string> softwareIdentifierDifference = null;
                        var lastUploadedData = await _licenseServerRepository.GetLastUploadedLicenseUsageInRealTime(newUploadData.TenantId);

                        if (lastUploadedData != null)
                        {
                            var newUploadDataAsString  = JsonConvert.SerializeObject(newUploadData);
                            var lastUploadDataAsString = JsonConvert.SerializeObject(lastUploadedData);

                            // If two uploads are the exact same, there is no need to upload the new data.
                            if (string.Equals(newUploadDataAsString, lastUploadDataAsString, StringComparison.OrdinalIgnoreCase))
                            {
                                _logger.LogInformation("Skipping license usage in real time report because no new licenses were consumed, tenant {TenantId}", newUploadData.TenantId);
                                continue;
                            }

                            softwareIdentifierDifference = lastUploadedData.SoftwareUtilized?.Except(newUploadData.SoftwareUtilized).ToList();
                            if (softwareIdentifierDifference == null || softwareIdentifierDifference.Any())
                            {
                                // Here we must store the newUploadData only with the SoftwareUtilized it already has.
                                await _licenseServerRepository.StoreLastUploadedLicenseUsageInRealTime(newUploadData);
                                // But the upload must have the softwareIdentifierDifference too,
                                // as it needs to upload the previously used Software, in order to correctly show the licenses usage in real time
                                if (softwareIdentifierDifference != null)
                                    newUploadData.SoftwareUtilized.AddRange(softwareIdentifierDifference);
                            }
                        }

                        await _dataUploaderService.UploadLicenseUsageInRealTime(newUploadData);
                        _logger.LogInformation("Reported license usage in real time for tenant {TenantId}", newUploadData.TenantId);
                        
                        // If the newUploadData has been stored, we must not store it again.
                        if (softwareIdentifierDifference == null || !softwareIdentifierDifference.Any())
                            await _licenseServerRepository.StoreLastUploadedLicenseUsageInRealTime(newUploadData);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Could not upload license usage in real time");
                }
                await Task.Delay(recurFrequencyInMilliseconds, cancellationToken);
            }
        }
    }
}