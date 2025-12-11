using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.Seeder;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Seeders
{
    public class LicensingStatusSeederTest : LicensingManagementTestBase
    {
        private static Guid LicensedTenantId => Guid.Parse("C542FFD2-0DA0-4497-A162-41ADE60F2B05");
        private static Guid TenantId => Guid.Parse("06BB09D8-40E7-4CF3-89D2-EA35D20BAC2C");
        
        [Fact]
        public async Task ShouldUpdateStatusToBlocked()
        {
            var licensedTenantsRepo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var unitOfWork = ServiceProvider.GetService<IUnitOfWork>();
            
            var tenant = new LicensedTenant
            {
                Id = LicensedTenantId,
                TenantId = TenantId,
                Status = 0
            };

            await licensedTenantsRepo.InsertAsync(tenant, true);
            
            var seeder = new LicensingStatusSeeder(licensedTenantsRepo, unitOfWork);
            
            await seeder.SeedDataAsync();
            
            var licensedTenant = licensedTenantsRepo.First(lt => lt.Id == LicensedTenantId);
            licensedTenant.Status.Should().Be(LicensingStatus.Blocked);
        }
    }
}