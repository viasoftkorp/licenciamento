using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class LicensedTenantSettingsController : BaseController
    {
        private readonly IRepository<LicensedTenantSettings> _licensedTenantSettings;

        public LicensedTenantSettingsController(IRepository<LicensedTenantSettings> licensedTenantSettings)
        {
            _licensedTenantSettings = licensedTenantSettings;
        }

        [HttpGet("/licensing/licensing-management/licenses/{identifier:guid}/license-server-settings")]
        public async Task<LicensedTenantSettingsOutput> Get([FromRoute] Guid identifier)
        {
            var licensedTenantSettings = await _licensedTenantSettings
                .AsNoTracking()
                .FirstAsync(licensedTenantSettings => 
                    licensedTenantSettings.LicensingIdentifier == identifier &&
                    licensedTenantSettings.Key == LicensedTenantSettingsKeys.UseSimpleHardwareIdKey
                );

            return new LicensedTenantSettingsOutput(licensedTenantSettings);
        }
        
        [HttpPost("/licensing/licensing-management/licenses/{identifier:guid}/license-server-settings")]
        public async Task<LicensedTenantSettingsOutput> Update([FromRoute] Guid identifier, [FromBody] LicensedTenantSettingsInput input)
        {
            var licensedTenantSettings = await _licensedTenantSettings
                .FirstAsync(licensedTenantSettings => 
                    licensedTenantSettings.LicensingIdentifier == identifier &&
                    licensedTenantSettings.Key == LicensedTenantSettingsKeys.UseSimpleHardwareIdKey
                );
            
            licensedTenantSettings.UpdateValue(input.UseSimpleHardwareId);

            await _licensedTenantSettings.UpdateAsync(licensedTenantSettings, true);

            return new LicensedTenantSettingsOutput(licensedTenantSettings);
        }
    }
}