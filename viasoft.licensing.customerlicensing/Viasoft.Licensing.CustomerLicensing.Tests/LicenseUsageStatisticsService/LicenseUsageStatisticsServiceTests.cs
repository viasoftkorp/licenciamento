using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Viasoft.Core.ApiClient;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DynamicLinqQueryBuilder;
using Viasoft.Licensing.CustomerLicensing.Domain.Contracts.LicensingManagement;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageStatistics;
using Viasoft.Licensing.CustomerLicensing.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.UnitTest;
using Xunit;

namespace Viasoft.Licensing.CustomerLicensing.Tests.LicenseUsageStatisticsService
{
    public class LicenseUsageStatisticsServiceTests : CustomerLicensingTestBase
    {
        [Fact]
        public async Task MustReturnTheCorrectAmountOfOnlineTenants()
        {
            //arrange
            var tenants = new List<LicenseUsageInRealTime>
            {
                new()
                {
                    Cnpj = "28793635000102",
                    Language = "pt-br",
                    User = "Mario",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Mario's Account",
                    AdditionalLicense = false,
                    AdditionalLicenses = 0,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Mario's app",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingIdentifier = Guid.NewGuid(),
                    LicensingStatus = LicensingStatus.Active,
                    SoftwareIdentifier = "web",
                    SoftwareName = "Software web",
                    SoftwareVersion = "1.0"
                },
                new()
                {
                    Cnpj = "81339664000181",
                    Language = "pt-br",
                    User = "Luigi",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Luigi's Account",
                    AdditionalLicense = true,
                    AdditionalLicenses = 100,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Luigi's App",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingStatus = LicensingStatus.Active,
                    LicensingIdentifier = Guid.NewGuid(),
                    SoftwareIdentifier = "WEB",
                    SoftwareName = "Web Software",
                    SoftwareVersion = "1.0"
                }
            };
            var repo = ServiceProvider.GetService<IRepository<LicenseUsageInRealTime>>();
            var fakeApiGatewayCall = new Mock<IApiClientCallBuilder>();
            var service = ActivatorUtilities
                .CreateInstance<Domain.Services.LicenseUsageStatistics.LicenseUsageStatisticsService>(ServiceProvider, fakeApiGatewayCall.Object);

            // act
            var onlineTenantsOutputZero = await service.GetOnlineTenantCountAsync();
            await repo.InsertAsync(tenants[0], true);
            var onlineTenantsOutputOne = await service.GetOnlineTenantCountAsync();
            await repo.InsertAsync(tenants[1], true);
            var onlineTenantsOutputTwo = await service.GetOnlineTenantCountAsync();


            //assert
            onlineTenantsOutputZero.TenantCount.Should().Be(0);
            onlineTenantsOutputOne.TenantCount.Should().Be(1);
            onlineTenantsOutputTwo.TenantCount.Should().Be(2);
        }

        [Fact]
        public async Task MustReturnTheCorrectAmountOfUsersOnline()
        {
            //arrange
            var users = new List<LicenseUsageInRealTime>
            {
                new()
                {
                    Cnpj = "28793635000102",
                    Language = "pt-br",
                    User = "Mario",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Mario's Account",
                    AdditionalLicense = false,
                    AdditionalLicenses = 0,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Mario's app",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingIdentifier = Guid.NewGuid(),
                    LicensingStatus = LicensingStatus.Active,
                    SoftwareIdentifier = "web",
                    SoftwareName = "Software web",
                    SoftwareVersion = "1.0"
                },
                new()
                {
                    Cnpj = "81339664000181",
                    Language = "pt-br",
                    User = "Luigi",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Luigi's Account",
                    AdditionalLicense = true,
                    AdditionalLicenses = 100,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Luigi's App",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingStatus = LicensingStatus.Active,
                    LicensingIdentifier = Guid.NewGuid(),
                    SoftwareIdentifier = "WEB",
                    SoftwareName = "Web Software",
                    SoftwareVersion = "1.0"
                }
            };
            var repo = ServiceProvider.GetService<IRepository<LicenseUsageInRealTime>>();
            var fakeApiGatewayCall = new Mock<IApiClientCallBuilder>();
            var service = ActivatorUtilities
                .CreateInstance<Domain.Services.LicenseUsageStatistics.LicenseUsageStatisticsService>(ServiceProvider, fakeApiGatewayCall.Object);

            // act
            var onlineUsersOutputZero = await service.GetOnlineUserCountAsync(null);
            await repo.InsertAsync(users[0], true);
            var onlineUsersOutputOne = await service.GetOnlineUserCountAsync(null);
            await repo.InsertAsync(users[1], true);
            var onlineUsersOutputTwo = await service.GetOnlineUserCountAsync(null);
            
            //arrange
            onlineUsersOutputZero.OnlineUserCount.Should().Be(0);
            onlineUsersOutputOne.OnlineUserCount.Should().Be(1);
            onlineUsersOutputTwo.OnlineUserCount.Should().Be(2);
        }

        [Fact]
        public async Task MustReturnTheCorrectAmountOfUsersOnlineWithAdvancedFilter()
        {
            //arrange
            var users = new List<LicenseUsageInRealTime>
            {
                new()
                {
                    Cnpj = "28793635000102",
                    Language = "pt-br",
                    User = "Mario",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Mario's Account",
                    AdditionalLicense = false,
                    AdditionalLicenses = 0,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Mario's app",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingIdentifier = Guid.NewGuid(),
                    LicensingStatus = LicensingStatus.Active,
                    SoftwareIdentifier = "web",
                    SoftwareName = "Software web",
                    SoftwareVersion = "2.0"
                },
                new()
                {
                    Cnpj = "81339664000181",
                    Language = "pt-br",
                    User = "Luigi",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Luigi's Account",
                    AdditionalLicense = true,
                    AdditionalLicenses = 100,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Luigi's App",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingStatus = LicensingStatus.Active,
                    LicensingIdentifier = Guid.NewGuid(),
                    SoftwareIdentifier = "WEB",
                    SoftwareName = "Web Software",
                    SoftwareVersion = "1.0"
                }
            };
            var repo = ServiceProvider.GetService<IRepository<LicenseUsageInRealTime>>();
            var fakeApiGatewayCall = new Mock<IApiClientCallBuilder>();
            var service = ActivatorUtilities
                .CreateInstance<Domain.Services.LicenseUsageStatistics.LicenseUsageStatisticsService>(ServiceProvider, fakeApiGatewayCall.Object);
            
            var advancedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new()
                    {
                        Field = "SoftwareVersion",
                        Operator = "equal",
                        Value = "2.0",
                        Type = "string"
                    }
                }
            };

            var serializedAdvancedFilter = JsonConvert.SerializeObject(advancedFilter, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            foreach (var user in users)
            {
                await repo.InsertAsync(user, true);
            }

            //act
            var onlineUsersCountOutput = await service.GetOnlineUserCountAsync(serializedAdvancedFilter);
            
            //assert
            onlineUsersCountOutput.OnlineUserCount.Should().Be(1);
        }

        [Fact]
        public async Task MustReturnTheCorrectAmountOfUsersOnlineWithLicensingIdentifier()
        {
            //arrange
            var users = new List<LicenseUsageInRealTime>
            {
                new()
                {
                    Cnpj = "28793635000102",
                    Language = "pt-br",
                    User = "Mario",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Mario's Account",
                    AdditionalLicense = false,
                    AdditionalLicenses = 0,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Mario's app",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingIdentifier = Guid.NewGuid(),
                    LicensingStatus = LicensingStatus.Active,
                    SoftwareIdentifier = "web",
                    SoftwareName = "Software web",
                    SoftwareVersion = "2.0"
                },
                new()
                {
                    Cnpj = "81339664000181",
                    Language = "pt-br",
                    User = "Luigi",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Luigi's Account",
                    AdditionalLicense = true,
                    AdditionalLicenses = 100,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Luigi's App",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingStatus = LicensingStatus.Active,
                    LicensingIdentifier = Guid.NewGuid(),
                    SoftwareIdentifier = "WEB",
                    SoftwareName = "Web Software",
                    SoftwareVersion = "1.0"
                }
            };
            var repo = ServiceProvider.GetService<IRepository<LicenseUsageInRealTime>>();
            var fakeApiGatewayCall = new Mock<IApiClientCallBuilder>();
            var service = ActivatorUtilities
                .CreateInstance<Domain.Services.LicenseUsageStatistics.LicenseUsageStatisticsService>(ServiceProvider, fakeApiGatewayCall.Object);

            foreach (var user in users)
            {
                await repo.InsertAsync(user, true);
            }
            
            //act
            var onlineUserCountOutput = await service.GetOnlineUserCountAsync(null, users[0].LicensingIdentifier);
            
            //assert
            onlineUserCountOutput.OnlineUserCount.Should().Be(1);
        }

        [Fact]
        public async Task MustReturnTheCorrectAmountOfAppsAndTotalNumberOfApps()
        {
            //arrange
            var apps = new List<LicenseUsageInRealTime>
            {
                new()
                {
                    Cnpj = "28793635000102",
                    Language = "pt-br",
                    User = "Mario",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Mario's Account",
                    AdditionalLicense = false,
                    AdditionalLicenses = 0,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Mario's app",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingIdentifier = Guid.NewGuid(),
                    LicensingStatus = LicensingStatus.Active,
                    SoftwareIdentifier = "web",
                    SoftwareName = "Software web",
                    SoftwareVersion = "2.0"
                },
                new()
                {
                    Cnpj = "81339664000181",
                    Language = "pt-br",
                    User = "Luigi",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Luigi's Account",
                    AdditionalLicense = true,
                    AdditionalLicenses = 100,
                    AppIdentifier = "APP2",
                    AppLicenses = 10,
                    AppName = "Luigi's App",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingStatus = LicensingStatus.Active,
                    LicensingIdentifier = Guid.NewGuid(),
                    SoftwareIdentifier = "WEB",
                    SoftwareName = "Web Software",
                    SoftwareVersion = "1.0"
                }
            };
            var repo = ServiceProvider.GetService<IRepository<LicenseUsageInRealTime>>();
            var service = ActivatorUtilities.CreateInstance<LicenseUsageStatisticsServiceMock>(ServiceProvider);

            // act
            var onlineAppsCountOutput = await service.GetAppsInUseCountAsync(null);
            await repo.InsertAsync(apps[0], true);
            var onlineAppsCountOutputOne = await service.GetAppsInUseCountAsync(null);
            await repo.InsertAsync(apps[1], true);
            var onlineAppsCountOutputTwo = await service.GetAppsInUseCountAsync(null);
            
            //arrange
            onlineAppsCountOutput.TotalApps.Should().Be(10);
            onlineAppsCountOutputOne.TotalApps.Should().Be(10);
            onlineAppsCountOutputTwo.TotalApps.Should().Be(10);
            onlineAppsCountOutput.AppsInUse.Should().Be(0);
            onlineAppsCountOutputOne.AppsInUse.Should().Be(1);
            onlineAppsCountOutputTwo.AppsInUse.Should().Be(2);
        }

        [Fact]
        public async Task MustReturnTheCorrectAmountOfAppsAndTotalNumberOfAppsWithAdvancedFilter()
        {
            //arrange
            var apps = new List<LicenseUsageInRealTime>
            {
                new()
                {
                    Cnpj = "28793635000102",
                    Language = "pt-br",
                    User = "Mario",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Mario's Account",
                    AdditionalLicense = false,
                    AdditionalLicenses = 0,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Mario's app",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingIdentifier = Guid.NewGuid(),
                    LicensingStatus = LicensingStatus.Active,
                    SoftwareIdentifier = "web",
                    SoftwareName = "Software web",
                    SoftwareVersion = "2.0"
                },
                new()
                {
                    Cnpj = "81339664000181",
                    Language = "pt-br",
                    User = "Luigi",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Luigi's Account",
                    AdditionalLicense = true,
                    AdditionalLicenses = 100,
                    AppIdentifier = "APP2",
                    AppLicenses = 10,
                    AppName = "Luigi's App",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingStatus = LicensingStatus.Active,
                    LicensingIdentifier = Guid.NewGuid(),
                    SoftwareIdentifier = "WEB",
                    SoftwareName = "Web Software",
                    SoftwareVersion = "1.0"
                }
            };
            var repo = ServiceProvider.GetService<IRepository<LicenseUsageInRealTime>>();
            var service = ActivatorUtilities.CreateInstance<LicenseUsageStatisticsServiceMock>(ServiceProvider);
            foreach (var app in apps)
            {
                await repo.InsertAsync(app, true);
            }
            
            var advancedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new()
                    {
                        Field = "SoftwareVersion",
                        Operator = "equal",
                        Value = "2.0",
                        Type = "string"
                    }
                }
            };

            var serializedAdvancedFilter = JsonConvert.SerializeObject(advancedFilter, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            // act
            var onlineAppsCountOutput = await service.GetAppsInUseCountAsync(serializedAdvancedFilter);
            
            //arrange
            onlineAppsCountOutput.TotalApps.Should().Be(10);
            onlineAppsCountOutput.AppsInUse.Should().Be(1);
        }

        [Fact]
        public async Task MustReturnTheCorrectAmountOfAppsAndTotalNumberOfAppsWithLicensingIdentifier()
        {
            //arrange
            var apps = new List<LicenseUsageInRealTime>
            {
                new()
                {
                    Cnpj = "28793635000102",
                    Language = "pt-br",
                    User = "Mario",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Mario's Account",
                    AdditionalLicense = false,
                    AdditionalLicenses = 0,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Mario's app",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingIdentifier = Guid.NewGuid(),
                    LicensingStatus = LicensingStatus.Active,
                    SoftwareIdentifier = "web",
                    SoftwareName = "Software web",
                    SoftwareVersion = "2.0"
                },
                new()
                {
                    Cnpj = "81339664000181",
                    Language = "pt-br",
                    User = "Luigi",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Luigi's Account",
                    AdditionalLicense = true,
                    AdditionalLicenses = 100,
                    AppIdentifier = "APP2",
                    AppLicenses = 10,
                    AppName = "Luigi's App",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingStatus = LicensingStatus.Active,
                    LicensingIdentifier = Guid.NewGuid(),
                    SoftwareIdentifier = "WEB",
                    SoftwareName = "Web Software",
                    SoftwareVersion = "1.0"
                }
            };
            var repo = ServiceProvider.GetService<IRepository<LicenseUsageInRealTime>>();
            var ownedAppRepo = ServiceProvider.GetService<IRepository<OwnedAppCount>>();
            var service = ActivatorUtilities.CreateInstance<LicenseUsageStatisticsServiceMock>(ServiceProvider);
            foreach (var app in apps)
            {
                await repo.InsertAsync(app, true);
            }

            await ownedAppRepo.InsertAsync(new OwnedAppCount
            {
                Count = 10,
                LicensingIdentifier = apps[0].LicensingIdentifier
            }, true);
            
            // act
            var onlineAppsCountOutput = await service.GetAppsInUseCountAsync(null, apps[0].LicensingIdentifier);
            
            //arrange
            onlineAppsCountOutput.TotalApps.Should().Be(10);
            onlineAppsCountOutput.AppsInUse.Should().Be(1);
        }

        [Fact]
        public async Task MustReturnTheCorrectLicenseIdentifiersFromAdvancedFilter()
        {
            //arrange
            var licenseIdentifier1 = Guid.NewGuid();
            var licenseIdentifier2 = Guid.NewGuid();
            var licenseUsage = new List<LicenseUsageInRealTime>
            {
                new()
                {
                    Cnpj = "28793635000102",
                    Language = "pt-br",
                    User = "Mario",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Mario's Account",
                    AdditionalLicense = false,
                    AdditionalLicenses = 0,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Mario's app",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingIdentifier = licenseIdentifier1,
                    LicensingStatus = LicensingStatus.Active,
                    SoftwareIdentifier = "web",
                    SoftwareName = "Software web",
                    SoftwareVersion = "2.0"
                },
                new()
                {
                    Cnpj = "287936353200102",
                    Language = "pt-br",
                    User = "Toad",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Toad's Account",
                    AdditionalLicense = false,
                    AdditionalLicenses = 0,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Toad's app",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingIdentifier = licenseIdentifier1,
                    LicensingStatus = LicensingStatus.Active,
                    SoftwareIdentifier = "web",
                    SoftwareName = "Software web",
                    SoftwareVersion = "2.0"
                },
                new()
                {
                    Cnpj = "81339664000181",
                    Language = "pt-br",
                    User = "Luigi",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Luigi's Account",
                    AdditionalLicense = true,
                    AdditionalLicenses = 100,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Luigi's App",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingStatus = LicensingStatus.Active,
                    LicensingIdentifier = licenseIdentifier2,
                    SoftwareIdentifier = "WEB",
                    SoftwareName = "Web Software",
                    SoftwareVersion = "1.0"
                }
            };
            var advancedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new()
                    {
                        Field = "User",
                        Operator = "equal",
                        Value = "Luigi",
                        Type = "string"
                    }
                }
            };
            var serializedAdvancedFilter = JsonConvert.SerializeObject(advancedFilter, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var fakeApiGatewayCall = new Mock<IApiClientCallBuilder>();
            var repo = ServiceProvider.GetService<IRepository<LicenseUsageInRealTime>>();
            foreach (var usage in licenseUsage)
            {
                await repo.InsertAsync(usage, true);
            }
            var expectedResult = new List<Guid>{ licenseIdentifier2 };
            var service = ActivatorUtilities
                .CreateInstance<Domain.Services.LicenseUsageStatistics.LicenseUsageStatisticsService>(ServiceProvider, fakeApiGatewayCall.Object);
            //act
            var licensesIdentifiers = await service.GetLicenseIdentifiersForUsageReporting(serializedAdvancedFilter);
            //assert
            licensesIdentifiers.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task MustReturnTheCorrectAmountOfLicenseUsageCount()
        {
            //arrange
            var licenseIdentifier1 = Guid.NewGuid();
            var licenseIdentifier2 = Guid.NewGuid();
            var users = new List<LicenseUsageInRealTime>
            {
                new()
                {
                    Cnpj = "28793635000102",
                    Language = "pt-br",
                    User = "Mario",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Mario's Account",
                    AdditionalLicense = false,
                    AdditionalLicenses = 0,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Mario's app",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingIdentifier = licenseIdentifier1,
                    LicensingStatus = LicensingStatus.Active,
                    SoftwareIdentifier = "web",
                    SoftwareName = "Software web",
                    SoftwareVersion = "2.0"
                },
                new()
                {
                    Cnpj = "287936353200102",
                    Language = "pt-br",
                    User = "Toad",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Toad's Account",
                    AdditionalLicense = false,
                    AdditionalLicenses = 0,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Toad's app",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingIdentifier = licenseIdentifier1,
                    LicensingStatus = LicensingStatus.Active,
                    SoftwareIdentifier = "web",
                    SoftwareName = "Software web",
                    SoftwareVersion = "2.0"
                },
                new()
                {
                    Cnpj = "81339664000181",
                    Language = "pt-br",
                    User = "Luigi",
                    AccountId = Guid.NewGuid(),
                    AccountName = "Luigi's Account",
                    AdditionalLicense = true,
                    AdditionalLicenses = 100,
                    AppIdentifier = "APP",
                    AppLicenses = 10,
                    AppName = "Luigi's App",
                    AppStatus = LicensedAppStatus.AppActive,
                    LicensingStatus = LicensingStatus.Active,
                    LicensingIdentifier = licenseIdentifier2,
                    SoftwareIdentifier = "WEB",
                    SoftwareName = "Web Software",
                    SoftwareVersion = "1.0"
                }
            };
            var expectedResult = new List<LicenseUsageReportingOutput>();
            expectedResult.Add(new LicenseUsageReportingOutput {LicensingIdentifier = licenseIdentifier1, UsageCount = 2});
            expectedResult.Add(new LicenseUsageReportingOutput {LicensingIdentifier = licenseIdentifier2, UsageCount = 1});
            var repo = ServiceProvider.GetService<IRepository<LicenseUsageInRealTime>>();
            var fakeApiGatewayCall = new Mock<IApiClientCallBuilder>();
            var service = ActivatorUtilities
                .CreateInstance<Domain.Services.LicenseUsageStatistics.LicenseUsageStatisticsService>(ServiceProvider, fakeApiGatewayCall.Object);

            foreach (var user in users)
            {
                await repo.InsertAsync(user, true);
            }
            
            //act
            var licensesInUsage = await service.GetLicenseUsageForReporting();
            //assert
            expectedResult.Should().BeEquivalentTo(licensesInUsage);
        }
    }
}