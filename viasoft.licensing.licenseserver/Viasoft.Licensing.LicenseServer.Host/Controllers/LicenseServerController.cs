using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicenseServer.Domain.Catalogs;
using Viasoft.Licensing.LicenseServer.Domain.Controllers.DTOs;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Enums;

namespace Viasoft.Licensing.LicenseServer.Host.Controllers
{
    public class LicenseServerController: BaseController
    {
        private readonly ITenantCatalog _tenantCatalog;
        private readonly ILogger<LicenseServerController> _logger;

        public LicenseServerController(ITenantCatalog tenantCatalog, ILogger<LicenseServerController> logger)
        {
            _tenantCatalog = tenantCatalog;
            _logger = logger;
        }

        [HttpGet("/licensing/license-server/licenses/{tenantId:guid}")]
        public async Task<ActionResult<GetLicensedTenantControllerOutput>> GetLicensedTenant([FromRoute] Guid tenantId)
        {
            var result = await _tenantCatalog.GetTenantCurrentLicenseStatus(tenantId);

            if (result == null)
            {
                _logger.LogWarning("Could not find/load licenses for tenant {TenantId}", tenantId);
                return NotFound();
            }

            var output = new GetLicensedTenantControllerOutput
            {
                TenantDetails = result.TenantDetails
            };

            return Ok(output);
        }
        
        [HttpPost("/licensing/license-server/licenses/{tenantId:guid}/consume")]
        public async Task<ActionResult<ConsumeLicenseOutput>> ConsumeLicense([FromBody] ConsumeLicenseControllerInput input, [FromRoute] Guid tenantId)
        {
            var consumeLicenseInput = new ConsumeLicenseInput
            {
                Cnpj = input.Cnpj,
                Token = input.Sid,
                User = input.User,
                AppIdentifier = input.AppIdentifier,
                TenantId = tenantId,
                CustomAppName = input.CustomAppName,
                LicenseUsageAdditionalInformation = input.LicenseUsageAdditionalInformation,
                DeviceId = input.DeviceId
            };
            var result = await _tenantCatalog.ConsumeLicense(consumeLicenseInput);
            
            if (result.ConsumeAppLicenseStatus is ConsumeAppLicenseStatus.LicenseConsumed or ConsumeAppLicenseStatus.LicenseAlreadyInUseByUser)
            {
                return Ok(result);
            }

            _logger.LogWarning("Consume not ok for user {User}, tenant {TenantId}, response {Response}",  input.User, tenantId, JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

            return BadRequest(result);
        }
        
        [HttpPost("/licensing/license-server/licenses/{tenantId:guid}/release")]
        public async Task<ActionResult<ReleaseLicenseOutput>> ReleaseLicense([FromBody] ReleaseLicenseControllerInput input, [FromRoute] Guid tenantId)
        {
            var releaseLicenseInput = new ReleaseLicenseInput
            {
                Cnpj = input.Cnpj,
                Token = input.Sid,
                User = input.User,
                AppIdentifier = input.AppIdentifier,
                TenantId = tenantId,
            };
            var result = await _tenantCatalog.ReleaseLicense(releaseLicenseInput);

            if (result.ReleaseAppLicenseStatus is ReleaseAppLicenseStatus.LicenseReleased or ReleaseAppLicenseStatus.LicenseStillInUseByUser)
            {
                return Ok(result);
            }
            
            _logger.LogWarning("Release not ok for user {User}, tenant {TenantId}, response {Response}", input.User, tenantId, JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

            return BadRequest(result);
        }
        
        [HttpPost("/licensing/license-server/licenses/{tenantId:guid}/refresh/{user}")]
        public async Task<ActionResult<RefreshAppLicenseInUseByUserOutput>> RefreshAppLicenseInUseByUser([FromBody] RefreshAppLicenseInUseByUserControllerInput input, 
            [FromRoute] Guid tenantId, [FromRoute] string user)
        {
            var refreshAppLicenseInUseByUserInput = new RefreshAppLicenseInUseByUserInput
            {
                Cnpj = input.Cnpj,
                Token = input.Sid,
                User = user,
                AppIdentifier = input.AppIdentifier,
                TenantId = tenantId,
                CustomAppName = input.CustomAppName,
                LicenseUsageAdditionalInformation = input.LicenseUsageAdditionalInformation,
                DeviceId = input.DeviceId
            };
            var result = await _tenantCatalog.RefreshAppLicenseInUseByUser(refreshAppLicenseInUseByUserInput);

            if (result.Status is RefreshAppLicenseInUseByUserStatus.RefreshSuccessful or RefreshAppLicenseInUseByUserStatus.RefreshSuccessfulLicenseConsumed)
            {
                return Ok(result);
            }
            
            _logger.LogWarning("Refresh not ok for user {User}, tenant {TenantId}, response {Response}", user, tenantId, JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

            return BadRequest(result);
        }
    }
}