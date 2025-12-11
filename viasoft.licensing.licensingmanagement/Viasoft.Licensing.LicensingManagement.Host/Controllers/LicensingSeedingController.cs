using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.Seeder;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LicensingSeedingController: BaseController
    {
        private readonly ISoftwareAndAppDataSeeder _softwareAndAppDataSeeder;

        public LicensingSeedingController(ISoftwareAndAppDataSeeder softwareAndAppDataSeeder)
        {
            _softwareAndAppDataSeeder = softwareAndAppDataSeeder;
        }
        
        //this method only exists so our seeders can run
        [HttpPost]
        public async Task<IActionResult> SeedTenant(Guid tenantIdentifier, string administratorEmail, string defaultLicensedCnpjs, 
            Guid? administratorUserId)
        {
            try
            {
                await _softwareAndAppDataSeeder.Seed(tenantIdentifier, administratorEmail, defaultLicensedCnpjs, administratorUserId);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}