using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageInRealTime;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.TenantInfo;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageInRealTime;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.TenantInfo;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    public class LicenseUsageInRealTimeController: BaseController
    {
        private readonly ILicenseUsageInRealTimeService _licenseUsageInRealTimeService;
        private readonly ITenantInfoService _tenantInfoService;

        public LicenseUsageInRealTimeController(ILicenseUsageInRealTimeService licenseUsageInRealTimeService, ITenantInfoService tenantInfoService)
        {
            _licenseUsageInRealTimeService = licenseUsageInRealTimeService;
            _tenantInfoService = tenantInfoService;
        }

        // license server. cannot enforce authorization for now
        [HttpPost]
        [AllowAnonymous]
        [TenantNotRequired]
        [UserNotRequired]
        public async Task<bool> Import(LicenseUsageInRealTimeImportInput input)
        {
            await _licenseUsageInRealTimeService.ImportLicenseUsage(input);
            return true;
        }
        
        [HttpGet]
        public Task<TenantInfoOutput> GetTenantInfoFromId([FromQuery] Guid tenantId)
        {
            return _tenantInfoService.GetTenantInfoFromId(tenantId);
        }
    }
}