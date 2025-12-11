using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.App;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Repositories.App
{
    public class AppRepositoryUnitTest: LicensingManagementTestBase
    {
        
        [Fact(DisplayName = "Testa se o identifier correto do app está sendo retornado")]
        public async Task Check_Get_AppIdentifier()
        {
            // prepare data
            var fakeAppId = Guid.NewGuid();
            var repository = GetRepository();
            var fakeApp = await InsertFakeApp(fakeAppId);
            // execute
            var appIdentifier = await repository.GetAppIdentifiersByAppIds(new List<Guid>{fakeAppId});
            // test
            Assert.Single(appIdentifier);
            Assert.Equal(fakeApp.Identifier, appIdentifier[0].Identifier);
            Assert.Equal(fakeApp.Id, appIdentifier[0].Id);
        }
        
        [Fact(DisplayName = "Testa se o método para retornar um app em licenciamentos especificos está retornando corretamente")]
        public async Task Get_App_For_Licenses()
        {
            // prepare data
            var fakeAppId = Guid.NewGuid();
            var fakeLicensesId = new List<Guid>{Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};
            var licensedApps = await InsertFakeLicensedApps(fakeAppId, fakeLicensesId);
            var fakeLicensesIdToFilterApps = new List<Guid>{fakeLicensesId[0]};
            var expectedResult = licensedApps.Where(l => fakeLicensesIdToFilterApps.Contains(l.LicensedTenantId)).ToList();
            var repository = GetRepository();
            // execute
            var result = await repository.GetLicensedAppsForLicenses(fakeLicensesIdToFilterApps, fakeAppId);
            // test
            expectedResult.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa o retorno do metodo que busca apps que pertencem a licenças específicas")]
        public async Task Get_Apps_Already_In_Licenses()
        {
            // prepare data
            var fakeAppsId = new List<Guid>{Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};
            var fakeLicensesId = new List<Guid>{Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};
            var licensedApps = await InsertFakeLicensedApps(fakeAppsId[0], fakeLicensesId);
            var expectedResult = licensedApps.Where(l => fakeLicensesId.Contains(l.LicensedTenantId) && fakeAppsId.Contains(l.AppId)).Select(l => new AlreadyLicensedApp { AppId = l.AppId, LicensedTenantId = l.LicensedTenantId}).ToList();
            var repository = GetRepository();
            // execute
            var result = await repository.GetAppsAlreadyLicensed(fakeAppsId, fakeLicensesId);
            // test
            expectedResult.Should().BeEquivalentTo(result);
        }
        
        private AppRepository GetRepository()
        {
            return ActivatorUtilities.CreateInstance<AppRepository>(ServiceProvider);
        }

        private async Task<Domain.Entities.App> InsertFakeApp(Guid appId)
        {
            var memoryRepository = ServiceProvider.GetService<IRepository<Domain.Entities.App>>();
            var newApp = new Domain.Entities.App
            {
                Domain = Domain.Enums.Domain.Billing,
                Identifier = "chiaua",
                Name = "testeee",
                Id = appId
            };
            return await memoryRepository.InsertAsync(newApp, true);
        }
        
        private async Task<List<LicensedApp>> InsertFakeLicensedApps(Guid appId, List<Guid> licenseIds)
        {
            var memoryRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var output = new List<LicensedApp>();
            foreach (var licenseId in licenseIds)
            {
                var newLicensedApp = new LicensedApp
                {
                    Id = Guid.NewGuid(),
                    AppId = appId,
                    LicensedTenantId = licenseId
                };
                output.Add(newLicensedApp);
                await memoryRepository.InsertAsync(newLicensedApp, true);
            }

            return output;
        }

        [Fact]
        public async Task MustReturnDictionaryOfIdsAndDomains()
        {
            //arrange
            var apps = new List<Domain.Entities.App>
            {
                new Domain.Entities.App
                {
                    Default = true,
                    Domain = GetRandomDomain(),
                    Id = Guid.NewGuid(),
                    Identifier = "TST",
                    Name = "teste",
                    IsActive = true,
                    SoftwareId = Guid.NewGuid(),
                    TenantId = Guid.NewGuid()
                },
                new Domain.Entities.App
                {
                    Default = true,
                    Domain = GetRandomDomain(),
                    Id = Guid.NewGuid(),
                    Identifier = "TST2",
                    Name = "teste2",
                    SoftwareId = Guid.NewGuid(),
                    TenantId = Guid.NewGuid()
                }
            };
            var repo = ServiceProvider.GetService<IRepository<Domain.Entities.App>>();
            var service = ActivatorUtilities.CreateInstance<AppRepository>(ServiceProvider);
            foreach (var app in apps)
            {
                await repo.InsertAsync(app, true);
            }
            var ids = apps.Select(a => a.Identifier).ToList();
            
            //act
            var dictionary = await service.GetDomainsByAppIdentifiers(ids);
            
            //assert
            Assert.True(dictionary.ContainsKey(apps[0].Identifier));
            Assert.True(dictionary.ContainsKey(apps[1].Identifier));
            Assert.True(dictionary.ContainsValue(apps[0].Domain));
            Assert.True(dictionary.ContainsValue(apps[1].Domain));
        }

        [Fact]
        public async Task MustReturnADictionaryOfIdsAndDomainsFromBothTenants()
        {
            //arrange
            var apps = new List<Domain.Entities.App>
            {
                new Domain.Entities.App
                {
                    Default = true,
                    Domain = GetRandomDomain(),
                    Id = Guid.NewGuid(),
                    Identifier = "TST",
                    Name = "teste",
                    IsActive = true,
                    SoftwareId = Guid.NewGuid(),
                    TenantId = Guid.NewGuid()
                },
                new Domain.Entities.App
                {
                    Default = true,
                    Domain = GetRandomDomain(),
                    Id = Guid.NewGuid(),
                    Identifier = "TST2",
                    Name = "teste2",
                    SoftwareId = Guid.NewGuid(),
                    TenantId = Guid.NewGuid()
                }
            };
            var tenant = ServiceProvider.GetService<ICurrentTenant>();
            tenant.Id = apps[0].TenantId;
            var repo = ServiceProvider.GetService<IRepository<Domain.Entities.App>>();
            var service = ActivatorUtilities.CreateInstance<AppRepository>(ServiceProvider);
            foreach (var app in apps)
            {
                await repo.InsertAsync(app, true);
            }
            var ids = apps.Select(a => a.Identifier).ToList();
            
            //act
            var dictionary = await service.GetDomainsByAppIdentifiers(ids);
            
            //assert
            Assert.True(dictionary.ContainsKey(apps[0].Identifier));
            Assert.True(dictionary.ContainsKey(apps[1].Identifier));
            Assert.True(dictionary.ContainsValue(apps[0].Domain));
            Assert.True(dictionary.ContainsValue(apps[1].Domain));
        }

        private Domain.Enums.Domain GetRandomDomain()
        {
            var rnd = new Random();
            return (Domain.Enums.Domain) rnd.Next(Enum.GetNames(typeof(Domain.Enums.Domain)).Length);
        }
    }
}