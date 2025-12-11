using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageInRealTime;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageInRealTime;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    public class LicenseUsageInRealTimeImportController: BaseController
    {
        private readonly ILicenseUsageInRealTimeService _licenseUsageInRealTimeService;

        public LicenseUsageInRealTimeImportController(ILicenseUsageInRealTimeService licenseUsageInRealTimeService)
        {
            _licenseUsageInRealTimeService = licenseUsageInRealTimeService;
        }

        [HttpPost("/licensing/customer-licensing/real-time")]
        [TenantNotRequired]
        [UserNotRequired]
        public async Task<ActionResult> Import([FromBody] LicenseUsageInRealTimeImportInput input)
        {
            await _licenseUsageInRealTimeService.ImportLicenseUsage(input);
            return Ok();
        }
    }
}