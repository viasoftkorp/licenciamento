using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class LicensedBundleController: BaseController
    {
        private readonly ILicensedTenantService _licensedTenantService;
        private readonly ILicensedBundleService _licensedBundleService;

        public LicensedBundleController(ILicensedTenantService licensedTenantService, ILicensedBundleService licensedBundleService)
        {
            _licensedTenantService = licensedTenantService;
            _licensedBundleService = licensedBundleService;
        }

        [HttpGet("/licensing/licensing-management/licenses/{licensedTenant:guid}/licensed-bundles/{licensedBundle:guid}/named-user")]
        [TenantIdParameterHint("licensedTenant", ParameterLocation.Route, TenantIdParameterKind.LicensedTenantId)]
        public async Task<ActionResult<GetNamedUserFromBundleOutput>> GetNamedUserFromBundle([FromRoute] Guid licensedTenant, [FromRoute] Guid licensedBundle, [FromQuery] GetAllNamedUserBundleInput input)
        {
            var result = await _licensedTenantService.GetNamedUserFromBundle(licensedTenant, licensedBundle, input);

            switch (result.NamedUserFromBundleValidationCode)
            {
                case GetNamedUserFromBundleValidationCode.NoError:
                    return Ok(result);
                case GetNamedUserFromBundleValidationCode.NoLicensedTenant:
                case GetNamedUserFromBundleValidationCode.NoLicensedBundle:
                    return NotFound(result);
                default:
                    throw new InvalidEnumArgumentException($"Couldn't find a proper way to handle the following operationValidation value: {result.NamedUserFromBundleValidationCode} {result.NamedUserFromBundleValidationCode.ToString()}");
            }
        }

        [HttpPost("/licensing/licensing-management/licenses/{licensedTenant:guid}/licensed-bundles/{licensedBundle:guid}/named-user")]
        [TenantIdParameterHint("licensedTenant", ParameterLocation.Route, TenantIdParameterKind.LicensedTenantId)]
        public async Task<ActionResult> AddNamedUserToBundle([FromRoute] Guid licensedTenant,
            [FromRoute] Guid licensedBundle, [FromBody] CreateNamedUserBundleLicenseInput input)
        {
            
            var result = await _licensedTenantService.AddNamedUserToLicensedBundle(licensedTenant, licensedBundle, input);

            switch (result.OperationValidation)
            {
                case OperationValidation.NoTenantWithSuchId:
                case OperationValidation.NoLicensedBundleWithSuchId:
                    return NotFound(result);
                case OperationValidation.NoError:
                    return Created($"/licensing/licensing-management/licenses/{licensedTenant}/licensed-bundles/{licensedBundle}/named-user", result);
                case OperationValidation.TooManyNamedUserBundleLicenses:
                case OperationValidation.NamedUserEmailAlreadyInUse:    
                case OperationValidation.NotANamedLicense:
                    return BadRequest(result);
                default:
                    throw new InvalidEnumArgumentException($"Couldn't find a proper way to handle the following operationValidation value: {result.OperationValidation} {result.OperationValidation.ToString()}");
            }
        }

        [HttpPut("/licensing/licensing-management/licenses/{licensedTenant:guid}/licensed-bundles/{licensedBundle:guid}/named-user/{id:guid}")]
        [TenantIdParameterHint("licensedTenant", ParameterLocation.Route, TenantIdParameterKind.LicensedTenantId)]
        [UserNotRequired]
        public async Task<ActionResult> UpdateNamedUsersFromBundle([FromRoute] Guid licensedTenant, [FromRoute] Guid licensedBundle, [FromBody] UpdateNamedUserBundleLicenseInput input, 
            [FromRoute] Guid id)
        {
            var result = await _licensedTenantService.UpdateNamedUsersFromBundle(licensedTenant, licensedBundle, input, id);
            
            switch (result.ValidationCode)
            {
                case UpdateNamedUsersFromBundleValidationCode.NoError:
                    return Ok(result);
                case UpdateNamedUsersFromBundleValidationCode.NoLicensedTenant:
                case UpdateNamedUsersFromBundleValidationCode.NoLicensedBundle:
                case UpdateNamedUsersFromBundleValidationCode.NoNamedUser:
                    return NotFound(result);
                case UpdateNamedUsersFromBundleValidationCode.NamedUserEmailAlreadyInUse:
                    return BadRequest(result);
                default:
                    throw new InvalidEnumArgumentException($"Couldn't find a proper way to handle the following operationValidation value: {result.ValidationCode} {result.ValidationCode.ToString()}");
            }
        }

        [HttpDelete("/licensing/licensing-management/licenses/{licensedTenant:guid}/licensed-bundles/{licensedBundle:guid}/named-user/{id:guid}")]
        [TenantIdParameterHint("licensedTenant", ParameterLocation.Route, TenantIdParameterKind.LicensedTenantId)]
        public async Task<ActionResult> RemoveNamedUserFromBundle([FromRoute] Guid licensedTenant, [FromRoute] Guid licensedBundle, [FromRoute] Guid id)
        {
            var result = await _licensedTenantService.RemoveNamedUserFromBundle(licensedTenant, licensedBundle, id);

            if (result.Success) 
                return Ok();

            switch (result.ValidationCode)
            {
                case RemoveNamedUserFromBundleValidationCode.NoNamedUser:
                case RemoveNamedUserFromBundleValidationCode.NoLicensedTenant:
                case RemoveNamedUserFromBundleValidationCode.NoLicensedBundle:
                    return NotFound(result);
                default:
                    throw new InvalidEnumArgumentException($"Couldn't find a proper way to handle the following operationValidation value: {result.ValidationCode} {result.ValidationCode.ToString()}");
            }
        }
        
        [HttpGet("/licensing/licensing-management/licenses/licensed-bundles/{licensedBundleId:guid}")]
        public async Task<LicensedBundleOutput> GetLicensedBundleById(Guid licensedBundleId)
        {
            return await _licensedBundleService.GetLicensedBundleById(licensedBundleId);
        }
        
    }
}