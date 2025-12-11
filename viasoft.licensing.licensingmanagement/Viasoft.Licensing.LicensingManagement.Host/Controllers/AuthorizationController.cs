using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller.Authorization;
using Viasoft.Licensing.LicensingManagement.Domain.Authorizations;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class AuthorizationController: BaseAuthorizationController
    {
        [HttpGet]
        public override Task<List<string>> GetAuthorizations()
        {
            return Task.FromResult(
                new List<string>
                {
                    Policy.InsertAppInLicenses,
                    Policy.RemoveAppInLicenses,
                    Policy.InsertBundlesInLicenses,
                    Policy.InsertAppsInLicenses,
                    Policy.InsertAppsInBundles,
                    Policy.CreateLicense,
                    Policy.UpdateLicense,
                    Policy.DeleteLicense,
                    Policy.CreateAccount,
                    Policy.UpdateAccount,
                    Policy.DeleteAccount,
                    Policy.CreateApp,
                    Policy.UpdateApp,
                    Policy.DeleteApp,
                    Policy.CreateBundle,
                    Policy.UpdateBundle,
                    Policy.DeleteBundle,
                    Policy.CreateSoftware,
                    Policy.UpdateSoftware,
                    Policy.DeleteSoftware
                }
            );
        }
    }
}