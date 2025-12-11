using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageStatistics;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageStatistics;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    public class LicenseUsageStatisticsController : BaseController
    {
        private readonly ILicenseUsageStatisticsService _licenseUsageStatisticsService;

        public LicenseUsageStatisticsController(ILicenseUsageStatisticsService licenseUsageStatisticsService)
        {
            _licenseUsageStatisticsService = licenseUsageStatisticsService;
        }

        [HttpGet]
        public async Task<OnlineTenantCountOutput> GetOnlineTenantCount()
        {
            return await _licenseUsageStatisticsService.GetOnlineTenantCountAsync();
        }

        [HttpGet]
        public async Task<OnlineUserCountOutput> GetOnlineUserCount([FromQuery] OnlineUserCountInput input)
        {
            if (input.LicensingIdentifier.GetValueOrDefault() != Guid.Empty)
            {
                return await _licenseUsageStatisticsService.GetOnlineUserCountAsync(input.AdvancedFilter, input.LicensingIdentifier.GetValueOrDefault());
            }
            return await _licenseUsageStatisticsService.GetOnlineUserCountAsync(input.AdvancedFilter);
        }

        [HttpGet]
        public async Task<OnlineAppsCountOutput> GetOnlineAppsCount([FromQuery] OnlineAppsCountInput input)
        {
            if (input.LicensingIdentifier.GetValueOrDefault() != Guid.Empty)
            {
                return await _licenseUsageStatisticsService.GetAppsInUseCountAsync(input.AdvancedFilter,
                    input.LicensingIdentifier.GetValueOrDefault());
            }

            return await _licenseUsageStatisticsService.GetAppsInUseCountAsync(input.AdvancedFilter);
        }
        
        [HttpGet]
        public async Task<List<Guid>> GetLicenseIdentifiersForUsageReporting(string advancedFilter)
        {
            return await _licenseUsageStatisticsService.GetLicenseIdentifiersForUsageReporting(advancedFilter);
        }

        [HttpGet]
        public async Task<List<LicenseUsageReportingOutput>> GetLicenseUsageForReporting()
        {
            return await _licenseUsageStatisticsService.GetLicenseUsageForReporting();
        }
    }
}