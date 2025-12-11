using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.App;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Repositories.LicenseBatchRepository
{
    public class LicenseBatchRepositoryUnitTest: LicensingManagementTestBase
    {
        [Fact(DisplayName = "Testa o metodo para checar  bundles com apps licenciados (Sem compatibilidade)")]
        public async Task Check_Licensed_Bundles_With_Apps_Licensed_No_Data()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var fakeIdInvoke = Guid.NewGuid();
            var fakeBundleList = FakeBundleList(fakeIdInvoke, bundleId);
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(r => r.GetLicensedBundles(bundleId))
                .ReturnsAsync(fakeBundleList);
            var fakeAppRepository = new Mock<IAppRepository>();
            var service = GetRepository(fakeBundleRepository, fakeAppRepository);
            // execute
            var result = await service.GetLicensedBundlesWithAppsLicensed(bundleId, appId);
            // test
            Assert.Empty(result);
        }
        
        [Fact(DisplayName = "Testa o metodo para checar  bundles com apps licenciados (Com licenciamento compátivel e este app não licenciado)")]
        public async Task Check_Licensed_Bundles_With_Apps_Licensed_With_Data()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var fakeIdInvoke = Guid.NewGuid();
            var fakeBundleList = FakeBundleList(fakeIdInvoke, bundleId);
            await CreateLicensedApp(fakeIdInvoke, appId, bundleId);
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(r => r.GetLicensedBundles(bundleId))
                .ReturnsAsync(fakeBundleList);
            var fakeAppRepository = new Mock<IAppRepository>();
            var service = GetRepository(fakeBundleRepository, fakeAppRepository);
            // execute
            var result = await service.GetLicensedBundlesWithAppsLicensed(bundleId, appId);
            // test
            fakeBundleList.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa o metodo de buscar um dicionario de identifier")]
        public async Task Check_Licensed_Tenant_Dictionary()
        {
            // prepare data
            var licenseTenantIds = new List<Guid>{Guid.NewGuid()};
            var identifiers = await CreateLicenseTenants(licenseTenantIds);
            var fakeBundleRepository = new Mock<IBundleRepository>();
            var fakeAppRepository = new Mock<IAppRepository>();
            var service = GetRepository(fakeBundleRepository, fakeAppRepository);
            // execute
            var result = await service.GetLicensedTenantToIdentifierDictionary(licenseTenantIds);
            // test
            Assert.Single(result);
            Assert.Equal(result[licenseTenantIds[0]], identifiers[0]);
        }
        
        [Fact(DisplayName = "Testa o metodo de buscar um dicionario de identifier (Alguns Aplicativos Licenciados)")]
        public async Task Check_Licensed_Bundles_With_Apps_UnLicenced_Some_Apps_Licensed()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var appsIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid()};
            var licensedTenantId = Guid.NewGuid();
            await CreateLicensedApp(licensedTenantId, appsIds[0], null);
            var fakeBundleRepository = new Mock<IBundleRepository>();
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(a => a.GetAppIdentifiersByAppIds(new List<Guid> {appsIds[1]}))
                .ReturnsAsync(new List<AppsGetForBatchOperations>{ new AppsGetForBatchOperations{ Id = appsIds[1], Identifier = "Soul Song - Grey Daze"}});
            fakeBundleRepository.Setup(b => b.GetLicensedBundles(bundleId))
                .ReturnsAsync(new List<LicensedBundle>{ new LicensedBundle{ BundleId = bundleId, LicensedTenantId = licensedTenantId, NumberOfLicenses = 0, Status = LicensedBundleStatus.BundleBlocked}});
            var expectedOutput = new List<LicensedBundlesWithUnLicensedAppsForBatchOperations>
            {
                new LicensedBundlesWithUnLicensedAppsForBatchOperations
                {
                    Status = LicensedBundleStatus.BundleBlocked,
                    BundleId = bundleId,
                    LicensedTenantId = licensedTenantId,
                    NumberOfLicenses = 0,
                    AppsGetForBatchOperations = new List<AppsGetForBatchOperations>
                        {new AppsGetForBatchOperations {Id = appsIds[1], Identifier = "Soul Song - Grey Daze"}}
                }
            };
            var service = GetRepository(fakeBundleRepository, fakeAppRepository);
            // execute
            var result = await service.GetLicensedBundlesWithUnLicensedAppsForBatchOperations(bundleId, appsIds);
            // TEST METHOD RETURNS 
            expectedOutput.Should().BeEquivalentTo(result);
            // TEST CALLS COUNT
            fakeAppRepository.Invocations.AssertCount(1, nameof(IAppRepository.GetAppIdentifiersByAppIds));
            fakeBundleRepository.Invocations.AssertCount(1, nameof(IBundleRepository.GetLicensedBundles));
            // TEST METHOD INVOCATIONS
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppIdentifiersByAppIds), new List<Guid> {appsIds[1]});
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetLicensedBundles), bundleId);
        }
        
        [Fact(DisplayName = "Testa o metodo de buscar um dicionario de identifier (Todos os aplicativos licenciados)")]
        public async Task Check_Licensed_Bundles_With_Apps_UnLicenced_All_Apps_Licensed()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var appsIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid()};
            var licensedTenantIdFirst = Guid.NewGuid();
            var licensedTenantIdSecond = Guid.NewGuid();
            await CreateLicensedApp(licensedTenantIdFirst, appsIds[0], null);
            await CreateLicensedApp(licensedTenantIdFirst, appsIds[1], null);
            await CreateLicensedApp(licensedTenantIdSecond, appsIds[0], null);
            await CreateLicensedApp(licensedTenantIdSecond, appsIds[1], null);
            var fakeBundleRepository = new Mock<IBundleRepository>();
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(a => a.GetAppIdentifiersByAppIds(new List<Guid>()))
                .ReturnsAsync(new List<AppsGetForBatchOperations>());
            fakeBundleRepository.Setup(b => b.GetLicensedBundles(bundleId))
                .ReturnsAsync(new List<LicensedBundle>
                    {
                        new LicensedBundle{ BundleId = bundleId, LicensedTenantId = licensedTenantIdFirst, NumberOfLicenses = 0, Status = LicensedBundleStatus.BundleBlocked},
                        new LicensedBundle{ BundleId = bundleId, LicensedTenantId = licensedTenantIdSecond, NumberOfLicenses = 500, Status = LicensedBundleStatus.BundleActive}
                    }
                );
            var service = GetRepository(fakeBundleRepository, fakeAppRepository);
            // execute
            var result = await service.GetLicensedBundlesWithUnLicensedAppsForBatchOperations(bundleId, appsIds);
            // TEST METHOD RETURNS 
            new List<LicensedBundlesWithUnLicensedAppsForBatchOperations>().Should().BeEquivalentTo(result);
            // TEST CALLS COUNT
            fakeAppRepository.Invocations.AssertCount(2, nameof(IAppRepository.GetAppIdentifiersByAppIds));
            fakeBundleRepository.Invocations.AssertCount(1, nameof(IBundleRepository.GetLicensedBundles));
            // TEST METHOD INVOCATIONS
            fakeAppRepository.Invocations.AssertArgument(0,0, nameof(IAppRepository.GetAppIdentifiersByAppIds), new List<Guid>());
            fakeAppRepository.Invocations.AssertArgument(1,0, nameof(IAppRepository.GetAppIdentifiersByAppIds), new List<Guid>());
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetLicensedBundles), bundleId);
        }

        private Domain.Repositories.LicenseBatchRepository.LicenseBatchRepository GetRepository(Mock<IBundleRepository> bundleService, Mock<IAppRepository> appRepository)
        {
            return ActivatorUtilities.CreateInstance<Domain.Repositories.LicenseBatchRepository.LicenseBatchRepository>(ServiceProvider, bundleService.Object, appRepository.Object);
        }

        private async Task<List<Guid>> CreateLicenseTenants(List<Guid> licenseTenantIds)
        {
            var memoryRepository = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var identifiers = new List<Guid>();
            foreach (var licenseTenantId in licenseTenantIds)
            {
                var newLicensedTenant = new LicensedTenant
                {
                    Id = licenseTenantId,
                    Identifier = Guid.NewGuid()
                };
                identifiers.Add(newLicensedTenant.Identifier);
                await memoryRepository.InsertAsync(newLicensedTenant, true);
            }

            return identifiers;
        }
        
        private async Task<LicensedApp> CreateLicensedApp(Guid licensedId, Guid appId, Guid? bundleId)
        {
            var memoryRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var newLicensedApp = new LicensedApp
            {
                AppId = appId,
                Id = Guid.NewGuid(),
                LicensedTenantId = licensedId,
                LicensedBundleId = bundleId
            };
            return await memoryRepository.InsertAsync(newLicensedApp, true);
        }
        
        private List<LicensedBundle> FakeBundleList(Guid licensedTenant, Guid bundleId)
        {
            return new List<LicensedBundle>
            {
                new LicensedBundle
                {
                    Id = Guid.NewGuid(),
                    Status = LicensedBundleStatus.BundleActive,
                    LicensedTenantId = licensedTenant,
                    NumberOfLicenses = 1,
                    BundleId = bundleId
                }
            };
        }
    }
}