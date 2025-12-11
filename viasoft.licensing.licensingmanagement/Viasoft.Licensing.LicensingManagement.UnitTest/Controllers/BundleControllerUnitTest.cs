using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Software;
using Viasoft.Licensing.LicensingManagement.Host.Controllers;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Controllers
{
    public class BundleControllerUnitTest: LicensingManagementTestBase
    {
        
        [Fact(DisplayName = "Testa o retorno de uma busca por bundles licenciados e suas chamadas a serviços externos")]
        public async Task Test_Get_All_Licensed_Bundles_Method_Returns_And_Calls()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var softwareId = Guid.NewGuid();
            var licensedBundleId = Guid.NewGuid();

            var bundleRepo = ServiceProvider.GetRequiredService<IRepository<Bundle>>();
            var licensedBundleRepo = ServiceProvider.GetRequiredService<IRepository<LicensedBundle>>();
            var softwareRepo = ServiceProvider.GetRequiredService<IRepository<Software>>();
            var namedUserBundleLicenseRepo = ServiceProvider.GetRequiredService<IRepository<NamedUserBundleLicense>>();

            await licensedBundleRepo.InsertAsync(new LicensedBundle
            {
                BundleId = bundleId,
                Id = licensedBundleId,
                Status = LicensedBundleStatus.BundleActive,
                LicensingMode = null,
                LicensingModel = LicensingModels.Floating,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 10,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null
            }, true);

            await bundleRepo.InsertAsync(new Bundle
            {
                Id = bundleId,
                Identifier = "BND",
                Name = "Bundle",
                IsActive = true,
                IsCustom = false,
                SoftwareId = softwareId
            }, true);

            await softwareRepo.InsertAsync(new Software
            {
                Id = softwareId,
                Name = "Software"
            }, true);

            await namedUserBundleLicenseRepo.InsertAsync(new NamedUserBundleLicense()
            {
                LicensedBundleId = licensedBundleId,
                NamedUserEmail = string.Empty
            }, true);
            var input = new GetAllLicensedBundleInput
            {
                SkipCount = 0, 
                Filter = null, 
                Sorting = null, 
                AdvancedFilter = null, 
                MaxResultCount = 25,
                LicensedTenantId = licensedTenantId
            };
            var expectedOutput = new PagedResultDto<LicensedBundleOutput> { TotalCount = 1, Items = new List<LicensedBundleOutput> { new()
            {
                Id = bundleId,
                SoftwareId = softwareId,
                SoftwareName = "Software",
                Identifier = "BND",
                Name = "Bundle",
                IsActive = true,
                IsCustom = false,
                LicensingMode = null,
                LicensingModel = LicensingModels.Floating,
                NumberOfLicenses = 10,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null,
                LicensedBundleId = licensedBundleId,
                Status = LicensedBundleStatus.BundleActive,
                NumberOfUsedLicenses = 1
            }}};
            var controller = new BundleController(bundleRepo, null, null, null, null, licensedBundleRepo, null, softwareRepo, namedUserBundleLicenseRepo);
            // execute
            var result = await controller.GetAllLicensedBundle(input);
            // TEST RETURN
            expectedOutput.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa o retorno de uma busca por bundles licenciados e suas chamadas a serviços externos quando temos um filtro")]
        public async Task Test_Get_All_Licensed_Bundles_Method_Returns_And_Calls_When_Have_Filter()
        {
            // prepare data
            var bundleIds = new List<Guid>{Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};
            var licensedTenantId = Guid.NewGuid();
            var softwareId = Guid.NewGuid();
            var licensedBundleId = Guid.NewGuid();
            
            var bundleRepo = ServiceProvider.GetRequiredService<IRepository<Bundle>>();
            var licensedBundleRepo = ServiceProvider.GetRequiredService<IRepository<LicensedBundle>>();
            var softwareRepo = ServiceProvider.GetRequiredService<IRepository<Software>>();
            var namedUserBundleLicenseRepo = ServiceProvider.GetRequiredService<IRepository<NamedUserBundleLicense>>();

            await licensedBundleRepo.InsertAsync(new LicensedBundle
            {
                BundleId = bundleIds[0],
                Id = Guid.NewGuid(),
                Status = LicensedBundleStatus.BundleActive,
                LicensingMode = null,
                LicensingModel = LicensingModels.Floating,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 10,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null
            }, true);

            await licensedBundleRepo.InsertAsync(new LicensedBundle
            {
                Id = licensedBundleId,
                Status = LicensedBundleStatus.BundleActive,
                BundleId = bundleIds[1],
                LicensingMode = null,
                LicensingModel = LicensingModels.Floating,
                NumberOfLicenses = 55,
                NumberOfTemporaryLicenses = 2,
                ExpirationDateOfTemporaryLicenses = null,
                LicensedTenantId = licensedTenantId
            }, true);

            await licensedBundleRepo.InsertAsync(new LicensedBundle
            {
                Id = Guid.NewGuid(),
                Status = LicensedBundleStatus.BundleActive,
                BundleId = bundleIds[2],
                LicensingMode = null,
                LicensingModel = LicensingModels.Floating,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 22,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null
            }, true);

            await bundleRepo.InsertAsync(new Bundle
            {
                Id = bundleIds[0],
                Identifier = "BND",
                Name = "Bundle",
                IsActive = true,
                IsCustom = false,
                SoftwareId = softwareId
            }, true);

            await bundleRepo.InsertAsync(new Bundle
            {
                Id = bundleIds[1],
                Identifier = "BNZ",
                Name = "BoomNZoom",
                IsActive = true,
                IsCustom = false,
                SoftwareId = softwareId
            }, true);

            await bundleRepo.InsertAsync(new Bundle
            {
                Id = bundleIds[2],
                Identifier = "TNB",
                Name = "TurnNBurn",
                IsActive = true,
                IsCustom = false,
                SoftwareId = softwareId
            });

            await softwareRepo.InsertAsync(new Software
            {
                Id = softwareId,
                Name = "Software"
            }, true);
            
            var expectedOutput = new PagedResultDto<LicensedBundleOutput> { TotalCount = 3, Items = new List<LicensedBundleOutput> { new LicensedBundleOutput
            {
                Id = bundleIds[1],
                SoftwareId = softwareId,
                SoftwareName = "Software",
                Identifier = "BNZ",
                Name = "BoomNZoom",
                IsActive = true,
                IsCustom = false,
                LicensingMode = null,
                LicensingModel = LicensingModels.Floating,
                NumberOfLicenses = 55,
                NumberOfTemporaryLicenses = 2,
                ExpirationDateOfTemporaryLicenses = null,
                LicensedBundleId = licensedBundleId,
                Status = LicensedBundleStatus.BundleActive,
                NumberOfUsedLicenses = 0
            }}};
            var controller = new BundleController(bundleRepo, null, null, null, null, licensedBundleRepo, null, softwareRepo, namedUserBundleLicenseRepo);

            var input = new GetAllLicensedBundleInput
            {
                SkipCount = 1,
                LicensedTenantId = licensedTenantId,
                MaxResultCount = 1
            };
            // execute
            var result = await controller.GetAllLicensedBundle(input);
            // TEST RETURN
            expectedOutput.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa o retorno de uma busca por bundles licenciados e suas chamadas a serviços externos quando não existem dados")]
        public async Task Test_Get_All_Licensed_Bundles_Method_Returns_And_Calls_When_No_Have_Data()
        {
            // prepare data
            var licensedTenantId = Guid.NewGuid();
            var softwareNameDictionary = new Dictionary<Guid,string>();
            var mockSoftwareRepository = new Mock<ISoftwareRepository>();
            mockSoftwareRepository
                .Setup(method => method.GetSoftwareNamesFromIdList(new List<Guid>())).ReturnsAsync(softwareNameDictionary);
            var input = new GetAllLicensedBundleInput
            {
                SkipCount = 0, 
                Filter = null, 
                Sorting = null, 
                AdvancedFilter = null, 
                MaxResultCount = 25,
                LicensedTenantId = licensedTenantId
            };
            var expectedOutput = new PagedResultDto<LicensedBundleOutput> { TotalCount = 0, Items = new List<LicensedBundleOutput>()};
            var controller = GetController(mockSoftwareRepository.Object);
            // execute
            var result = await controller.GetAllLicensedBundle(input);
            // TEST RETURN
            expectedOutput.Should().BeEquivalentTo(result);
        }
        

        private BundleController GetController(ISoftwareRepository softwareRepository)
        {
            return ActivatorUtilities.CreateInstance<BundleController>(ServiceProvider, softwareRepository);
        }

        private async Task<List<LicensedBundle>> CreateLicensedBundles(List<Guid> bundlesIds, Guid licensedTenantId)
        {
            var output = new List<LicensedBundle>();
            var memoryRepository = ServiceProvider.GetService<IRepository<LicensedBundle>>();
            foreach (var bundleId in bundlesIds)
            {
                var newLicensedBundle = new LicensedBundle
                {
                    Id = Guid.NewGuid(),
                    LicensedTenantId = licensedTenantId,
                    BundleId = bundleId
                };
                await memoryRepository.InsertAsync(newLicensedBundle, true);
                output.Add(newLicensedBundle);
            }

            return output;
        }
        
        private async Task<List<Bundle>> CreateBundles(List<Guid> bundleIds)
        {
            var output = new List<Bundle>();
            var memoryRepository = ServiceProvider.GetService<IRepository<Bundle>>();
            foreach (var bundleId in bundleIds)
            {
                var newBundle = new Bundle
                {
                    Id = bundleId,
                    SoftwareId = Guid.NewGuid()
                };
                await memoryRepository.InsertAsync(newBundle, true);
                output.Add(newBundle);
            }

            return output;
        }
    }
}