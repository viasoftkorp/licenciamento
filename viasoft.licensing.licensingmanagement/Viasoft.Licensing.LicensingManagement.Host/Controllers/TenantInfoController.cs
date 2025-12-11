using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.TenantInfo;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantInfo;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class TenantInfoController : BaseController
    {
        private readonly ITenantInfoService _tenantInfoService;

        public TenantInfoController(ITenantInfoService tenantInfoService)
        {
            _tenantInfoService = tenantInfoService;
        }

        [HttpGet]
        [TenantIdParameterHint("identifier", ParameterLocation.Query)]
        public async Task<TenantInfoOutput> GetTenantInfoFromLicensingIdentifier([FromQuery] Guid identifier)
        {
            return await _tenantInfoService.GetTenantInfoFromLicensingIdentifier(identifier);
        }
    }
}