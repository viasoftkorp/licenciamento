using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.Services.HostTenant;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.TenantIdSearch
{
    public class TenantIdSearchTests : LicensingManagementTestBase
    {
        [Fact]
        public async Task MustReturnTenantIdFromLicensingIdentifier()
        {
            // arrange
            var licensedTenants = new List<Domain.Entities.LicensedTenant>
            {
                new Domain.Entities.LicensedTenant
                {
                    TenantId = Guid.NewGuid(),
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "adm@adm.com.br",
                    LicensedCnpjs = "12446117000182",
                    LicensedCnpjList = {"12446117000182"}
                },
                new Domain.Entities.LicensedTenant
                {
                    TenantId = Guid.NewGuid(),
                    Id = Guid.NewGuid(),
                    Identifier = Guid.NewGuid(),
                    Notes = null,
                    Status = LicensingStatus.Active,
                    AccountId = Guid.NewGuid(),
                    AdministratorEmail = "aaa@aaa.com.br",
                    LicensedCnpjs = "6662135213123",
                    LicensedCnpjList = {"6662135213123"}
                }
            };
            var repo = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            foreach (var tenant in licensedTenants)
            {
                await repo.InsertAsync(tenant, true);
            }
            var service = ActivatorUtilities.CreateInstance<HostTenantService>(ServiceProvider);
            
            // act
            var tenantIdSearchOutput = await service.GetHostTenantIdFromLicensingIdentifier(licensedTenants[0].Identifier);

            // assert
            var result = tenantIdSearchOutput.TenantId;
            Assert.Equal(result, licensedTenants[0].TenantId);
        }
    }
}