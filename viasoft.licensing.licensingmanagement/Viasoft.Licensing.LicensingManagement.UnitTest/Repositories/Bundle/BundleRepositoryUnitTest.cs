using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Repositories.Bundle
{
    public class BundleRepositoryUnitTest: LicensingManagementTestBase
    {
        
        [Fact(DisplayName = "Testa o retorno do metodo que busca apps em bundles")]
        public async Task Get_Bundle_Apps_Returns()
        {
            // prepare data
            var bundlesIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var appId = Guid.NewGuid();
            var expectedResult = await InsertBundleApps(appId, bundlesIds);
            var repository = GetRepository();
            // execute
            var result = await repository.GetBundledApps(new List<Guid>{ appId }, bundlesIds);
            // test
            expectedResult.Should().BeEquivalentTo(result);
        }
    
        [Fact(DisplayName = "Testa o retorno do metodo que busca bundles que que já estão licenciados")]
        public async Task Get_BundlesId_Already_In_Licenses()
        {
            // prepare data
            var bundlesIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var licensedTenantIds = new List<Guid> { Guid.NewGuid() };
            await InsertLicensedBundles(bundlesIds, licensedTenantIds);
            var repository = GetRepository();
            // execute
            var result = await repository.GetBundlesIdsAlreadyLicensed(bundlesIds);
            // test
            bundlesIds.Should().BeEquivalentTo(result);
        }

        [Fact(DisplayName = "Testa se um Bundle está licensiado pelo seu Id (Sem dados para este Id no Repositório)")]
        public async Task Check_License_Bundle_No_Data()
        {
            // prepare data
            var repository = GetRepository();
            var fakeData = Guid.NewGuid();
            // execute
            var licensedBundle = await repository.CheckIfBundleIsLicensedInAnyLicensing(fakeData);
            // test
            Assert.False(licensedBundle);
        }
        
        [Fact(DisplayName = "Testa se um Bundle está licensiado pelo seu Id (Com dados para este Id no Repositório)")]
        public async Task Check_License_Bundle_With_Data()
        {
            // prepare data
            var repository = GetRepository();
            var bundlesIds = new List<Guid> { Guid.NewGuid() };
            var licensedTenantIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var fakeData = await InsertLicensedBundles(bundlesIds, licensedTenantIds);
            // execute
            var licensedBundle = await repository.CheckIfBundleIsLicensedInAnyLicensing(fakeData[0].BundleId);
            // test
            Assert.True(licensedBundle);
        }
        
        [Fact(DisplayName = "Retorna a lista de licenciamentos referente a um bundle específico (Sem bundles para este Id no Repositório)")]
        public async Task Get_License_Bundle_By_Licenses_No_Data()
        {
            // prepare data
            var repository = GetRepository();
            var fakeData = Guid.NewGuid();
            // execute
            var licensedBundles = await repository.GetLicensedBundles(fakeData);
            // test
            new List<LicensedBundle>().Should().BeEquivalentTo(licensedBundles);
        }
        
        [Fact(DisplayName = "Retorna a lista de licenciamentos referente a um bundle específico (Com dados para este Id no Repositório)")]
        public async Task Get_License_Bundle_By_Licenses_With_Data()
        {
            // prepare data
            var repository = GetRepository();
            var bundleId = Guid.NewGuid();
            var expectedResult = new List<LicensedBundle>();
            var bundlesIds = new List<Guid> { bundleId, bundleId, bundleId, Guid.NewGuid(), bundleId };
            var licensedTenantIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            expectedResult.AddRange(await InsertLicensedBundles(bundlesIds, licensedTenantIds));
            expectedResult = expectedResult.Where(r => r.BundleId == bundleId).ToList();
            // execute
            var licensedBundles = await repository.GetLicensedBundles(bundleId);
            // test
            expectedResult.Should().BeEquivalentTo(licensedBundles);
        }
        
        [Fact(DisplayName = "Testa se o GetAll de bundles está retornando corretamente todas as ocorrências")]
        public async Task Check_Bundles_Get_All()
        {
            // prepare data
            var bundlesIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var bundles = await InsertBundles(bundlesIds);
            var repository = GetRepository();
            var expectedBundles = bundles.Where(b => b.IsActive).Select(b => new BundlesGetForBatchOperations {Id = b.Id, Identifier = b.Identifier}).ToList();
            // execute
            var result = await repository.GetAllBundlesForBatchOperation(null);
            // test
            expectedBundles.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa se o GetAll de bundles está retornando corretamente todas as ocorrências com filtro avançado")]
        public async Task Check_Bundles_Get_All_With_Advanced_Filter()
        {
            // prepare data
            var bundlesIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var bundles = await InsertBundles(bundlesIds);
            var repository = GetRepository();
            var advancedFilter = "{\"condition\":\"and\",\"rules\":[{\"field\":\"name\",\"operator\":\"contains\",\"type\":\"string\",\"value\":\"0\"}]}";
            var expectedBundles = bundles.Where(b => b.IsActive && b.Name.Contains("0")).Select(b => new BundlesGetForBatchOperations {Id = b.Id, Identifier = b.Identifier}).ToList();
            // execute
            var result = await repository.GetAllBundlesForBatchOperation(advancedFilter);
            // test
            expectedBundles.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa se buscar as bundles por uma lista de ids está retornando corretamente as ocorrências")]
        public async Task Check_Get_Bundles_By_Ids()
        {
            // prepare data
            var bundlesIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var excludeBundlesIds= new List<Guid> {bundlesIds[0], bundlesIds[1]};
            var includeBundlesIds = new List<Guid> {bundlesIds[2], bundlesIds[3]};
            var bundles = await InsertBundles(bundlesIds);
            var repository = GetRepository();
            var expectedResult = bundles
                .Where(l => !excludeBundlesIds.Contains(l.Id) && includeBundlesIds.Contains(l.Id) && l.IsActive).Select(b => new BundlesGetForBatchOperations {Id = b.Id, Identifier = b.Identifier}).ToList();
            // execute
            var result = await repository.GetBundlesByIdsForBatchOperations(excludeBundlesIds, includeBundlesIds);
            // test
            expectedResult.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa o retorno do metodo que busca bundles que pertencem a licenças específicas")]
        public async Task Get_Bundles_Already_In_Licenses()
        {
            // prepare data
            var bundlesIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var licensedTenantIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var bundleIdsToFind = new List<Guid> {bundlesIds[2], bundlesIds[3]};
            var bundles = await InsertLicensedBundles(bundlesIds, licensedTenantIds);
            var expectedResult = bundles.Where(b => bundleIdsToFind.Contains(b.BundleId) && licensedTenantIds.Contains(b.LicensedTenantId)).Select(b => new LicensedBundleGetForBatchOperation {BundleId = b.BundleId, LicensedTenantId = b.LicensedTenantId}).ToList();
            var repository = GetRepository();
            // execute
            var result = await repository.GetBundlesAlreadyInLicenses(bundleIdsToFind, licensedTenantIds);
            // test
            expectedResult.Should().BeEquivalentTo(result);
        }

        public BundleRepository GetRepository()
        {
            return ActivatorUtilities.CreateInstance<BundleRepository>(ServiceProvider);
        }
        
        private async Task<List<LicensedBundle>> InsertLicensedBundles(List<Guid> bundleIds, List<Guid> licensesId)
        {
            var memoryRepository = ServiceProvider.GetService<IRepository<LicensedBundle>>();
            var output = new List<LicensedBundle>();
            foreach (var bundleId in bundleIds)
            {
                foreach (var license in licensesId)
                {
                    var licensedBundle = new LicensedBundle
                    {
                        Id = Guid.NewGuid(),
                        Status = LicensedBundleStatus.BundleActive,
                        BundleId = bundleId,
                        TenantId = Guid.NewGuid(),
                        NumberOfLicenses = 1,
                        LicensedTenantId = license
                    };
                    output.Add(licensedBundle);
                    await memoryRepository.InsertAsync(licensedBundle, true);
                }
            }
            return output;
        }

        private async Task<List<BundledApp>> InsertBundleApps(Guid appId, List<Guid> bundleIds)
        {
            var memoryRepository = ServiceProvider.GetService<IRepository<BundledApp>>();
            var output = new List<BundledApp>();
            foreach (var bundleId in bundleIds)
            {
                var newBundleApp = new BundledApp
                {
                    Id = Guid.NewGuid(),
                    AppId = appId,
                    BundleId = bundleId
                };
                await memoryRepository.InsertAsync(newBundleApp, true);
                output.Add(newBundleApp);
            }
            return output;
        }
        
        private async Task<List<Domain.Entities.Bundle>> InsertBundles(List<Guid> bundleIds)
        {
            var memoryRepository = ServiceProvider.GetService<IRepository<Domain.Entities.Bundle>>();
            var output = new List<Domain.Entities.Bundle>();
            for (int i = 0; i < bundleIds.Count; i++)
            {
                var isActive = i % 2 == 0;
                var bundle = new Domain.Entities.Bundle
                {
                    Id = bundleIds[i],
                    IsActive = isActive,
                    Name = i.ToString()
                };
                output.Add(bundle);
                await memoryRepository.InsertAsync(bundle, true);
            }
            return output;
        }
    }
}