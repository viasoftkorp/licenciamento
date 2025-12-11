using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings.Messages;
using Viasoft.Licensing.LicensingManagement.Host.Handlers.LicensedTenantSettings;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Handlers.LicensedTenantSettings
{
    public class SeedLicensedTenantSettingsHandlerUnitTest : LicensingManagementTestBase
    {
        [Fact(DisplayName = "Testar handle para a mensagem SeedLicensingTenantSettingsMessage")]
        public async Task HandleSeedLicensingTenantSettingsMessageTest()
        {
            #region Arrange

            var licensedTenantSettingsRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenantSettings>>();
            var licensedTenantRepository = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var handler = new SeedLicensedTenantSettingsHandler(licensedTenantRepository, licensedTenantSettingsRepository, UnitOfWork);
            
            var firstLicensedTenantSettingsToInsert = new Domain.Entities.LicensedTenantSettings
            {
                Id = TestUtils.Guids[0],
                Key = TestUtils.CodeString[0],
                Value = TestUtils.CodeString[0],
                TenantId = TestUtils.Guids[0],
                LicensingIdentifier = TestUtils.Guids[0]
            };

            var licensedTenantsToInsert = new List<LicensedTenant>
            {
                new() {
                    Id = TestUtils.Guids[0],
                    TenantId = TestUtils.Guids[0],
                    Identifier = TestUtils.Guids[0]
                },
                
                new() {
                    Id = TestUtils.Guids[1],
                    TenantId = TestUtils.Guids[1],
                    Identifier = TestUtils.Guids[1]
                },
                
                new() {
                    Id = TestUtils.Guids[2],
                    TenantId = TestUtils.Guids[2],
                    Identifier = TestUtils.Guids[2]
                }
            };
            
            await licensedTenantSettingsRepository!.InsertAsync(firstLicensedTenantSettingsToInsert);
            await licensedTenantRepository!.InsertRangeAsync(licensedTenantsToInsert);
            await UnitOfWork.SaveChangesAsync();

            #endregion
            
            #region Act

            await handler.Handle(new SeedLicensingTenantSettingsMessage());

            #endregion
            
            #region Assert

            var licensedTenantSettingsInserted = await licensedTenantSettingsRepository
                .OrderBy(l => l.CreationTime)
                .ToListAsync();

            licensedTenantSettingsInserted.Count.Should().Be(3);
            
            licensedTenantSettingsInserted[0].Id.Should().Be(firstLicensedTenantSettingsToInsert.Id);       
            licensedTenantSettingsInserted[0].Key.Should().Be(firstLicensedTenantSettingsToInsert.Key);
            licensedTenantSettingsInserted[0].Value.Should().Be(firstLicensedTenantSettingsToInsert.Value);
            licensedTenantSettingsInserted[0].TenantId.Should().Be(firstLicensedTenantSettingsToInsert.TenantId);
            licensedTenantSettingsInserted[0].LicensingIdentifier.Should().Be(firstLicensedTenantSettingsToInsert.LicensingIdentifier);
            
            licensedTenantSettingsInserted[1].Id.Should().NotBeEmpty();       
            licensedTenantSettingsInserted[1].Key.Should().Be(LicensedTenantSettingsKeys.UseSimpleHardwareIdKey);
            licensedTenantSettingsInserted[1].Value.Should().Be(bool.FalseString);
            licensedTenantSettingsInserted[1].TenantId.Should().Be(licensedTenantsToInsert[1].TenantId);
            licensedTenantSettingsInserted[1].LicensingIdentifier.Should().Be(licensedTenantsToInsert[1].Identifier);
            
            licensedTenantSettingsInserted[2].Id.Should().NotBeEmpty();       
            licensedTenantSettingsInserted[2].Key.Should().Be(LicensedTenantSettingsKeys.UseSimpleHardwareIdKey);
            licensedTenantSettingsInserted[2].Value.Should().Be(bool.FalseString);
            licensedTenantSettingsInserted[2].TenantId.Should().Be(licensedTenantsToInsert[2].TenantId);
            licensedTenantSettingsInserted[2].LicensingIdentifier.Should().Be(licensedTenantsToInsert[2].Identifier);
            
            #endregion
        }
    }
}