using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Authorizations;
using Viasoft.Licensing.LicensingManagement.Host.Controllers;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Controllers
{
    public class AuthorizationControllerUnitTest: LicensingManagementTestBase
    {
        
        [Fact(DisplayName = "Testa o retorno do método que busca autorizações")]
        public async Task Test_Get_Authorizations_Returns()
        {
            // prepare data
            var expectedResult = new List<string>
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
            };
            var controller = GetController();
            // execute
            var result = await controller.GetAuthorizations();
            // TEST RETURN
            expectedResult.Should().BeEquivalentTo(result);
        }
        
        private AuthorizationController GetController()
        {
            return ActivatorUtilities.CreateInstance<AuthorizationController>(ServiceProvider);
        }
    }
}