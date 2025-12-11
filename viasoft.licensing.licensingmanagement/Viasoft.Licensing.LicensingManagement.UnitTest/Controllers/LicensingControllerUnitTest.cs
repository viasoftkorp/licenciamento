using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;
using Viasoft.Licensing.LicensingManagement.Host.Controllers;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Controllers
{
    public class LicensingControllerUnitTest: LicensingManagementTestBase
    {
        [Fact(DisplayName = "Testa o retorno de uma busca por apps licenciados e suas chamadas a serviços externos")]
        public async Task Test_Get_All_Licensed_Loose_Apps_Method_Returns_And_Calls()
        {
            // prepare data
            var appId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var softwareId = Guid.NewGuid();
            var licensedAppId = Guid.NewGuid();

            var licensedAppRepo = ServiceProvider.GetRequiredService<IRepository<LicensedApp>>();
            var appRepo = ServiceProvider.GetRequiredService<IRepository<App>>();
            var softwareRepo = ServiceProvider.GetRequiredService<IRepository<Software>>();

            await licensedAppRepo.InsertAsync(new LicensedApp
            {
                Id = licensedAppId,
                Status = LicensedAppStatus.AppActive,
                AppId = appId,
                LicensingMode = null,
                LicensingModel = LicensingModels.Floating,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 10,
                AdditionalNumberOfLicenses = 0,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null
            }, true);

            await appRepo.InsertAsync(new App
            {
                Id = appId,
                Default = false,
                Domain = Domain.Enums.Domain.Accounting,
                Identifier = "APP",
                Name = "application",
                IsActive = true,
                SoftwareId = softwareId
            }, true);

            await softwareRepo.InsertAsync(new Software
            {
                Id = softwareId,
                Name = "software"
            }, true);

            var input = new GetAllLooseLicensedAppInput
            {
                SkipCount = 0, 
                Filter = null, 
                Sorting = null, 
                AdvancedFilter = null, 
                MaxResultCount = 25,
                LicensedTenantId = licensedTenantId
            };
            
            var expectedOutput = new PagedResultDto<LooseLicensedAppOutput> { TotalCount = 1, Items = new List<LooseLicensedAppOutput> { new LooseLicensedAppOutput
            {
                Id = appId,
                SoftwareId = softwareId,
                SoftwareName = "software",
                Domain = Domain.Enums.Domain.Accounting,
                Name = "application",
                Status = LicensedAppStatus.AppActive,
                LicensingMode = null,
                LicensingModel = LicensingModels.Floating,
                NumberOfLicenses = 10,
                AdditionalNumberOfLicenses = 0,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null,
                LicensedAppId = licensedAppId
            }}};
            var controller = new LicensingController(null, null, null, licensedAppRepo, appRepo, null, null, null, softwareRepo);
            // execute
            var result = await controller.GetAllLooseLicensedApps(input);
            // TEST RETURN
            expectedOutput.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa o retorno de uma busca por apps licenciados e suas chamadas a serviços externos quando temos um filtro")]
        public async Task Test_Get_All_Licensed_Loose_Apps_Method_Returns_And_Calls_When_Have_Filter()
        {
            // prepare data
            var appIds = new List<Guid>{Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};
            var licensedAppId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var softwareId = Guid.NewGuid();

            var licensedAppRepo = ServiceProvider.GetRequiredService<IRepository<LicensedApp>>();
            var appRepo = ServiceProvider.GetRequiredService<IRepository<App>>();
            var softwareRepo = ServiceProvider.GetRequiredService<IRepository<Software>>();

            var licensedApps = new List<LicensedApp>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Status = LicensedAppStatus.AppActive,
                    AppId = appIds[0],
                    LicensingMode = null,
                    LicensingModel = LicensingModels.Floating,
                    LicensedTenantId = licensedTenantId,
                    NumberOfLicenses = 10,
                    NumberOfTemporaryLicenses = 55,
                    ExpirationDateOfTemporaryLicenses = null
                },
                new()
                {
                    Id = licensedAppId,
                    Status = LicensedAppStatus.AppActive,
                    AppId = appIds[1],
                    LicensingMode = null,
                    LicensingModel = LicensingModels.Floating,
                    LicensedTenantId = licensedTenantId,
                    NumberOfLicenses = 2,
                    ExpirationDateOfTemporaryLicenses = null,
                    NumberOfTemporaryLicenses = 0,
                    AdditionalNumberOfLicenses = 0
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Status = LicensedAppStatus.AppActive,
                    AppId = appIds[2],
                    LicensingMode = null,
                    LicensingModel = LicensingModels.Floating,
                    LicensedTenantId = licensedTenantId,
                    NumberOfLicenses = 89,
                    ExpirationDateOfTemporaryLicenses = null,
                    NumberOfTemporaryLicenses = 0,
                    AdditionalNumberOfLicenses = 0
                }
            };

            foreach (var licensedApp in licensedApps)
            {
                await licensedAppRepo.InsertAsync(licensedApp, true);
            }

            var apps = new List<App>
            {
                new()
                {
                    Default = false,
                    Domain = Domain.Enums.Domain.Accounting,
                    Id = appIds[0],
                    Identifier = "APP0",
                    Name = "application0",
                    IsActive = true,
                    SoftwareId = softwareId
                },
                new()
                {
                    Default = false,
                    Domain = Domain.Enums.Domain.Billing,
                    Id = appIds[1],
                    Identifier = "APP1",
                    Name = "application1",
                    IsActive = true,
                    SoftwareId = softwareId
                },
                new()
                {
                    Default = false,
                    Domain = Domain.Enums.Domain.Configurations,
                    Id = appIds[2],
                    Identifier = "APP2",
                    Name = "application2",
                    IsActive = true,
                    SoftwareId = softwareId
                }
            };

            foreach (var app in apps)
            {
                await appRepo.InsertAsync(app, true);
            }

            await softwareRepo.InsertAsync(new Software
            {
                Id = softwareId,
                Name = "software"
            }, true);
            
            var input = new GetAllLooseLicensedAppInput
            {
                SkipCount = 1, 
                Filter = null, 
                Sorting = null, 
                AdvancedFilter = null, 
                MaxResultCount = 1,
                LicensedTenantId = licensedTenantId
            };
            var expectedOutput = new PagedResultDto<LooseLicensedAppOutput> { TotalCount = 3, Items = new List<LooseLicensedAppOutput> { new()
            {
                Id = apps[1].Id,
                SoftwareId = softwareId,
                SoftwareName = "software",
                Domain = apps[1].Domain,
                Name = apps[1].Name,
                Status = licensedApps[1].Status,
                LicensingMode = licensedApps[1].LicensingMode,
                LicensingModel = licensedApps[1].LicensingModel,
                NumberOfLicenses = licensedApps[1].NumberOfLicenses,
                AdditionalNumberOfLicenses = licensedApps[1].AdditionalNumberOfLicenses,
                NumberOfTemporaryLicenses = licensedApps[1].NumberOfTemporaryLicenses,
                ExpirationDateOfTemporaryLicenses = null,
                LicensedAppId = licensedAppId
            }}};
            var controller = new LicensingController(null, null, null, licensedAppRepo, appRepo, null, null, null, softwareRepo);
            // execute
            var result = await controller.GetAllLooseLicensedApps(input);
            // TEST RETURN
            expectedOutput.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa o retorno de uma busca por apps licenciados e suas chamadas a serviços externos quando não existem dados")]
        public async Task Test_Get_All_Licensed_Loose_Apps_Method_Returns_And_Calls_When_No_Have_Data()
        {
            // prepare data
            var licensedTenantId = Guid.NewGuid();

            var licensedAppRepo = ServiceProvider.GetRequiredService<IRepository<LicensedApp>>();
            var appRepo = ServiceProvider.GetRequiredService<IRepository<App>>();
            var softwareRepo = ServiceProvider.GetRequiredService<IRepository<Software>>();
            
            var input = new GetAllLooseLicensedAppInput
            {
                SkipCount = 0, 
                Filter = null, 
                Sorting = null, 
                AdvancedFilter = null, 
                MaxResultCount = 25,
                LicensedTenantId = licensedTenantId
            };
            var expectedOutput = new PagedResultDto<LooseLicensedAppOutput> { TotalCount = 0, Items = new List<LooseLicensedAppOutput>()};
            var controller = new LicensingController(null, null, null, licensedAppRepo, appRepo, null, null, null, softwareRepo);
            // execute
            var result = await controller.GetAllLooseLicensedApps(input);
            // TEST RETURN
            expectedOutput.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa o retorno e as chamadas realizadas ao criar uma nova licença")]
        public async Task Test_Create_New_License_Method_Returns_And_Calls()
        {
            // prepare data
            var input = new LicenseTenantCreateInput
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid(),
                Status = LicensingStatus.Active,
                AccountId = Guid.NewGuid(),
                AdministratorEmail = "admin@korp",
                Notes = "",
                LicensedCnpjs = "",
                LicenseConsumeType = LicenseConsumeType.Access
            };
            var expectedOutput = new LicenseTenantCreateOutput {Notes = "fui testado corretamente"};
            var mockLicensedTenantService = new Mock<ILicensedTenantService>();
            mockLicensedTenantService.Setup(r => r.CreateNewTenantLicensing(input, true))
                .ReturnsAsync(expectedOutput);
            
            var controller = GetController(mockLicensedTenantService.Object);
            // execute
            var result = await controller.Create(input);
            // TEST RETURN
            expectedOutput.Should().BeEquivalentTo(result);
            // TEST METHOD CALLS COUNT
            mockLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.CreateNewTenantLicensing));
            // TEST ARGUMENTS IN METHOD
            mockLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.CreateNewTenantLicensing), input);
        }
        
        [Fact(DisplayName = "Testa o retorno e as chamadas realizadas ao atualizar uma licença")]
        public async Task Test_Update_License_Method_Returns_And_Calls()
        {
            // prepare data
            var input = new LicenseTenantUpdateInput
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid(),
                Status = LicensingStatus.Active,
                AccountId = Guid.NewGuid(),
                AdministratorEmail = "admin@korp",
                Notes = "",
                LicensedCnpjs = "",
                LicenseConsumeType = LicenseConsumeType.Access
            };
            var expectedOutput = new LicenseTenantUpdateOutput {Notes = "fui testado corretamente"};
            var mockLicensedTenantService = new Mock<ILicensedTenantService>();
            mockLicensedTenantService.Setup(r => r.UpdateTenantLicensing(input, null))
                .ReturnsAsync(expectedOutput);
            
            var controller = GetController(mockLicensedTenantService.Object);
            // execute
            var result = await controller.Update(input);
            // TEST RETURN
            expectedOutput.Should().BeEquivalentTo(result);
            // TEST METHOD CALLS COUNT
            mockLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.UpdateTenantLicensing));
            // TEST ARGUMENTS IN METHOD
            mockLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.UpdateTenantLicensing), input);
        }
        
        private LicensingController GetController(ILicensedTenantService licensedTenantService)
        {
            var mockLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            return ActivatorUtilities.CreateInstance<LicensingController>(ServiceProvider, licensedTenantService, mockLicensedTenantCacheService.Object);
        }

        private async Task<List<LicensedApp>> CreateLicensedApps(List<Guid> licensedAppIds, Guid licensedTenantId)
        {
            var output = new List<LicensedApp>();
            var memoryRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            foreach (var licensedAppId in licensedAppIds)
            {
                var newLicensedApp = new LicensedApp
                {
                    Id = Guid.NewGuid(),
                    LicensedTenantId = licensedTenantId,
                    AppId = licensedAppId
                };
                await memoryRepository.InsertAsync(newLicensedApp, true);
                output.Add(newLicensedApp);
            }

            return output;
        }
        
        private async Task<List<App>> CreateApps(List<Guid> appsIds)
        {
            var output = new List<App>();
            var memoryRepository = ServiceProvider.GetService<IRepository<App>>();
            foreach (var appsId in appsIds)
            {
                var newApp = new App
                {
                    Id = appsId,
                    SoftwareId = Guid.NewGuid()
                };
                await memoryRepository.InsertAsync(newApp, true);
                output.Add(newApp);
            }

            return output;
        }
    }
}