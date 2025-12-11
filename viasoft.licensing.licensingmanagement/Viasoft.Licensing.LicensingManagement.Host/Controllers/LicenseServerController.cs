using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicenseServer;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    [UserNotRequired]
    public class LicenseServerController : BaseController
    {
        private readonly ILicenseServerService _licenseServerService;

        public LicenseServerController(ILicenseServerService licenseServerService)
        {
            _licenseServerService = licenseServerService;
        }

        // license server. cannot enforce authorization for now
        [HttpGet]
        [AllowAnonymous]
        [TenantIdParameterHint("tenantId", ParameterLocation.Query)]
        public async Task<LicenseByIdentifier> GetLicenseByTenantId(Guid tenantId)
        {
            return await _licenseServerService.GetLicenseByIdentifier(tenantId);
        }
    }
}