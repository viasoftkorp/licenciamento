using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Statistics;
using Viasoft.Licensing.LicensingManagement.Domain.Services.Statistics;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class LicensingManagementStatisticsController : BaseController
    {
        private readonly ILicenseManagementStatisticsService _licenseManagementStatisticsService;

        public LicensingManagementStatisticsController(ILicenseManagementStatisticsService licenseManagementStatisticsService)
        {
            _licenseManagementStatisticsService = licenseManagementStatisticsService;
        }

        [HttpGet]
        public async Task<NumberOfAppsTotalOutput> GetNumberOfAppsInTotal()
        {
            return await _licenseManagementStatisticsService.GetNumberOfAppsInTotal();
        }
    }
}