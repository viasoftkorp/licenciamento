using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicenseServer;
using Xunit;
using Utils = Viasoft.Licensing.LicensingManagement.UnitTest.Services.LicenseServer.LicenseServerServiceUnitTestUtils;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.LicenseServer
{
    public class LicenseServerServiceUnitTest: LicensingManagementTestBase
    {
        [Fact(DisplayName = "Busca licenças por identificadores específicos e seus detalhes")]
        public async Task Get_Licenses_By_Identifiers()
        {
            var utils = await Utils.NewUtils(ServiceProvider);
            
            var service = ActivatorUtilities.CreateInstance<LicenseServerService>(ServiceProvider, utils.MockLicenseRepository.Object);
            var result = await service.GetLicensesByIdentifiers(utils.LicensedIdentifiers);
            
            utils.ExpectedLicenseByIdentifiers.Should().BeEquivalentTo(result);
            utils.MockLicenseRepository.Verify(repository => repository.GetLicenseDetailsByIdentifiers(utils.LicensedIdentifiers), Times.Once);
        }
    }
}