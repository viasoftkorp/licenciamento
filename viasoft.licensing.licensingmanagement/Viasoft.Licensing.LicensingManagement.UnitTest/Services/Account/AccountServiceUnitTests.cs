using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Service;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.Account
{
    public class AccountServiceUnitTests : LicensingManagementTestBase
    {
        [Fact]
        public async Task MustReturnAccountNameFromLicensingIdentifier()
        {
            //arrange
            var account = new Domain.Entities.Account
            {
                Id = Guid.NewGuid(),
                City = "Curitiba",
                Country = "Brasil",
                Detail = null,
                Email = "teste@teste.com.br",
                Neighborhood = "Centro",
                Number = "2144",
                Phone = null,
                State = "Paraná",
                Status = AccountStatus.Active,
                Street = "Rua teste",
                BillingEmail = null,
                CnpjCpf = "83189520000111",
                CompanyName = "teste company name",
                TradingName = "teste trading name",
                WebSite = null,
                ZipCode = null
            };
            var tenant = new Domain.Entities.LicensedTenant
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid(),
                Notes = null,
                Status = LicensingStatus.Active,
                AccountId = account.Id,
                AdministratorEmail = "teste.teste@teste.com",
                LicensedCnpjList = {"83189520000111"},
                TenantId = Guid.Parse("EE19958D-5F6A-4201-B989-08D7468F1643"),
                LicensedCnpjs = "83189520000111"
            };
            var accountRepo = ServiceProvider.GetService<IRepository<Domain.Entities.Account>>();
            var tenantRepo = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            await accountRepo.InsertAsync(account, true);
            await tenantRepo.InsertAsync(tenant, true);
            var service = ActivatorUtilities.CreateInstance<AccountsService>(ServiceProvider);
            
            //act
            var accountNameOutput = await service.GetAccountInfoFromLicensingIdentifier(tenant.Identifier);
            var result = accountNameOutput.AccountName;

            //assert
            result.Should().Be(account.CompanyName);
        }

        [Fact]
        public async Task MustReturnNullFromLicensingIdentifier()
        {
            //arrange
            var account = new Domain.Entities.Account
            {
                Id = Guid.NewGuid(),
                City = "Curitiba",
                Country = "Brasil",
                Detail = null,
                Email = "teste@teste.com.br",
                Neighborhood = "Centro",
                Number = "2144",
                Phone = null,
                State = "Paraná",
                Status = AccountStatus.Active,
                Street = "Rua teste",
                BillingEmail = null,
                CnpjCpf = "83189520000111",
                CompanyName = "teste company name",
                TradingName = "teste trading name",
                WebSite = null,
                ZipCode = null
            };
            var tenant = new Domain.Entities.LicensedTenant
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid(),
                Notes = null,
                Status = LicensingStatus.Active,
                AccountId = account.Id,
                AdministratorEmail = "teste.teste@teste.com",
                LicensedCnpjList = {"83189520000111"},
                TenantId = Guid.Parse("EE19958D-5F6A-4201-B989-08D7468F1643"),
                LicensedCnpjs = "83189520000111"
            };
            var accountRepo = ServiceProvider.GetService<IRepository<Domain.Entities.Account>>();
            var tenantRepo = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            await accountRepo.InsertAsync(account, true);
            await tenantRepo.InsertAsync(tenant, true);
            var service = ActivatorUtilities.CreateInstance<AccountsService>(ServiceProvider);
            
            //act
            var accountNameOutput = await service.GetAccountInfoFromLicensingIdentifier(Guid.NewGuid());
            var result = accountNameOutput;

            //assert
            result.Should().BeNull();
        }
    }
}