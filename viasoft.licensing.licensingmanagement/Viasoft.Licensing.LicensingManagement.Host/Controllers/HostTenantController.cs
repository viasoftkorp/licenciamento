using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.HostTenantId;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensingIdentifierToHost;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class HostTenantController: BaseController
    {
        private readonly ILicensingIdentifierToHostCacheService _licensingIdentifierToHostCacheService;

        public HostTenantController( ILicensingIdentifierToHostCacheService licensingIdentifierToHostCacheService)
        {
            _licensingIdentifierToHostCacheService = licensingIdentifierToHostCacheService;
        }

        [HttpGet]
        [TenantIdParameterHint("licensingIdentifier", ParameterLocation.Query)]
        public async Task<HostTenantIdOutput> GetHostTenantIdFromLicensingIdentifier(Guid licensingIdentifier)
        {
            return await _licensingIdentifierToHostCacheService.GetHostTenantIdFromLicensingIdentifier(licensingIdentifier, TenantIdParameterKind.LicensingIdentifier);
        }
    }
}