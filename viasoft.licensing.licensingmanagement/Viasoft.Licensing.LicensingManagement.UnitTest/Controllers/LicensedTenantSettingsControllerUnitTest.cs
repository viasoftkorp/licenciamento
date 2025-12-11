using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Host.Controllers;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Controllers
{
    public class LicensedTenantSettingsControllerUnitTest : LicensingManagementTestBase
    {
        private readonly List<LicensedTenantSettings> _licensedTenantSettingsToInsert = new()
        {
            new LicensedTenantSettings {
                Id = TestUtils.Guids[0],
                Key = TestUtils.CodeString[0],
                Value = TestUtils.CodeString[0],
                LicensingIdentifier = TestUtils.Guids[0]
            },
            
            new LicensedTenantSettings {
                Id = TestUtils.Guids[1],
                Key = LicensedTenantSettingsKeys.UseSimpleHardwareIdKey,
                Value = TestUtils.CodeString[1],
                LicensingIdentifier = TestUtils.Guids[0]
            },
        };
        
        [Fact(DisplayName = "Testar método Get")]
        public async Task GetTest()
        {
            #region Arrange

            var licensedTenantSettingsRepository = ServiceProvider.GetService<IRepository<LicensedTenantSettings>>();
            var controller = new LicensedTenantSettingsController(licensedTenantSettingsRepository);

            var identifier = TestUtils.Guids[0];

            await licensedTenantSettingsRepository!.InsertRangeAsync(_licensedTenantSettingsToInsert);
            await UnitOfWork.SaveChangesAsync();

            #endregion
            
            #region Act

            var output = await controller.Get(identifier);

            #endregion
            
            #region Assert

            output.Id.Should().Be(_licensedTenantSettingsToInsert[1].Id);
            output.Key.Should().Be(_licensedTenantSettingsToInsert[1].Key);
            output.Value.Should().Be(_licensedTenantSettingsToInsert[1].Value);
            
            #endregion
        }
        
        [Fact(DisplayName = "Testar método Update")]
        public async Task UpdateTest()
        {
            #region Arrange

            var licensedTenantSettingsRepository = ServiceProvider.GetService<IRepository<LicensedTenantSettings>>();
            var controller = new LicensedTenantSettingsController(licensedTenantSettingsRepository);

            var identifier = TestUtils.Guids[0];
            var input = new LicensedTenantSettingsInput
            {
                UseSimpleHardwareId = true
            };

            await licensedTenantSettingsRepository!.InsertRangeAsync(_licensedTenantSettingsToInsert);
            await UnitOfWork.SaveChangesAsync();

            #endregion
            
            #region Act

            var output = await controller.Update(identifier, input);

            #endregion
            
            #region Assert

            output.Id.Should().Be(_licensedTenantSettingsToInsert[1].Id);
            output.Key.Should().Be(_licensedTenantSettingsToInsert[1].Key);
            output.Value.Should().Be("True");
            
            #endregion
        }
    }
}