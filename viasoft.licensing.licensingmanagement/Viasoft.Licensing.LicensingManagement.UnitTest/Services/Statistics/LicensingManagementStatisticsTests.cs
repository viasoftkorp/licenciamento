using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Services.Statistics;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.Statistics
{
    public class LicensingManagementStatisticsTests: LicensingManagementTestBase
    {
        [Fact]
        public async Task MustReturnTheCorrectAmountOfAppsInTotalWithNoLicensingIdentifier()
        {
            //arrange
            var tenant = Guid.NewGuid();
            var licensedApps = new List<LicensedApp>
            {
                new LicensedApp
                {
                    Id = Guid.NewGuid(),
                    Status = LicensedAppStatus.AppActive,
                    AppId = Guid.NewGuid(),
                    NumberOfTemporaryLicenses = 10,
                    NumberOfLicenses = 2000,
                    AdditionalNumberOfLicenses = 10,
                    LicensedTenantId = Guid.NewGuid(),
                    TenantId = tenant
                },
                new LicensedApp
                {
                    Id = Guid.NewGuid(),
                    Status = LicensedAppStatus.AppActive,
                    AppId = Guid.NewGuid(),
                    NumberOfTemporaryLicenses = 10,
                    NumberOfLicenses = 2000,
                    AdditionalNumberOfLicenses = 10,
                    LicensedTenantId = Guid.NewGuid(),
                    TenantId = tenant
                }
            };
            var repo = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var service = ActivatorUtilities.CreateInstance<LicenseManagementStatisticsService>(ServiceProvider);
            
            //act
            var totalAppsZero = await service.GetNumberOfAppsInTotal();
            await repo.InsertAsync(licensedApps[0], true);
            var totalAppsOne = await service.GetNumberOfAppsInTotal();
            await repo.InsertAsync(licensedApps[1], true);
            var totalAppsTwo = await service.GetNumberOfAppsInTotal();
            
            //assert
            totalAppsZero.NumberOfAppsTotal.Should().Be(0);
            totalAppsOne.NumberOfAppsTotal.Should().Be(1);
            totalAppsTwo.NumberOfAppsTotal.Should().Be(2);
        }
    }
}