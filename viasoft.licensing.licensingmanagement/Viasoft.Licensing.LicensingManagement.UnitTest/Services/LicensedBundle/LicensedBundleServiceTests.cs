using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Entities;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensedBundle;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.LicensedBundle
{
    public class LicensedBundleServiceTests: LicensingManagementTestBase
    {
        private static Guid TenantId => Guid.Parse("1BA7A91F-80C0-4975-842F-957A76504CE6");
        private static Guid LicensedBundleId => Guid.Parse("32271794-D276-411B-B067-D0953C8C9B40");
        private static Guid LicensedTenantId => Guid.Parse("203361BD-0236-4F78-B3DC-0345DF7CBA03");
        private static Guid BundleId => Guid.Parse("5AEC9315-30B3-43EA-9115-3EF6995D06A7");

        [Fact(DisplayName = "Tenta remover pacote licenciado de uma licença")]
        public async Task TestRemoveLicensedBundleFromLicense()
        {
            var licensedBundle = new Domain.Entities.LicensedBundle
            {
                Id = LicensedBundleId,
                Status = LicensedBundleStatus.BundleActive,
                BundleId = Guid.NewGuid(),
                LicensingMode = LicensingModes.Offline,
                LicensingModel = LicensingModels.Named,
                TenantId = TenantId,
                LicensedTenantId = LicensedTenantId,
                NumberOfLicenses = 10,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null
            };
            var licensedTenant = new Domain.Entities.LicensedTenant
            {
                Id = LicensedTenantId,
                Identifier = Guid.NewGuid(),
                TenantId = TenantId
            };
            
            var licensedAppRepo = await GetRepo(new List<LicensedApp>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Status = LicensedAppStatus.AppActive,
                    AppId = Guid.NewGuid(),
                    ExpirationDateOfTemporaryLicenses = null,
                    LicensingMode = LicensingModes.Offline,
                    LicensingModel = LicensingModels.Named,
                    TenantId = TenantId,
                    LicensedBundleId = licensedBundle.BundleId,
                    LicensedTenantId = LicensedTenantId,
                    NumberOfLicenses = 10,
                    AdditionalNumberOfLicenses = 0,
                    NumberOfTemporaryLicenses = 0
                }
            });
            var namedUserBundleLicenseRepo = await GetRepo(new List<NamedUserBundleLicense>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    DeviceId = "DeviceId",
                    TenantId = TenantId,
                    LicensedBundleId = LicensedBundleId,
                    LicensedTenantId = LicensedTenantId,
                    NamedUserEmail = "admin@korp.com.br",
                    NamedUserId = Guid.NewGuid()
                }
            });
            var licensedBundleRepo = await GetRepo(new List<Domain.Entities.LicensedBundle>
            {
                licensedBundle
            });
            
            var service = new LicensedBundleService(licensedAppRepo, namedUserBundleLicenseRepo, licensedBundleRepo, null, null);
            var unitOfWork = ServiceProvider.GetService<IUnitOfWork>();

            using (unitOfWork!.Begin())
            {
                await service.RemoveLicensedBundleFromLicense(licensedTenant, licensedBundle);
                await unitOfWork.CompleteAsync();
            }

            var licensedBundleOutput = await licensedBundleRepo.FindAsync(licensedBundle.Id);
            var namedUserBundleLicensesOutput = await namedUserBundleLicenseRepo.Where(b => b.LicensedBundleId == licensedBundle.Id).ToListAsync();
            var licensedAppsOutput = await licensedAppRepo.Where(a => a.LicensedBundleId == licensedBundle.BundleId).ToListAsync();

            licensedBundleOutput.Should().BeNull();
            namedUserBundleLicensesOutput.Should().BeEmpty();
            licensedAppsOutput.Should().BeEmpty();
        }

        [Fact(DisplayName = "Tenta atualizar um pacote licenciado de uma licença")]
        public async Task TestUpdateLicensedBundle()
        {
            var licensedBundle = new Domain.Entities.LicensedBundle
            {
                Id = LicensedBundleId,
                Status = LicensedBundleStatus.BundleActive,
                BundleId = Guid.NewGuid(),
                LicensingMode = LicensingModes.Offline,
                LicensingModel = LicensingModels.Named,
                TenantId = TenantId,
                LicensedTenantId = LicensedTenantId,
                NumberOfLicenses = 10,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null
            };
            var licensedTenant = new Domain.Entities.LicensedTenant
            {
                Id = LicensedTenantId,
                Identifier = Guid.NewGuid(),
                TenantId = TenantId
            };
            
            var licensedAppRepo = await GetRepo(new List<LicensedApp>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Status = LicensedAppStatus.AppActive,
                    AppId = Guid.NewGuid(),
                    ExpirationDateOfTemporaryLicenses = null,
                    LicensingMode = LicensingModes.Offline,
                    LicensingModel = LicensingModels.Named,
                    TenantId = TenantId,
                    LicensedBundleId = licensedBundle.BundleId,
                    LicensedTenantId = LicensedTenantId,
                    NumberOfLicenses = 10,
                    AdditionalNumberOfLicenses = 0,
                    NumberOfTemporaryLicenses = 0
                }
            });
            var namedUserBundleLicenseRepo = await GetRepo(new List<NamedUserBundleLicense>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    DeviceId = "DeviceId",
                    TenantId = TenantId,
                    LicensedBundleId = LicensedBundleId,
                    LicensedTenantId = LicensedTenantId,
                    NamedUserEmail = "admin@korp.com.br",
                    NamedUserId = Guid.NewGuid()
                }
            });
            var licensedBundleRepo = await GetRepo(new List<Domain.Entities.LicensedBundle>
            {
                licensedBundle
            });
            
            var service = new LicensedBundleService(licensedAppRepo, namedUserBundleLicenseRepo, licensedBundleRepo, null, null);
            var unitOfWork = ServiceProvider.GetService<IUnitOfWork>();

            Domain.Entities.LicensedBundle output;
            
            using (unitOfWork!.Begin())
            {
                output = await service.UpdateLicensedBundle(licensedTenant, licensedBundle, new LicensedBundleUpdateInput
                {
                    Status = LicensedBundleStatus.BundleBlocked,
                    BundleId = licensedBundle.BundleId,
                    LicensedTenantId = LicensedTenantId,
                    NumberOfLicenses = 2,
                    NumberOfTemporaryLicenses = 2,
                    ExpirationDateOfTemporaryLicenses = DateTime.Parse("2021-01-01"),
                    LicensingMode = null,
                    LicensingModel = LicensingModels.Floating
                });
                await unitOfWork.CompleteAsync();
            }
            
            var licensedBundleOutput = await licensedBundleRepo.FindAsync(licensedBundle.Id);
            var namedUserBundleLicensesOutput = await namedUserBundleLicenseRepo.Where(b => b.LicensedBundleId == licensedBundle.Id).ToListAsync();
            var licensedAppsOutput = await licensedAppRepo.Where(a => a.LicensedBundleId == licensedBundle.BundleId).ToListAsync();

            licensedBundleOutput.Should().BeEquivalentTo(output);
            namedUserBundleLicensesOutput.Should().BeEmpty();
            licensedAppsOutput[0].NumberOfLicenses.Should().Be(2);
            licensedAppsOutput[0].NumberOfTemporaryLicenses = 2;
            licensedAppsOutput[0].ExpirationDateOfTemporaryLicenses.Should().Be(DateTime.Parse("2021-01-01"));
            licensedAppsOutput[0].LicensingMode.Should().Be(null);
            licensedAppsOutput[0].LicensingModel.Should().Be(LicensingModels.Floating);

        }

        [Fact]
        public async Task GetLicensedBundleById()
        {
            var licensedBundle = new Domain.Entities.LicensedBundle
            {
                Id = LicensedBundleId,
                Status = LicensedBundleStatus.BundleActive,
                BundleId = BundleId,
                LicensingMode = LicensingModes.Offline,
                LicensingModel = LicensingModels.Named,
                TenantId = TenantId,
                LicensedTenantId = LicensedTenantId,
                NumberOfLicenses = 10,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null
            };
            
            var licensedAppRepo = await GetRepo(new List<LicensedApp>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Status = LicensedAppStatus.AppActive,
                    AppId = Guid.NewGuid(),
                    ExpirationDateOfTemporaryLicenses = null,
                    LicensingMode = LicensingModes.Offline,
                    LicensingModel = LicensingModels.Named,
                    TenantId = TenantId,
                    LicensedBundleId = licensedBundle.BundleId,
                    LicensedTenantId = LicensedTenantId,
                    NumberOfLicenses = 10,
                    AdditionalNumberOfLicenses = 0,
                    NumberOfTemporaryLicenses = 0
                }
            });
            var namedUserBundleLicenseRepo = await GetRepo(new List<NamedUserBundleLicense>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    DeviceId = "DeviceId",
                    TenantId = TenantId,
                    LicensedBundleId = LicensedBundleId,
                    LicensedTenantId = LicensedTenantId,
                    NamedUserEmail = "admin@korp.com.br",
                    NamedUserId = Guid.NewGuid()
                }
            });
            var licensedBundleRepo = await GetRepo(new List<Domain.Entities.LicensedBundle>
            {
                licensedBundle
            });
            var bundleRepo = await GetRepo(new List<Bundle>
            {
                new()
                {
                    Id = BundleId,
                    Identifier = "Default",
                    Name = "Bundle",
                    IsActive = true,
                    IsCustom = true,
                }
            });
            
            var expectedOutput = new LicensedBundleOutput()
            {
                Id = BundleId,
                Identifier = "Default",
                Name = "Bundle",
                IsActive = true,
                IsCustom = true,
                LicensingMode = licensedBundle.LicensingMode,
                LicensingModel = licensedBundle.LicensingModel,
                NumberOfLicenses = licensedBundle.NumberOfLicenses,
                NumberOfTemporaryLicenses = licensedBundle.NumberOfTemporaryLicenses,
                ExpirationDateOfTemporaryLicenses = licensedBundle.ExpirationDateOfTemporaryLicenses,
                LicensedBundleId = licensedBundle.Id,
                Status = licensedBundle.Status,
                NumberOfUsedLicenses = 1
            };
            
            var service = new LicensedBundleService(licensedAppRepo, namedUserBundleLicenseRepo, licensedBundleRepo, null, bundleRepo);
            var output = await service.GetLicensedBundleById(LicensedBundleId);
            
            output.Should().BeEquivalentTo(expectedOutput);
        }

        private async Task<IRepository<T>> GetRepo<T>(List<T> items) where T: Entity
        {
            var repo = ServiceProvider.GetService<IRepository<T>>();

            foreach (var item in items)
            {
                await repo!.InsertAsync(item, true);
            }

            return repo;
        }
    }
}