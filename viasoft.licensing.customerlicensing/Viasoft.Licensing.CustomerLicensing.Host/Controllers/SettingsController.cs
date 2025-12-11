using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.Settings;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    public class SettingsController : BaseController
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("/licensing/customer-licensing/settings")]
        [Authorize("Settings.Read")]
        public async Task<GetSettingsOutput> GetSettings([FromQuery] Guid tenantId)
        {
            var result = await _settingsService.GetSettings(tenantId);
            return result;
        }

        [HttpGet("/licensing/customer-licensing/settings/{version}")]
        [Authorize("Settings.Read")]
        public async Task<GetDeployCommandByVersionOutput> GetDeployCommandByVersion([FromRoute] string version)
        {
            return await _settingsService.GetDeployCommandByVersion(version);
        }

        [HttpGet("/licensing/customer-licensing/settings/{version}/update")]
        [Authorize("Settings.Read")]
        public async Task<GetUpdateVersionCommandByVersionOutput> GetDeployCommandByUpdateVersion([FromRoute] string version)
        {
            return await _settingsService.GetDeployCommandByUpdateVersion(version);
        }
        
        [HttpGet("/licensing/customer-licensing/settings/{version}/uninstall")]
        [Authorize("Settings.Read")]
        public async Task<GetUninstallCommandByVersionOutput> GetDeployCommandByUninstallVersion([FromRoute] string version)
        {
            return await _settingsService.GetDeployCommandByUninstallVersion(version);
        }

        [HttpPut("/licensing/customer-licensing/settings")]
        [Authorize("Settings.Update")]
        public async Task<ActionResult<InfrastructureConfigurationUpdateOutput>> PutSettings(
            [FromBody] InfrastructureConfigurationUpdateInput input)
        {
            var result = await _settingsService.PutSettings(input);
            return Ok(result);
        }
    }
}