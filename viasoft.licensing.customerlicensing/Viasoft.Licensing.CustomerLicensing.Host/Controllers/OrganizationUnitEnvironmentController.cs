using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.MultiTenancy.Abstractions.Environment.Model;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.Environment;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    public class OrganizationUnitEnvironmentController : BaseController
    {
        private readonly IEnvironmentService _environmentService;

        public OrganizationUnitEnvironmentController(IEnvironmentService environmentService)
        {
            _environmentService = environmentService;
        }
        
        [HttpGet("/licensing/customer-licensing/organizations/units/{unitId:guid}/environments/{id:guid}")]
        [Authorize("Environments.Read")]
        //O parâmetro unitId é válido mas não é necessário para a chamada do Get, Activate e Deactivate da EnvironmentController do TenantManagement
        public async Task<OrganizationUnitEnvironmentOutput> GetEnvironmentById([FromRoute] Guid unitId, [FromRoute] Guid id, [FromQuery] GetEnvironmentInput input)
        {
            return await _environmentService.GetEnvironmentById(id, input);
        }
        
        [HttpGet("/licensing/customer-licensing/organizations/units/{unitId:guid}/environments")]
        [Authorize("Environments.Read")]
        public async Task<PagedResultDto<OrganizationUnitEnvironmentOutput>> GetEnvironmentByUnit( 
            [FromRoute] Guid unitId, [FromQuery] GetEnvironmentByUnitInput input)
        {
            return await _environmentService.GetEnvironmentByUnit(unitId, input);
        }

        [HttpPost("/licensing/customer-licensing/organizations/units/{unitId:guid}/environments")]
        [Authorize("Environments.Update")]
        public async Task<IActionResult> CreateEnvironment([FromRoute] Guid unitId, [FromBody] CreateOrUpdateEnvironmentInput input)
        {
            var createEnvironmentOutput = await _environmentService.CreateEnvironment(unitId, input);
            if (createEnvironmentOutput.Status != CreateEnvironmentOutputStatus.Ok)
            {
                return BadRequest(createEnvironmentOutput);
            }
            return Ok(createEnvironmentOutput);
        }

        [HttpPut("/licensing/customer-licensing/organizations/units/{unitId:guid}/environments/{id:guid}")]
        [Authorize("Environments.Update")]
        public async Task<IActionResult> UpdateEnvironment([FromRoute] Guid unitId, [FromRoute] Guid id, [FromBody] CreateOrUpdateEnvironmentInput input)
        {
            var updateEnvironmentOutput = await _environmentService.UpdateEnvironment(unitId, id, input);
            if (updateEnvironmentOutput.Status != UpdateEnvironmentOutputStatus.Ok)
            {
                return BadRequest(updateEnvironmentOutput);
            }

            return Ok(updateEnvironmentOutput);
        }

        [HttpPost("/licensing/customer-licensing/organizations/units/{unitId:guid}/environments/{id:guid}/activate")]
        [Authorize("Environments.Update")]
        public async Task<IActionResult> ActivateEnvironment([FromRoute] Guid unitId, [FromRoute] Guid id)
        {
            await _environmentService.ActivateEnvironment(id);
            return Ok();
        }
        
        [HttpPost("/licensing/customer-licensing/organizations/units/{unitId:guid}/environments/{id:guid}/deactivate")]
        [Authorize("Environments.Update")]
        public async Task<IActionResult> DeactivateEnvironment([FromRoute] Guid unitId, [FromRoute] Guid id)
        {
            await _environmentService.DeactivateEnvironment(id);
            return Ok();
        }
    }
}