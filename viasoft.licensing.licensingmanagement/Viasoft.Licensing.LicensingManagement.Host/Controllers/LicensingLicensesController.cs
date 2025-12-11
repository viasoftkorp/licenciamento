using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensingLicenses;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensingLicenses;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class LicensingLicensesController: BaseController
    {
        private readonly IProvideLicensingLicensesService _provideLicensingLicensesService;

        public LicensingLicensesController(IProvideLicensingLicensesService provideLicensingLicensesService)
        {
            _provideLicensingLicensesService = provideLicensingLicensesService;
        }

        [HttpGet("/licensing/licensing-management/licenses/{identifier:guid}")]
        [TenantIdParameterHint("identifier", ParameterLocation.Route)]
        [UserNotRequired]
        public async Task<IActionResult> GetLicensingLicenses([FromRoute] Guid identifier)
        {
            var result = await _provideLicensingLicensesService.GetLicensingLicenses(identifier);
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        [HttpPut("/licensing/licensing-management/licenses/{identifier:guid}/hardwareId")]
        [TenantIdParameterHint("identifier", ParameterLocation.Route)]
        [UserNotRequired]
        public async Task<IActionResult> UpdateHardwareId([FromRoute] Guid identifier, [FromBody] UpdateHardwareIdInput input)
        {
            var result = await _provideLicensingLicensesService.UpdateHardwareId(identifier, input);
            
            if (!result.IsSuccess)
            {
                switch (result.Code)
                {
                    case UpdateHardwareIdEnum.CouldNotFindEntity:
                        return NotFound(result);
                    case UpdateHardwareIdEnum.EntityHardwareIdNotEmpty:
                        return BadRequest(result);
                    case UpdateHardwareIdEnum.InputHardwareIdEmpty:
                        return BadRequest(result);
                }
            }

            return Ok(result);
        }
    }
}