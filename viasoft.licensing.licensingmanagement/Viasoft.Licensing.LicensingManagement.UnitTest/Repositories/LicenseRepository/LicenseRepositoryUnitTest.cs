using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.AmbientData.Extensions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Xunit;
using Utils = Viasoft.Licensing.LicensingManagement.UnitTest.Repositories.LicenseRepository.LicenseRepositoryUtils;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Repositories.LicenseRepository
{
    public class LicenseRepositoryUnitTest: LicensingManagementTestBase
    {
        [Fact(DisplayName = "Testa se o app é default pelo appID (Não sendo um app Default)")]
        public async Task Check_Licensed_App_Is_Default_No_Default()
        {
            // prepare data
            var licensedAppId = Guid.NewGuid();
            var repository = GetRepository();
            await InsertFakeApp(licensedAppId, false);
            // execute
            var isDefault = await repository.CheckIfLicensedAppIsDefault(licensedAppId);
            // test
            Assert.False(isDefault);
        }
        
        [Fact(DisplayName = "Testa se o app é default pelo appID (Sendo um app Default)")]
        public async Task Check_Licensed_App_Is_Default_Default()
        {
            // prepare data
            var licensedAppId = Guid.NewGuid();
            var repository = GetRepository();
            await InsertFakeApp(licensedAppId, true);
            // execute
            var isDefault = await repository.CheckIfLicensedAppIsDefault(licensedAppId);
            // test
            Assert.True(isDefault);
        }
        
        [Fact(DisplayName = "Testa se o GetAll de licenças está retornando corretamente todas as ocorrências")]
        public async Task Check_Licenses_Get_All()
        {
            // prepare data
            var licensesId = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var licenses = await InsertFakeLicenses(licensesId);
            var repository = GetRepository();
            var expectedResult = licenses.Select(l => l.Id).ToList();
            // execute
            var result = await repository.GetAllLicensesForBatchOperations(null);
            // test
            expectedResult.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa se o GetAll de licenças está retornando corretamente todas as ocorrências com filtro avançado")]
        public async Task Check_Licenses_Get_All_With_Advanced_Filter()
        {
            // prepare data
            var licensesId = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var licenses = await InsertFakeLicenses(licensesId);
            var repository = GetRepository();
            var advancedFilter = "{\"condition\":\"and\",\"rules\":[{\"field\":\"administratorEmail\",\"operator\":\"contains\",\"type\":\"string\",\"value\":\"0\"}]}";
            var expectedResult = licenses.Where(l => l.AdministratorEmail.Contains("0")).Select(l => l.Id).ToList();
            // execute
            var result = await repository.GetAllLicensesForBatchOperations(advancedFilter);
            // test
            expectedResult.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa se buscar as licenças por uma lista de ids está retornando corretamente as ocorrências")]
        public async Task Check_Get_Licenses_By_Ids()
        {
            // prepare data
            var licensesId = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var excludeLicensesIds = new List<Guid> {licensesId[0], licensesId[1]};
            var includeLicensesIds = new List<Guid> {licensesId[2], licensesId[3]};
            var licenses = await InsertFakeLicenses(licensesId);
            var repository = GetRepository();
            var expectedResult = licenses
                .Where(l => !excludeLicensesIds.Contains(l.Id) && includeLicensesIds.Contains(l.Id)).Select(l => l.Id).ToList();
            // execute
            var result = await repository.GetLicensesByIdsForBatchOperations(excludeLicensesIds, includeLicensesIds);
            // test
            expectedResult.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa a busca de detalhes para vários tenants (Com tenant com dados para busca e licenças temporárias validas)")]
        public async Task Check_Get_LicenseDetails_By_TenantIds_With_Data_And_Valid_Temporary_Licenses()
        {
            // prepare data
            var repository = GetRepository();
            var licensedTenantIds = new List<Guid>{ Guid.NewGuid() };
            var temporaryLicenses = 500;
            var expirationDateTime = new DateTime(2090, 5, 25);
            var expectedResult = await Utils.AddFakeData(ServiceProvider, licensedTenantIds[0], temporaryLicenses, expirationDateTime);
            expectedResult[0].OwnedBundles[0].NumberOfLicenses = expectedResult[0].OwnedBundles[0].NumberOfLicenses + temporaryLicenses;
            expectedResult[0].OwnedApps[0].NumberOfLicenses = expectedResult[0].OwnedApps[0].NumberOfLicenses + temporaryLicenses;
            // execute
            var result = await repository.GetLicenseDetailsByIdentifiers(licensedTenantIds);
            // test
            expectedResult.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa a busca de detalhes para vários tenants (Com tenant com dados para busca e licenças temporárias inválidas)")]
        public async Task Check_Get_LicenseDetails_By_TenantIds_With_Data_And_Invalid_Temporary_Licenses()
        {
            // prepare data
            var repository = GetRepository();
            var licensedTenantIds = new List<Guid>{ Guid.NewGuid() };
            var temporaryLicenses = 0;
            var expirationDateTime = new DateTime();
            var expectedResult = await Utils.AddFakeData(ServiceProvider, licensedTenantIds[0], temporaryLicenses, expirationDateTime);
            // execute
            var result = await repository.GetLicenseDetailsByIdentifiers(licensedTenantIds);
            // test
            expectedResult.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa a busca de detalhes para vários tenants (Com tenant sem dados para busca)")]
        public async Task Check_Get_LicenseDetails_By_TenantIds_No_Data()
        {
            // prepare data
            var repository = GetRepository();
            var licensedTenantIds = new List<Guid>{ Guid.NewGuid() };
            var expectedResult = new List<LicensedTenantDetails>()
            {
                new LicensedTenantDetails(new List<LicensedBundleDetails>(), new List<LicensedAppDetails>(), null)
                {
                    LicenseIdentifier = licensedTenantIds[0]
                }
            };
            // execute
            var result = await repository.GetLicenseDetailsByIdentifiers(licensedTenantIds);
            // test
            expectedResult.Should().BeEquivalentTo(result);
        }
        
        private Domain.LicensedTenant.Repository.LicenseRepository GetRepository()
        {
            return ActivatorUtilities.CreateInstance<Domain.LicensedTenant.Repository.LicenseRepository>(ServiceProvider);
        }
        
        private async Task<Domain.Entities.App> InsertFakeApp(Guid appId, bool isDefault)
        {
            var memoryRepository = ServiceProvider.GetRequiredService<IRepository<Domain.Entities.App>>();
            var newApp = new Domain.Entities.App
            {
                Domain = Domain.Enums.Domain.Billing,
                Identifier = "the catalyst",
                Name = "i bleed it out",
                Id = appId,
                Default = isDefault
            };
            return await memoryRepository.InsertAsync(newApp, true);
        }
        
        private async Task<List<LicensedTenant>> InsertFakeLicenses(List<Guid> licenses)
        {
            var memoryRepository = ServiceProvider.GetRequiredService<IRepository<LicensedTenant>>();
            var output = new List<LicensedTenant>();
            int i = 0;
            foreach (var license in licenses)
            {
                var newLicense = new LicensedTenant
                {
                    Id = license,
                    AdministratorEmail = i.ToString(),
                    AccountId = license,
                    Identifier = license,
                    TenantId = AmbientData.GetTenantId()
                };
                output.Add(newLicense);
                i++;
                await memoryRepository.InsertAsync(newLicense, true);
            }
            return output;
        }
    }
}