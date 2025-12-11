using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserProduct;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.NamedUserBundle;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    public class NamedUserController : BaseController
    {
        private readonly INamedUserBundleService _namedUserBundleService;

        public NamedUserController(INamedUserBundleService namedUserBundleService)
        {
            _namedUserBundleService = namedUserBundleService;
        }
        [HttpGet("/licensing/customer-licensing/licenses/{licensingIdentifier:guid}/users")]
        public async Task<PagedResultDto<GetAllUsersOutput>> GetAllUsers([FromRoute] Guid licensingIdentifier, [FromQuery] GetAllUsersInput input)
        {
            return await _namedUserBundleService.GetAllUsers(licensingIdentifier, input);
        }

        [HttpPost("/licensing/customer-licensing/licenses/{licensedTenant:guid}/products/{licensedProduct:guid}/named-user")]
        public async Task<ActionResult<NamedUserProductLicenseOutput>> AddNamedUserToProduct([FromRoute] Guid licensedTenant,
            [FromRoute] Guid licensedProduct, [FromBody] CreateNamedUserProductLicenseInput input)
        {
            var result = await _namedUserBundleService.AddNamedUserToProduct(licensedTenant, licensedProduct, input);
            switch (result.ValidationCode)
            {
                case AddNamedUserToProductValidationCode.NoError:
                    return Created($"/licensing/customer-licensing/licenses/{licensedTenant}/products/{licensedProduct}/named-user", result.Output);
                case AddNamedUserToProductValidationCode.NoLicensedTenant:
                case AddNamedUserToProductValidationCode.NoProduct:
                    return NotFound(result);
                case AddNamedUserToProductValidationCode.TooManyNamedUsers:
                case AddNamedUserToProductValidationCode.NamedUserEmailAlreadyInUse:    
                case AddNamedUserToProductValidationCode.ProductIsNotNamed:
                    return BadRequest(result);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        [HttpPut("/licensing/customer-licensing/licenses/{licensedTenant:guid}/products/{licensedProduct:guid}/named-user/{namedUserEmail}")]
        public async Task<ActionResult<UpdateNamedUsersFromProductOutput>> UpdateNamedUserFromProduct([FromRoute] Guid licensedTenant,
            [FromRoute] Guid licensedProduct, [FromRoute] string namedUserEmail, [FromBody] UpdateNamedUserProductLicenseInput input)
        {
            var result = await _namedUserBundleService.UpdateNamedUserFromProduct(licensedTenant, licensedProduct, namedUserEmail, input);
            switch (result.ValidationCode)
            {
                case UpdateNamedUsersFromProductValidationCode.NoError:
                    return Ok(result);
                case UpdateNamedUsersFromProductValidationCode.NoLicensedTenant:
                case UpdateNamedUsersFromProductValidationCode.NoProduct:
                case UpdateNamedUsersFromProductValidationCode.NoNamedUser:
                    return NotFound(result);
                case UpdateNamedUsersFromProductValidationCode.NamedUserEmailAlreadyInUse:
                    return BadRequest(result);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [HttpDelete("/licensing/customer-licensing/licenses/{licensedTenant:guid}/products/{licensedProduct:guid}/named-user/{namedUserEmail}")]
        public async Task<ActionResult<RemoveNamedUserFromProductOutput>> RemoveNamedUserFromProduct([FromRoute] Guid licensedTenant,
            [FromRoute] Guid licensedProduct, [FromRoute] string namedUserEmail, [FromQuery] ProductType productType)
        {
            var result = await _namedUserBundleService.RemoveNamedUserFromProduct(licensedTenant, licensedProduct, namedUserEmail, productType);
            switch (result.ValidationCode)
            {
                case RemoveNamedUserFromProductValidationCode.NoError:
                    return Ok(result);
                case RemoveNamedUserFromProductValidationCode.NoLicensedTenant:
                case RemoveNamedUserFromProductValidationCode.NoProduct:
                case RemoveNamedUserFromProductValidationCode.NoNamedUser:
                    return NotFound(result);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}