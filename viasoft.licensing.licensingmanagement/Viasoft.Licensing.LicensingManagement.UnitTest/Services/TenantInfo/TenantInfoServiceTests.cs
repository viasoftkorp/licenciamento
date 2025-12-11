using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantInfo;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.TenantInfo
{
    public class TenantInfoServiceTests: LicensingManagementTestBase
    {
        [Fact]
        public async Task MustReturnTenantInfo()
        {
            //arrange
            var licensedTenants = new List<Domain.Entities.LicensedTenant>
            {
                new Domain.Entities.LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste@teste.com.br",
                    Notes = null,
                    LicensedCnpjs = "05044558000192",
                    LicensedCnpjList = {"05044558000192"},
                    LicenseConsumeType = LicenseConsumeType.Access,
                    TenantId = Guid.NewGuid()
                },
                new Domain.Entities.LicensedTenant
                {
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Blocked,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "teste2@teste2.com.br",
                    LicensedCnpjs = "20760155000133",
                    TenantId = Guid.NewGuid(),
                    LicensedCnpjList = {"20760155000133"},
                    LicenseConsumeType = LicenseConsumeType.Access
                }
            };
            var repo = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            var service = ActivatorUtilities.CreateInstance<TenantInfoService>(ServiceProvider);
            foreach (var tenant in licensedTenants)
            {
                await repo.InsertAsync(tenant, true);
            }
            
            //act
            var infoOne = await service.GetTenantInfoFromLicensingIdentifier(licensedTenants[0].Identifier);
            var infoTwo = await service.GetTenantInfoFromLicensingIdentifier(licensedTenants[1].Identifier);
            var noInfo = await service.GetTenantInfoFromLicensingIdentifier(Guid.NewGuid());
            
            //assert
            
            infoOne.Should().BeEquivalentTo(new
            {
                Cnpj = licensedTenants[0].LicensedCnpjs,
                LicensingStatus = licensedTenants[0].Status
            });
            infoTwo.Should().BeEquivalentTo(new
            {
                Cnpj = licensedTenants[1].LicensedCnpjs,
                LicensingStatus = licensedTenants[1].Status
            });
            noInfo.OperationValidation.Should().Be(OperationValidation.NoTenantWithSuchId);
        }
    }
}