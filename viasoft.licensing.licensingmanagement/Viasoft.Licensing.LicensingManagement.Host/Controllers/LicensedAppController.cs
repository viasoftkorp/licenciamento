using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class LicensedAppController: BaseController
    {
        private readonly ILicensedTenantService _licensedTenantService;

        public LicensedAppController(ILicensedTenantService licensedTenantService)
        {
            _licensedTenantService = licensedTenantService;
        }

        [HttpGet("/licensing/licensing-management/licenses/{licensedTenant:guid}/licensed-apps/{licensedApp:guid}/named-user")]
        [TenantIdParameterHint("licensedTenant", ParameterLocation.Route, TenantIdParameterKind.LicensedTenantId)]
        public async Task<ActionResult<GetNamedUserFromLicensedAppOutput>> GetNamedUserFromLicensedApp([FromRoute] Guid licensedTenant, [FromRoute] Guid licensedApp, [FromQuery] GetAllNamedUserAppInput input)
        {
            var result = await _licensedTenantService.GetNamedUserFromLicensedApp(licensedTenant, licensedApp, input);

            switch (result.ValidationCode)
            {
                case GetNamedUserFromLicensedAppValidationCode.NoError:
                    return Ok(result);
                case GetNamedUserFromLicensedAppValidationCode.NoLicensedTenant:
                case GetNamedUserFromLicensedAppValidationCode.NoLicensedApp:
                    return NotFound(result);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [HttpPost("/licensing/licensing-management/licenses/{licensedTenant:guid}/licensed-apps/{licensedApp:guid}/named-user")]
        [TenantIdParameterHint("licensedTenant", ParameterLocation.Route, TenantIdParameterKind.LicensedTenantId)]
        public async Task<ActionResult<AddNamedUserToLicensedAppOutput>> AddNamedUserToLicensedApp([FromRoute] Guid licensedTenant,
            [FromRoute] Guid licensedApp, [FromBody] AddNamedUserToLicensedAppInput input)
        {
            var result = await _licensedTenantService.AddNamedUserToLicensedApp(licensedTenant, licensedApp, input);
            
            switch (result.ValidationCode)
            {
                case AddNamedUserToLicensedAppValidationCode.NoError:
                    return Created($"/licensing/licensing-management/licenses/{licensedTenant}/licensed-aps/{licensedApp}/named-user", result.Output);
                case AddNamedUserToLicensedAppValidationCode.NoLicensedTenant:
                case AddNamedUserToLicensedAppValidationCode.NoLicensedApp:
                    return NotFound(result);
                case AddNamedUserToLicensedAppValidationCode.TooManyNamedUsers:
                case AddNamedUserToLicensedAppValidationCode.NamedUserEmailAlreadyInUse:    
                case AddNamedUserToLicensedAppValidationCode.LicensedAppIsNotNamed:
                    return BadRequest(result);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [HttpPut("/licensing/licensing-management/licenses/{licensedTenant:guid}/licensed-apps/{licensedApp:guid}/named-user/{id:guid}")]
        [TenantIdParameterHint("licensedTenant", ParameterLocation.Route, TenantIdParameterKind.LicensedTenantId)]
        [UserNotRequired]
        public async Task<ActionResult<UpdateNamedUsersFromAppOutput>> UpdateNamedUsersFromApp([FromRoute] Guid licensedTenant,
            [FromRoute] Guid licensedApp, [FromBody] UpdateNamedUsersFromAppInput input, [FromRoute] Guid id)
        {
           var result = await _licensedTenantService.UpdateNamedUsersFromApp(licensedTenant, licensedApp, input, id);

           switch (result.ValidationCode)
            {
                case UpdateNamedUserAppLicenseValidationCode.NoError:
                    return Ok(result);
                case UpdateNamedUserAppLicenseValidationCode.NoLicensedTenant:
                case UpdateNamedUserAppLicenseValidationCode.NoLicensedApp:
                case UpdateNamedUserAppLicenseValidationCode.NoNamedUser:
                    return NotFound(result);
                case UpdateNamedUserAppLicenseValidationCode.NamedUserEmailAlreadyInUse:
                    return BadRequest(result);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [HttpDelete("/licensing/licensing-management/licenses/{licensedTenant:guid}/licensed-apps/{licensedApp:guid}/named-user/{id:guid}")]
        [TenantIdParameterHint("licensedTenant", ParameterLocation.Route, TenantIdParameterKind.LicensedTenantId)]
        public async Task<ActionResult<DeleteNamedUsersFromAppOutput>> DeleteNamedUsersFromApp([FromRoute] Guid licensedTenant, [FromRoute] Guid licensedApp, [FromRoute] Guid id)
        {
            var result = await _licensedTenantService.DeleteNamedUsersFromApp(licensedTenant, licensedApp, id);

            switch (result.ValidationCode)
            {
                case DeleteNamedUsersFromAppValidationCode.NoError:
                    return Ok(result);
                case DeleteNamedUsersFromAppValidationCode.NoLicensedTenant:
                case DeleteNamedUsersFromAppValidationCode.NoLicensedApp:
                case DeleteNamedUsersFromAppValidationCode.NoNamedUser:
                    return NotFound(result);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}