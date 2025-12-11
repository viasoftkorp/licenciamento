using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.OrganizationUnit;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.OrganizationUnit;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    public class OrganizationUnitController : BaseController
    {
        private readonly IOrganizationUnitService _organizationUnitService;
        public OrganizationUnitController(IOrganizationUnitService organizationUnitService)
        {
            _organizationUnitService = organizationUnitService;
        }

        [HttpGet("/licensing/customer-licensing/organizations/{orgId:guid}/units")]
        [Authorize("Environments.Read")]
        public async Task<PagedResultDto<OrganizationUnitOutput>> GetUnitsByOrganization([FromRoute] Guid orgId, [FromQuery] PagedFilteredAndSortedRequestInput input)
        {
            return await _organizationUnitService.GetUnitsByOrganization(orgId, input);
        }
        
        [HttpGet("/licensing/customer-licensing/organizations/units/{id:guid}")]
        [Authorize("Environments.Read")]
        public async Task<OrganizationUnitOutput> GetUnitById([FromRoute] Guid id)
        {
            return await _organizationUnitService.GetUnitById(id);
        }
        
        [HttpPost("/licensing/customer-licensing/organizations/{orgId:guid}/units")]
        [Authorize("Environments.Update")]
        public async Task<IActionResult> CreateUnit([FromRoute] Guid orgId, [FromBody] CreateOrUpdateOrganizationUnitInput input)
        {
            var createUnitOutput =  await _organizationUnitService.CreateUnit(orgId, input);
            if (createUnitOutput.Status != CreateOrganizationUnitOutputStatus.Ok)
            {
                return BadRequest(createUnitOutput);
            }

            return Ok(createUnitOutput);
        }

        [HttpPut("/licensing/customer-licensing/organizations/units/{id:guid}")]
        [Authorize("Environments.Update")]
        public async Task<IActionResult> UpdateUnit([FromRoute] Guid id,
            [FromBody] CreateOrUpdateOrganizationUnitInput input)
        {
            var updateUnitOutput = await _organizationUnitService.UpdateUnit(id, input);
            if (updateUnitOutput.Status != UpdateOrganizationUnitOutputStatus.Ok)
            {
                return BadRequest(updateUnitOutput);
            }
            return Ok(updateUnitOutput);
        }

        [HttpPost("/licensing/customer-licensing/organizations/units/{id:guid}/activate")]
        [Authorize("Environments.Update")]
        public async Task<IActionResult> Activate([FromRoute] Guid id)
        { 
            await _organizationUnitService.ActivateUnit(id);
            return Ok();
        }
        
        [HttpPost("/licensing/customer-licensing/organizations/units/{id:guid}/deactivate")]
        [Authorize("Environments.Update")]
        public async Task<IActionResult> Deactivate([FromRoute] Guid id)
        {
            await _organizationUnitService.DeactivateUnit(id);
            return Ok();
        }
    }
}