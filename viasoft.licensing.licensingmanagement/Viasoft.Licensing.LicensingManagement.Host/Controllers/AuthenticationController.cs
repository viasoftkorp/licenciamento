using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Authentication;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.User;
using Viasoft.Licensing.LicensingManagement.Domain.Services.AuthenticationService;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class AuthenticationController: BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("/licensing/licensing-management/licenses/{identifier:guid}/users")]
        public async Task<ActionResult<PagedResultDto<UsersGetAllOutput>>> GetAllUsers([FromRoute] Guid identifier, [FromQuery] GetAllUsersInput input)
        {
            var result = await _authenticationService.GetAllUsersFromTenantIdentifier(identifier, input);
            return Ok(result);
        }
    }
}