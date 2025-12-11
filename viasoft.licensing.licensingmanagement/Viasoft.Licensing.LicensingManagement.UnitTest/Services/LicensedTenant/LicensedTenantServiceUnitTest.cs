using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Event;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Validator;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.Command;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.App;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicenseServer;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.LicensedTenant
{
    public class LicensedTenantServiceUnitTest: LicensingManagementTestBase
    {

        [Fact(DisplayName = "Testa se foi adicionado corretamente um app a um licenciamento)")]
        public async Task Check_Added_App_In_License()
        {
            // prepare data
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicensedTenantCacheService.Object);
            var fakeLicensedBundle = GetLicensedBundle();
            var fakeAppId = Guid.NewGuid();
            // execute
            var appInserted = await service.AddBundledAppsToLicense(fakeAppId, fakeLicensedBundle);
            await UnitOfWork.SaveChangesAsync();
            // test
            var expectedMemorySingle = ServiceProvider.GetService<IRepository<LicensedApp>>().Single();
            appInserted.Should().BeEquivalentTo(expectedMemorySingle);
        }
        
        [Fact(DisplayName = "Testa as chamas quando um app é removido de varias licenças")]
        public async Task Remove_App_From_Licenses_Calls()
        {
            // prepare data
            var fakeIdInvoked = Guid.NewGuid();
            var fakeLicenseByTenantId = new LicenseByIdentifier()
            {
                Status = LicensingStatus.Active,
                Identifier = fakeIdInvoked,
                ExpirationDateTime = DateTime.UtcNow
            };
            var licensedTenantId = new List<Guid> {Guid.NewGuid()};
            var fakeLicensedApps = await CreateLicensedApps(licensedTenantId);
            var fakeDictionary = LicensedDictionary(licensedTenantId);

            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService
                .Setup(r => r.InvalidateCacheForTenants(new List<Guid>{fakeIdInvoked}));

            var fakeServiceBus = new Mock<IServiceBus>();
            fakeServiceBus
                .Setup(r => r.Publish(null, null));
            
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService
                .Setup(r => r.GetLicensesByIdentifiers(new List<Guid>{fakeDictionary.First().Value})).ReturnsAsync(new List<LicenseByIdentifier>{fakeLicenseByTenantId});
            
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicensedTenantCacheService.Object, fakeServiceBus.Object, fakeLicenseServerService.Object);
            // execute
            await service.RemoveAppsFromLicenses(fakeLicensedApps, fakeDictionary);
            // test
            // TEST METHOD CALLS COUNT
            fakeLicenseServerService.Invocations.AssertSingle(nameof(ILicenseServerService.GetLicensesByIdentifiers));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenants));
            fakeServiceBus.Invocations.AssertSingle(nameof(IServiceBus.Publish));
            // TEST INVOCATION PARAMS OF METHODS
            fakeLicenseServerService.Invocations.AssertArgument(0, nameof(ILicenseServerService.GetLicensesByIdentifiers), new List<Guid>{fakeDictionary.First().Value});
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenants), new List<Guid>{fakeDictionary.First().Value});
            var argumentsInFakeServiceBus = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.Publish)).Arguments;
            var fakePublish = (LicensingDetailsUpdated)argumentsInFakeServiceBus[0];
            Assert.Equal(fakePublish.TenantId, fakeLicenseByTenantId.Identifier);
            fakePublish.LicenseByIdentifier.Should().BeEquivalentTo(fakeLicenseByTenantId);
        }
        
        [Fact(DisplayName = "Testa a remocao de um app de varias licenças")]
        public async Task Test_Remove_App_From_Licenses()
        {
            // prepare data
            var fakeIdInvoked = Guid.NewGuid();
            var fakeLicenseByTenantId = new LicenseByIdentifier()
            {
                Status = LicensingStatus.Active,
                Identifier = fakeIdInvoked,
                ExpirationDateTime = DateTime.UtcNow
            };
            var licensedTenantId = new List<Guid> {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};
            var fakeLicensedApps = await CreateLicensedApps(licensedTenantId);
            var fakeDictionary = LicensedDictionary(licensedTenantId);
            var fakeLicensesCall = new List<Guid>
            {
                fakeDictionary.ElementAt(0).Value,
                fakeDictionary.ElementAt(1).Value,
                fakeDictionary.ElementAt(2).Value,
            };

            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService
                .Setup(r => r.InvalidateCacheForTenants(new List<Guid>{fakeIdInvoked}));

            var fakeServiceBus = new Mock<IServiceBus>();
            fakeServiceBus
                .Setup(r => r.Publish(null, null));
            
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService
                .Setup(r => r.GetLicensesByIdentifiers(fakeLicensesCall)).ReturnsAsync(new List<LicenseByIdentifier>{fakeLicenseByTenantId});
            
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicensedTenantCacheService.Object, fakeServiceBus.Object, fakeLicenseServerService.Object);
            // execute
            await service.RemoveAppsFromLicenses(fakeLicensedApps, fakeDictionary);
            // test
            var memoryRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var countLicensedAppsAfterDelete = await memoryRepository.CountAsync();
            Assert.Equal(0, countLicensedAppsAfterDelete);
        }
        
        [Fact(DisplayName = "Testa a remocao de um app de uma licença (sendo um app Default)")]
        public async Task Test_Remove_App_From_License_With_Default_App()
        {
            // prepare data
            var tenantId = Guid.NewGuid();
            
            var fakeAppId = Guid.NewGuid();
            var fakeLicensedTenantId = Guid.NewGuid();
            var licensedAppsRepo = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var licensedTenantRepo = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            await licensedTenantRepo!.InsertAsync(new Domain.Entities.LicensedTenant
            {
                Id = fakeLicensedTenantId,
                Identifier = Guid.NewGuid(),
                Status = LicensingStatus.Active,
                AccountId = Guid.NewGuid(),
                AdministratorEmail = "123@korp.com.br",
                LicensedCnpjs = "73610432000198",
                TenantId = tenantId,
                LicenseConsumeType = LicenseConsumeType.Access
            }, true);
            await licensedAppsRepo!.InsertAsync(new LicensedApp
            {
                Id = Guid.NewGuid(),
                Status = LicensedAppStatus.AppActive,
                AppId = fakeAppId,
                LicensingModel = LicensingModels.Floating,
                TenantId = tenantId,
                LicensedTenantId = fakeLicensedTenantId,
                NumberOfLicenses = 10
            }, true);
            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            fakeLicenseRepository
                .Setup(r => r.CheckIfLicensedAppIsDefault(fakeAppId))
                .ReturnsAsync(true);
            var appToDeleteFromLicenses = new LicensedAppDeleteInput
            {
                AppId = fakeAppId,
                LicensedTenantId = fakeLicensedTenantId
            };
            var expectedResultWhenAppIsDefault = new RemoveAppFromLicenseOutput
            {
                Success = false,
                ErrorCode = OperationValidation.CantRemoveFromLicenseDefaultApp
            };
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = new LicensedTenantService(null, null, null, licensedAppsRepo, null, licensedTenantRepo, null, fakeLicenseRepository.Object, null, null, null, 
                fakeLicensedTenantCacheService.Object, null, null, null, null, null, null, null, null, null);
            // execute
            var result = await service.RemoveAppFromLicense(appToDeleteFromLicenses);
            // test
            expectedResultWhenAppIsDefault.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa a remocao de um app de uma licença (não sendo um app Default porém sem licença para este app)")]
        public async Task Test_Remove_App_From_License_With_No_Default_App_And_No_Licensed_App()
        {
            // prepare data
            var fakeAppId = Guid.NewGuid();
            var fakeLicensedTenantId = Guid.NewGuid();
            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            fakeLicenseRepository
                .Setup(r => r.CheckIfLicensedAppIsDefault(fakeAppId))
                .ReturnsAsync(false);
            var fakeAppRepository = new Mock<IAppRepository>();
            var licensedAppDeleteFromLicensesInput = new LicensedAppDeleteFromLicensesInput
            {
                AppId = fakeAppId,
                LicensedTenantsId = new List<Guid>
                {
                    fakeLicensedTenantId
                }
            };
            var fakeLicensedApps = new List<LicensedApp>();
            fakeAppRepository
                .Setup(r => r.GetLicensedAppsForLicenses(licensedAppDeleteFromLicensesInput.LicensedTenantsId, licensedAppDeleteFromLicensesInput.AppId))
                .ReturnsAsync(fakeLicensedApps);
            var appToDeleteFromLicenses = new LicensedAppDeleteInput
            {
                AppId = fakeAppId,
                LicensedTenantId = fakeLicensedTenantId
            };
            var expectedResultWhenAppNotHaveLicense = new RemoveAppFromLicenseOutput
            {
                Success = false,
                ErrorCode = OperationValidation.NoTenantWithSuchId
            };
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicenseRepository.Object, fakeAppRepository.Object, fakeLicensedTenantCacheService.Object);
            // execute
            var result = await service.RemoveAppFromLicense(appToDeleteFromLicenses);
            // test
            expectedResultWhenAppNotHaveLicense.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa as chamadas de remocao de um app de um licenciamento")]
        public async Task Test_Remove_App_From_License_Calls()
        {
            // prepare data
            var fakeAppId = Guid.NewGuid();
            var fakeLicensedTenantId = Guid.NewGuid();

            var licensedTenantRepo = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();

            await licensedTenantRepo!.InsertAsync(new Domain.Entities.LicensedTenant
            {
                Id = fakeLicensedTenantId,
                Identifier = Guid.NewGuid(),
                Status = LicensingStatus.Active,
                AccountId = Guid.NewGuid()
            }, true);
            
            var licensedAppRepo = ServiceProvider.GetService<IRepository<LicensedApp>>();

            await licensedAppRepo!.InsertAsync(new LicensedApp
            {
                Id = Guid.NewGuid(),
                LicensedTenantId = fakeLicensedTenantId,
                LicensingModel = LicensingModels.Floating,
                LicensedBundleId = null,
                AppId = fakeAppId
            }, true);

            var namedUserAppRepo = ServiceProvider.GetService<IRepository<NamedUserAppLicense>>();
            
            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            fakeLicenseRepository
                .Setup(r => r.CheckIfLicensedAppIsDefault(fakeAppId))
                .ReturnsAsync(false);
            new LicensedAppDeleteFromLicensesInput
            {
                AppId = fakeAppId,
                LicensedTenantsId = new List<Guid>
                {
                    fakeLicensedTenantId
                }
            };
            var fakeLicensedApps = new List<LicensedApp>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    LicensedTenantId = Guid.NewGuid()
                }
            };
            var fakeDictionary = new Dictionary<Guid, Guid>();
            fakeDictionary.Add(fakeLicensedApps[0].LicensedTenantId, Guid.NewGuid());
            var fakeLicensedTenantsId = fakeLicensedApps.Select(r => r.LicensedTenantId).ToList();
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService
                .Setup(r => r.GetLicensesByIdentifiers(It.IsAny<List<Guid>>())).ReturnsAsync(new List<LicenseByIdentifier>());
            var appToDeleteFromLicenses = new LicensedAppDeleteInput
            {
                AppId = fakeAppId,
                LicensedTenantId = fakeLicensedTenantId
            };
            var expectedSuccessResult = new RemoveAppFromLicenseOutput
            {
                Success = true
            };
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();

            var unitOfWork = ServiceProvider.GetService<IUnitOfWork>();

            var licensedAppService = new LicensedAppService(licensedAppRepo, namedUserAppRepo, null);
            
            var service = new LicensedTenantService(null, null, null, licensedAppRepo, unitOfWork, licensedTenantRepo, null, 
                fakeLicenseRepository.Object, null, fakeLicenseServerService.Object, null, fakeLicensedTenantCacheService.Object, null, 
                null, null, null, null, licensedAppService, null, null, null);
            // execute
            var result = await service.RemoveAppFromLicense(appToDeleteFromLicenses);
            // test
            expectedSuccessResult.Should().BeEquivalentTo(result);
            // TEST METHOD CALLS COUNT
            fakeLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.CheckIfLicensedAppIsDefault));
            // TEST INVOCATION PARAMS OF METHODS
            fakeLicenseRepository.Invocations.AssertArgument(0, nameof(ILicenseRepository.CheckIfLicensedAppIsDefault), fakeAppId);
        }
        
        [Fact(DisplayName = "Testa o método para criação de um novo bundle licenciado")]
        public async Task Test_New_Licensed_Bundle_Create()
        {
            // prepare data
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicensedTenantCacheService.Object);
            // execute
            var result = await service.CreateLicensedBundle(GetLicensedBundle());
            // test 
            await UnitOfWork.SaveChangesAsync();
            var memoryRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedBundle>>();
            var uniqueLicensedBundle = memoryRepository.Single();
            uniqueLicensedBundle.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas quando serão publicados detalhes de tenants")]
        public async Task Check_Calls_For_Tenant_Details_Updated()
        {
            // prepare data
            var licensedIdentifiersToPublish = new List<Guid> {Guid.NewGuid()};
            var fakeLicenseByTenantId = new LicenseByIdentifier()
            {
                Status = LicensingStatus.Active,
                Identifier = licensedIdentifiersToPublish[0],
                ExpirationDateTime = DateTime.UtcNow
            };
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService
                .Setup(r => r.GetLicensesByIdentifiers(licensedIdentifiersToPublish)).ReturnsAsync(new List<LicenseByIdentifier>{fakeLicenseByTenantId});
            var fakeServiceBus = new Mock<IServiceBus>();
            fakeServiceBus
                .Setup(r => r.Publish(null, null));
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicenseServerService.Object, fakeServiceBus.Object, fakeLicensedTenantCacheService.Object);
            // execute
            await service.PublishLicensingDetailsUpdatedEvents(licensedIdentifiersToPublish);
            // test
            // TEST METHOD CALLS COUNT
            fakeServiceBus.Invocations.AssertSingle(nameof(IServiceBus.Publish));
            fakeLicenseServerService.Invocations.AssertSingle(nameof(ILicenseServerService.GetLicensesByIdentifiers));
            // TEST INVOCATION PARAMS OF METHODS
            var argumentsInFakeServiceBus = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.Publish)).Arguments;
            var fakePublish = (LicensingDetailsUpdated)argumentsInFakeServiceBus[0];
            Assert.Equal(fakePublish.TenantId, fakeLicenseByTenantId.Identifier);
            fakePublish.LicenseByIdentifier.Should().BeEquivalentTo(fakeLicenseByTenantId);
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao adicionar um novo bundle a um licenciamento e o retorno do método")]
        public async Task Test_Add_Bundle_To_License_Calls_Another_Services_And_Return()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var licensedTenants = await CreateLicensedTenants(new List<Guid>{ licensedTenantId });
            var bundleApps = await CreateBundledApps(new List<Guid>{ bundleId });
            var appIds = bundleApps.Select(b => b.AppId).ToList();
            var licensedTenantIdentifier = licensedTenants.Select(l => l.Identifier).First();
            var input = new LicensedBundleCreateInput
            {
                Status = LicensedBundleStatus.BundleActive,
                BundleId = bundleId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 15,
                NumberOfTemporaryLicenses = 2,
                ExpirationDateOfTemporaryLicenses = new DateTime(2020, 6, 6)
            };
            var memoryLicensedBundleRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedBundle>>();
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService.Setup(l => l.GetLicenseByIdentifier(licensedTenantIdentifier))
                .ReturnsAsync(new LicenseByIdentifier());
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService.Setup(c => c.InvalidateCacheForTenant(licensedTenantIdentifier));
            var fakeServiceBus = new Mock<IServiceBus>();
            fakeServiceBus.Setup(b => b.Publish(null, null));
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(a => a.GetAppsAlreadyLicensed(appIds, new List<Guid> {input.LicensedTenantId}))
                .ReturnsAsync(new List<AlreadyLicensedApp>());
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(b => b.GetBundlesAlreadyInLicenses(new List<Guid> {input.BundleId}, new List<Guid> {input.LicensedTenantId}))
                .ReturnsAsync(new List<LicensedBundleGetForBatchOperation>());
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicenseServerService.Object, fakeLicensedTenantCacheService.Object, fakeServiceBus.Object, fakeAppRepository.Object, fakeBundleRepository.Object);
            // execute
            var output = await service.AddBundleToLicense(input);
            await UnitOfWork.SaveChangesAsync();
            // TEST METHOD RETURNS 
            var singleLicensedBundle = memoryLicensedBundleRepository.Single();
            var singleLicensedApp = memoryLicensedAppRepository.Single();
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedBundleCreateOutput>(singleLicensedBundle);
            await AssertBundleCreated(input, singleLicensedBundle);
            await AssertBundleAppCreated(input, singleLicensedApp);
            expectedOutput.Should().BeEquivalentTo(output);
            // TEST METHOD CALLS COUNT
            fakeLicenseServerService.Invocations.AssertSingle(nameof(ILicenseServerService.GetLicenseByIdentifier));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenant));
            fakeServiceBus.Invocations.AssertSingle(nameof(IServiceBus.Publish));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppsAlreadyLicensed));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesAlreadyInLicenses));
            // TEST INVOCATION PARAMS OF METHODS
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), new List<Guid> {input.BundleId});
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), new List<Guid> {input.LicensedTenantId});
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppsAlreadyLicensed), appIds);
            fakeAppRepository.Invocations.AssertArgument(1, nameof(IAppRepository.GetAppsAlreadyLicensed), new List<Guid> {input.LicensedTenantId});
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenant), licensedTenantIdentifier);
            var argumentsInFakeServiceBus = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.Publish)).Arguments;
            var argumentPublished = (LicensingDetailsUpdated)argumentsInFakeServiceBus[0];
            Assert.Equal(argumentPublished.TenantId, licensedTenantIdentifier);
            argumentPublished.LicenseByIdentifier.Should().BeEquivalentTo(new LicenseByIdentifier());
        }

        [Fact(DisplayName = "Testa as chamadas realizadas ao adicionar um novo bundle a um licenciamento com este bundle já licenciamento e o retorno do método")]
        public async Task Test_Add_Bundle_To_License_Calls_Another_Services_When_Bundle_Already_Licensed_And_Return()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var licensedTenants = await CreateLicensedTenants(new List<Guid>{ licensedTenantId });
            var bundleApps = await CreateBundledApps(new List<Guid>{ bundleId });
            var appIds = bundleApps.Select(b => b.AppId).ToList();
            var licensedTenantIdentifier = licensedTenants.Select(l => l.Identifier).First();
            var input = new LicensedBundleCreateInput
            {
                Status = LicensedBundleStatus.BundleActive,
                BundleId = bundleId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 15,
                NumberOfTemporaryLicenses = 2,
                ExpirationDateOfTemporaryLicenses = new DateTime(2020, 6, 6)
            };
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedBundleCreateOutput>(input);
            expectedOutput.OperationValidation = OperationValidation.DuplicatedIdentifier;
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService.Setup(l => l.GetLicenseByIdentifier(licensedTenantIdentifier))
                .ReturnsAsync(new LicenseByIdentifier());
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService.Setup(c => c.InvalidateCacheForTenant(licensedTenantIdentifier));
            var fakeServiceBus = new Mock<IServiceBus>();
            fakeServiceBus.Setup(b => b.Publish(null, null));
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(a => a.GetAppsAlreadyLicensed(appIds, new List<Guid> {input.LicensedTenantId}))
                .ReturnsAsync(new List<AlreadyLicensedApp>());
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(b => b.GetBundlesAlreadyInLicenses(new List<Guid> {input.BundleId}, new List<Guid> {input.LicensedTenantId}))
                .ReturnsAsync(new List<LicensedBundleGetForBatchOperation>{ new LicensedBundleGetForBatchOperation() });
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeBundleRepository.Object, fakeLicensedTenantCacheService.Object, fakeAppRepository.Object, fakeServiceBus.Object,
            fakeLicenseServerService.Object);
            // execute
            var output = await service.AddBundleToLicense(input);
            // TEST METHOD RETURNS 
            expectedOutput.Should().BeEquivalentTo(output);
            // TEST METHOD CALLS COUNT
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesAlreadyInLicenses));
            // TEST INVOCATION PARAMS OF METHODS
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), new List<Guid> {input.BundleId});
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), new List<Guid> {input.LicensedTenantId});
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao adicionar um novo bundle a um licenciamento com alguns apps deste bundle já licenciados e o retorno do método")]
        public async Task Test_Add_Bundle_To_License_Calls_Another_Services_When_Some_Bundle_Apps_Already_Licensed_And_Return()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var licensedTenants = await CreateLicensedTenants(new List<Guid>{ licensedTenantId });
            var bundleApps = await CreateBundledApps(new List<Guid>{ bundleId });
            var appIds = bundleApps.Select(b => b.AppId).ToList();
            var licensedTenantIdentifier = licensedTenants.Select(l => l.Identifier).First();
            var input = new LicensedBundleCreateInput
            {
                Status = LicensedBundleStatus.BundleActive,
                BundleId = bundleId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 15,
                NumberOfTemporaryLicenses = 2,
                ExpirationDateOfTemporaryLicenses = new DateTime(2020, 6, 6)
            };
            var memoryRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedBundle>>();
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService.Setup(l => l.GetLicenseByIdentifier(licensedTenantIdentifier))
                .ReturnsAsync(new LicenseByIdentifier());
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService.Setup(c => c.InvalidateCacheForTenant(licensedTenantIdentifier));
            var fakeServiceBus = new Mock<IServiceBus>();
            fakeServiceBus.Setup(b => b.Publish(null, null));
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(a => a.GetAppsAlreadyLicensed(appIds, new List<Guid> {input.LicensedTenantId}))
                .ReturnsAsync(new List<AlreadyLicensedApp>{ new AlreadyLicensedApp{ AppId = appIds[0]}});
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(b => b.GetBundlesAlreadyInLicenses(new List<Guid> {input.BundleId}, new List<Guid> {input.LicensedTenantId}))
                .ReturnsAsync(new List<LicensedBundleGetForBatchOperation>());
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicenseServerService.Object, fakeLicensedTenantCacheService.Object, fakeServiceBus.Object, fakeAppRepository.Object, fakeBundleRepository.Object);
            // execute
            var output = await service.AddBundleToLicense(input);
            await UnitOfWork.SaveChangesAsync();
            // TEST METHOD RETURNS 
            var singleLicensedBundle = memoryRepository.Single();
            var licensedApps = await memoryLicensedAppRepository.ToListAsync();
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedBundleCreateOutput>(singleLicensedBundle);
            await AssertBundleCreated(input, singleLicensedBundle);
            Assert.Empty(licensedApps);
            expectedOutput.OperationValidation = OperationValidation.AppAlreadyLicensed;
            expectedOutput.Should().BeEquivalentTo(output);
            // TEST METHOD CALLS COUNT
            fakeLicenseServerService.Invocations.AssertSingle(nameof(ILicenseServerService.GetLicenseByIdentifier));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenant));
            fakeServiceBus.Invocations.AssertSingle(nameof(IServiceBus.Publish));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppsAlreadyLicensed));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesAlreadyInLicenses));
            // TEST INVOCATION PARAMS OF METHODS
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), new List<Guid> {input.BundleId});
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), new List<Guid> {input.LicensedTenantId});
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppsAlreadyLicensed), appIds);
            fakeAppRepository.Invocations.AssertArgument(1, nameof(IAppRepository.GetAppsAlreadyLicensed), new List<Guid> {input.LicensedTenantId});
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenant), licensedTenantIdentifier);
            var argumentsInFakeServiceBus = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.Publish)).Arguments;
            var argumentPublished = (LicensingDetailsUpdated)argumentsInFakeServiceBus[0];
            Assert.Equal(argumentPublished.TenantId, licensedTenantIdentifier);
            argumentPublished.LicenseByIdentifier.Should().BeEquivalentTo(new LicenseByIdentifier());
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao adicionar um novo bundle a um licenciamento com número inválido de licenças")]
        public async Task Test_Add_Bundle_To_License_Calls_Another_Services_When_Have_Invalid_Number_Of_Licenses()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var input = new LicensedBundleCreateInput
            {
                Status = LicensedBundleStatus.BundleActive,
                BundleId = bundleId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 0,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null
            };
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedBundleCreateOutput>(input);
            expectedOutput.OperationValidation = OperationValidation.InvalidNumberOfLicenses;
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicensedTenantCacheService.Object);
            // execute
            var output = await service.AddBundleToLicense(input);
            // TEST METHOD RETURNS 
            expectedOutput.Should().BeEquivalentTo(output);
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao atualizar um bundle a um licenciamento e o retorno do método")]
        public async Task Test_Update_Bundle_To_License_Calls_Another_Services_And_Return()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            await CreateLicensedApps(new List<Guid>{ licensedTenantId }, bundleId);
            await CreateLicensedBundles(new List<Guid>{ bundleId }, licensedTenantId);
            await CreateBundledApps(new List<Guid>{ bundleId });
            var licensedTenants = await CreateLicensedTenants(new List<Guid>{ licensedTenantId });
            var licensedTenantIdentifier = licensedTenants.Select(l => l.Identifier).First();
            var input = new LicensedBundleUpdateInput
            {
                Status = LicensedBundleStatus.BundleActive,
                BundleId = bundleId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 150,
                NumberOfTemporaryLicenses = 200,
                ExpirationDateOfTemporaryLicenses = new DateTime(2020, 9, 6)
            };
            var memoryLicensedBundleRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedBundle>>();
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService.Setup(l => l.GetLicenseByIdentifier(licensedTenantIdentifier))
                .ReturnsAsync(new LicenseByIdentifier());
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService.Setup(c => c.InvalidateCacheForTenant(licensedTenantIdentifier));
            var fakeServiceBus = new Mock<IServiceBus>();
            fakeServiceBus.Setup(b => b.Publish(null, null));
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicenseServerService.Object, fakeLicensedTenantCacheService.Object, fakeServiceBus.Object);
            // execute
            var output = await service.UpdateBundleFromLicense(input);
            // TEST METHOD RETURNS 
            var singleLicensedBundle = memoryLicensedBundleRepository.Single();
            var singleLicensedApp = memoryLicensedAppRepository.Single();
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedBundleUpdateOutput>(singleLicensedBundle);
            expectedOutput.Should().BeEquivalentTo(output);
            await AssertBundleUpdated(input, singleLicensedBundle);
            await AssertAppUpdatedFromBundle(input, singleLicensedApp);
            // TEST METHOD CALLS COUNT
            fakeLicenseServerService.Invocations.AssertSingle(nameof(ILicenseServerService.GetLicenseByIdentifier));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenant));
            fakeServiceBus.Invocations.AssertSingle(nameof(IServiceBus.Publish));
            // TEST INVOCATION PARAMS OF METHODS
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenant), licensedTenantIdentifier);
            fakeLicenseServerService.Invocations.AssertArgument(0, nameof(ILicenseServerService.GetLicenseByIdentifier), licensedTenantIdentifier);
            var argumentsInFakeServiceBus = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.Publish)).Arguments;
            var argumentPublished = (LicensingDetailsUpdated)argumentsInFakeServiceBus[0];
            Assert.Equal(argumentPublished.TenantId, licensedTenantIdentifier);
            argumentPublished.LicenseByIdentifier.Should().BeEquivalentTo(new LicenseByIdentifier());
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao atualizar um bundle a um licenciamento e o retorno do método quando um bundle não contém apps")]
        public async Task Test_Update_Bundle_To_License_Calls_Another_Services_And_Return_When_Bundle_Not_Contains_Apps()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            await CreateLicensedBundles(new List<Guid>{ bundleId }, licensedTenantId);
            await CreateBundledApps(new List<Guid>{ bundleId });
            var licensedTenants = await CreateLicensedTenants(new List<Guid>{ licensedTenantId });
            var licensedTenantIdentifier = licensedTenants.Select(l => l.Identifier).First();
            var input = new LicensedBundleUpdateInput
            {
                Status = LicensedBundleStatus.BundleActive,
                BundleId = bundleId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 150,
                NumberOfTemporaryLicenses = 200,
                ExpirationDateOfTemporaryLicenses = new DateTime(2020, 9, 6)
            };
            var memoryLicensedBundleRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedBundle>>();
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var licensedApps = await memoryLicensedAppRepository.ToListAsync();
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService.Setup(l => l.GetLicenseByIdentifier(licensedTenantIdentifier))
                .ReturnsAsync(new LicenseByIdentifier());
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService.Setup(c => c.InvalidateCacheForTenant(licensedTenantIdentifier));
            var fakeServiceBus = new Mock<IServiceBus>();
            fakeServiceBus.Setup(b => b.Publish(null, null));
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicenseServerService.Object, fakeLicensedTenantCacheService.Object, fakeServiceBus.Object);
            // execute
            var output = await service.UpdateBundleFromLicense(input);
            // TEST METHOD RETURNS 
            var singleLicensedBundle = memoryLicensedBundleRepository.Single();
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedBundleUpdateOutput>(singleLicensedBundle);
            expectedOutput.Should().BeEquivalentTo(output);
            Assert.Empty(licensedApps);
            await AssertBundleUpdated(input, singleLicensedBundle);
            // TEST METHOD CALLS COUNT
            fakeLicenseServerService.Invocations.AssertSingle(nameof(ILicenseServerService.GetLicenseByIdentifier));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenant));
            fakeServiceBus.Invocations.AssertSingle(nameof(IServiceBus.Publish));
            // TEST INVOCATION PARAMS OF METHODS
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenant), licensedTenantIdentifier);
            fakeLicenseServerService.Invocations.AssertArgument(0, nameof(ILicenseServerService.GetLicenseByIdentifier), licensedTenantIdentifier);
            var argumentsInFakeServiceBus = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.Publish)).Arguments;
            var argumentPublished = (LicensingDetailsUpdated)argumentsInFakeServiceBus[0];
            Assert.Equal(argumentPublished.TenantId, licensedTenantIdentifier);
            argumentPublished.LicenseByIdentifier.Should().BeEquivalentTo(new LicenseByIdentifier());
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao atualizar um bundle a um licenciamento e o retorno do método quando este bundle já não está mais licenciado")]
        public async Task Test_Update_Bundle_To_License_Calls_Another_Services_And_Return_When_Not_Have_Licensed_Bundle()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var input = new LicensedBundleUpdateInput
            {
                Status = LicensedBundleStatus.BundleActive,
                BundleId = bundleId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 150,
                NumberOfTemporaryLicenses = 200,
                ExpirationDateOfTemporaryLicenses = new DateTime(2020, 9, 6)
            };
            var expectedOutput = new LicensedBundleUpdateOutput
            {
                Status = input.Status,
                BundleId = input.BundleId,
                OperationValidation = OperationValidation.LicenseDoesNotExist,
                LicensedTenantId = input.LicensedTenantId,
                NumberOfLicenses = input.NumberOfLicenses,
                NumberOfTemporaryLicenses = input.NumberOfTemporaryLicenses,
                ExpirationDateOfTemporaryLicenses = input.ExpirationDateOfTemporaryLicenses
            };
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicensedTenantCacheService.Object);
            // execute
            var output = await service.UpdateBundleFromLicense(input);
            // TEST METHOD RETURNS 
            expectedOutput.Should().BeEquivalentTo(output);
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao atualizar um bundle a um licenciamento e o retorno do método quando não é enviado um número valido de licenças")]
        public async Task Test_Update_Bundle_To_License_Calls_Another_Services_And_Return_When_Not_Have_Valid_Number_Of_Licenses()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var input = new LicensedBundleUpdateInput
            {
                Status = LicensedBundleStatus.BundleActive,
                BundleId = bundleId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 0,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null
            };
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedBundleUpdateOutput>(input);
            expectedOutput.OperationValidation = OperationValidation.InvalidNumberOfLicenses;
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicensedTenantCacheService.Object);
            // execute
            var output = await service.UpdateBundleFromLicense(input);
            // TEST METHOD RETURNS 
            expectedOutput.Should().BeEquivalentTo(output);
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao atribuir um app a um licenciamento e o retorno do método")]
        public async Task Test_Add_Loose_App_To_License_Calls_Another_Services_And_Return()
        {
            // prepare data
            var appId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var licensedTenants = await CreateLicensedTenants(new List<Guid>{ licensedTenantId });
            var licensedTenantIdentifier = licensedTenants.Select(l => l.Identifier).First();
            var input = new LicensedAppCreateInput
            {
                AppId = appId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 150,
                NumberOfTemporaryLicenses = 200,
                ExpirationDateOfTemporaryLicenses = new DateTime(2020, 9, 6)
            };
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService.Setup(l => l.GetLicenseByIdentifier(licensedTenantIdentifier))
                .ReturnsAsync(new LicenseByIdentifier());
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService.Setup(c => c.InvalidateCacheForTenant(licensedTenantIdentifier));
            var fakeServiceBus = new Mock<IServiceBus>();
            fakeServiceBus.Setup(b => b.Publish(null, null));
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicenseServerService.Object, fakeLicensedTenantCacheService.Object, fakeServiceBus.Object);
            // execute
            var output = await service.AddLooseAppToLicense(input);
            await UnitOfWork.CompleteAsync();
            // TEST METHOD RETURNS 
            var singleLicensedApp = memoryLicensedAppRepository.Single();
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedAppCreateOutput>(singleLicensedApp);
            expectedOutput.Should().BeEquivalentTo(output);
            await AssertLooseAppCreated(input, singleLicensedApp);
            // TEST METHOD CALLS COUNT
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenant));
            fakeLicenseServerService.Invocations.AssertSingle(nameof(ILicenseServerService.GetLicenseByIdentifier));
            fakeServiceBus.Invocations.AssertSingle(nameof(IServiceBus.Publish));
            // TEST INVOCATION PARAMS OF METHODS
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenant), licensedTenantIdentifier);
            fakeLicenseServerService.Invocations.AssertArgument(0, nameof(ILicenseServerService.GetLicenseByIdentifier), licensedTenantIdentifier);
            var argumentsInFakeServiceBus = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.Publish)).Arguments;
            var argumentPublished = (LicensingDetailsUpdated)argumentsInFakeServiceBus[0];
            Assert.Equal(argumentPublished.TenantId, licensedTenantIdentifier);
            argumentPublished.LicenseByIdentifier.Should().BeEquivalentTo(new LicenseByIdentifier());
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao atribuir um app a um licenciamento e o retorno do método quando este app já está licenciado")]
        public async Task Test_Add_Loose_App_To_License_Calls_Another_Services_And_Return_When_App_Already_Licensed()
        {
            // prepare data
            var licensedTenantId = Guid.NewGuid();
            var alreadyLicensedApp = await CreateLicensedApps(new List<Guid>{ licensedTenantId });
            var input = new LicensedAppCreateInput
            {
                AppId = alreadyLicensedApp[0].AppId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 150,
                NumberOfTemporaryLicenses = 200,
                ExpirationDateOfTemporaryLicenses = new DateTime(2020, 9, 6)
            };
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedAppCreateOutput>(input);
            expectedOutput.OperationValidation = OperationValidation.DuplicatedIdentifier;
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicensedTenantCacheService.Object);
            // execute
            var output = await service.AddLooseAppToLicense(input);
            // TEST METHOD RETURNS 
            expectedOutput.Should().BeEquivalentTo(output);
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao atribuir um app a um licenciamento e o retorno do método quando não é enviado um número valido de licenças")]
        public async Task Test_Add_Loose_App_To_License_Calls_Another_Services_And_Return_When_Not_Have_Valid_Number_Of_Licenses()
        {
            // prepare data
            var licensedTenantId = Guid.NewGuid();
            var alreadyLicensedApp = await CreateLicensedApps(new List<Guid>{ licensedTenantId });
            var input = new LicensedAppCreateInput
            {
                AppId = alreadyLicensedApp[0].AppId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 0,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null
            };
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedAppCreateOutput>(input);
            expectedOutput.OperationValidation = OperationValidation.InvalidNumberOfLicenses;
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicensedTenantCacheService.Object);
            // execute
            var output = await service.AddLooseAppToLicense(input);
            // TEST METHOD RETURNS 
            expectedOutput.Should().BeEquivalentTo(output);
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao autalizar um app a um licenciamento e o retorno do método")]
        public async Task Test_Update_Loose_App_To_License_Calls_Another_Services_And_Return()
        {
            // prepare data
            var licensedTenantId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var licensedTenantIdentifier = Guid.NewGuid();
            var licensedTenantRepo = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            var licensedAppRepo = ServiceProvider.GetService<IRepository<LicensedApp>>();
            

            await licensedTenantRepo!.InsertAsync(new Domain.Entities.LicensedTenant
            {
                Id = licensedTenantId,
                Identifier = licensedTenantIdentifier
            }, true);

            await licensedAppRepo!.InsertAsync(new LicensedApp
            {
                Id = Guid.NewGuid(),
                AppId = appId,
                LicensedTenantId = licensedTenantId,
                Status = LicensedAppStatus.AppActive,
                LicensingModel = LicensingModels.Floating,
                NumberOfLicenses = 10
            }, true);
                
            var input = new LicensedAppUpdateInput
            {
                AppId = appId,
                LicensedTenantId = licensedTenantId,
                NumberOfLicenses = 150,
                NumberOfTemporaryLicenses = 200,
                ExpirationDateOfTemporaryLicenses = new DateTime(2020, 9, 6)
            };
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService.Setup(l => l.GetLicenseByIdentifier(licensedTenantIdentifier))
                .ReturnsAsync(new LicenseByIdentifier());
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService.Setup(c => c.InvalidateCacheForTenant(licensedTenantIdentifier));
            var fakeServiceBus = new Mock<IServiceBus>();
            fakeServiceBus.Setup(b => b.Publish(null, null));
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicenseServerService.Object, fakeLicensedTenantCacheService.Object, fakeServiceBus.Object);
            // execute
            var output = await service.UpdateLooseAppFromLicense(input);
            // TEST METHOD RETURNS
            var singleLicensedApp = licensedAppRepo.Single();
            var expectedOutput = new LicensedAppUpdateOutput
            {
                Status = LicensedAppStatus.AppBlocked,
                AppId = appId,
                LicensedTenantId = licensedTenantId,
                LicensedBundleId = null,
                NumberOfLicenses = 150,
                NumberOfTemporaryLicenses = 200,
                ExpirationDateOfTemporaryLicenses = new DateTime(2020, 9, 6)
            };
            expectedOutput.Should().BeEquivalentTo(output);
            await AssertLooseAppUpdated(input, singleLicensedApp);
            // TEST METHOD CALLS COUNT
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenant));
            fakeLicenseServerService.Invocations.AssertSingle(nameof(ILicenseServerService.GetLicenseByIdentifier));
            fakeServiceBus.Invocations.AssertSingle(nameof(IServiceBus.Publish));
            // TEST INVOCATION PARAMS OF METHODS
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenant), licensedTenantIdentifier);
            fakeLicenseServerService.Invocations.AssertArgument(0, nameof(ILicenseServerService.GetLicenseByIdentifier), licensedTenantIdentifier);
            var argumentsInFakeServiceBus = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.Publish)).Arguments;
            var argumentPublished = (LicensingDetailsUpdated)argumentsInFakeServiceBus[0];
            Assert.Equal(argumentPublished.TenantId, licensedTenantIdentifier);
            argumentPublished.LicenseByIdentifier.Should().BeEquivalentTo(new LicenseByIdentifier());
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao autalizar um app a um licenciamento e o retorno do método quando este app já não está mais licenciado")]
        public async Task Test_Update_Loose_App_To_License_Calls_Another_Services_And_Return_When_App_Not_Licensed()
        {
            // prepare data
            var input = new LicensedAppUpdateInput
            {
                AppId = Guid.NewGuid(),
                LicensedTenantId = Guid.NewGuid(),
                NumberOfLicenses = 150,
                NumberOfTemporaryLicenses = 200,
                ExpirationDateOfTemporaryLicenses = new DateTime(2020, 9, 6)
            };
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedAppUpdateOutput>(input);
            expectedOutput.OperationValidation = OperationValidation.NoTenantWithSuchId;
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicensedTenantCacheService.Object);
            // execute
            var output = await service.UpdateLooseAppFromLicense(input);
            // TEST METHOD RETURNS
            expectedOutput.Should().BeEquivalentTo(output);
        }
        
        [Fact(DisplayName = "Testa as chamadas realizadas ao atualizar um app a um licenciamento e o retorno do método quando não é mandado um número valido de licenças")]
        public async Task Test_Update_Loose_App_To_License_Calls_Another_Services_And_Return_When_Have_Not_Valid_Number_Of_Licenses()
        {
            // prepare data
            var input = new LicensedAppUpdateInput
            {
                AppId = Guid.NewGuid(),
                LicensedTenantId = Guid.NewGuid(),
                NumberOfLicenses = 0,
                NumberOfTemporaryLicenses = 0,
                ExpirationDateOfTemporaryLicenses = null
            };
            var mapper = ServiceProvider.GetService<IMapper>();
            var expectedOutput = mapper.Map<LicensedAppUpdateOutput>(input);
            expectedOutput.OperationValidation = OperationValidation.InvalidNumberOfLicenses;
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicensedTenantCacheService.Object);
            // execute
            var output = await service.UpdateLooseAppFromLicense(input);
            // TEST METHOD RETURNS
            expectedOutput.Should().BeEquivalentTo(output);
        }
        
        [Fact(DisplayName = "Cria uma nova licença com status negativo, checa o retorno e as chamadas")]
        public async Task Test_Create_New_License_With_Invalid_Status()
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
            var fakeLicenseCrudValidator = new Mock<ILicensingCrudValidator>();
            var output = new LicenseTenantCreateOutput { OperationValidation = OperationValidation.DuplicatedIdentifier};
            fakeLicenseCrudValidator.Setup(r => r.ValidateLicensingForCreate(input)).ReturnsAsync(() => (false, output));
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicenseCrudValidator.Object, fakeLicensedTenantCacheService.Object);
            // execute
            var result = await service.CreateNewTenantLicensing(input, true);
            // TEST METHOD RETURNS
            output.Should().BeEquivalentTo(result);
            // TEST METHOD CALLS COUNT
            fakeLicenseCrudValidator.Invocations.AssertSingle(nameof(ILicensingCrudValidator.ValidateLicensingForCreate));
            // TEST ARGUMENTS IN METHOD
            fakeLicenseCrudValidator.Invocations.AssertArgument(0, nameof(ILicensingCrudValidator.ValidateLicensingForCreate), input);
        }
        
        [Fact(DisplayName = "Cria uma nova licença checa o retorno e as chamadas")]
        public async Task Test_Create_New_License()
        {
            // prepare data
            var input = new LicenseTenantCreateInput
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid(),
                Status = LicensingStatus.Active,
                AccountId = Guid.NewGuid(),
                AdministratorEmail = "admin@korp",
                LicensedCnpjs = "",
                LicenseConsumeType = LicenseConsumeType.Access,
                Notes = ""
            };
            var expectedOutput = new LicenseTenantCreateOutput
            {
                Id = input.Id,
                Identifier = input.Identifier,
                Status = LicensingStatus.Blocked,
                AccountId = input.AccountId,
                AdministratorEmail = "admin@korp",
                LicensedCnpjs = "",
                LicenseConsumeType = LicenseConsumeType.Access,
                Notes = ""
            };
            var expectedServiceBusSendLocal = new LicensedTenantCreatedCommand
            {
                Identifier = input.Identifier,
                Status = expectedOutput.Status,
                AccountId = input.AccountId,
                AdministratorEmail = input.AdministratorEmail,
                LicensedCnpjs = input.LicensedCnpjs,
                ExpirationDateTime = null,
                LicensedTenantId = input.Id,
                LicenseConsumeType = input.LicenseConsumeType
            };
            var expectedServiceBusPublish = new LicensedTenantCreated
            {
                Id = input.Id,
                TenantId = input.Identifier,
                Status = expectedOutput.Status,
                AdministratorEmail = input.AdministratorEmail,
                AccountName = null
            };
            var memoryRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            var fakeLicenseCrudValidator = new Mock<ILicensingCrudValidator>();
            var output = new LicenseTenantCreateOutput { OperationValidation = OperationValidation.DuplicatedIdentifier};
            fakeLicenseCrudValidator.Setup(r => r.ValidateLicensingForCreate(input)).ReturnsAsync(() => (true, null));
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService.Setup(r => r.InvalidateCacheForTenant(input.Identifier));
            var fakeAppRepository= new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAllDefaultApps()).ReturnsAsync(new List<App>());
            var fakeServiceBus = new Mock<IServiceBus>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicenseCrudValidator.Object, fakeLicensedTenantCacheService.Object, fakeServiceBus.Object, fakeAppRepository.Object);
            // execute
            var result = await service.CreateNewTenantLicensing(input, true);
            // TEST METHOD RETURNS
            expectedOutput.Should().BeEquivalentTo(result);
            // TEST MEMORY REPOSITORY DATA 
            var data = await memoryRepository.SingleAsync();
            await AssertLicensedTenantCreated(expectedOutput, data);
            // TEST METHOD CALLS COUNT
            fakeLicenseCrudValidator.Invocations.AssertSingle(nameof(ILicensingCrudValidator.ValidateLicensingForCreate));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenant));
            fakeServiceBus.Invocations.AssertSingle(nameof(IServiceBus.Publish));
            fakeServiceBus.Invocations.AssertCount(2, nameof(IServiceBus.SendLocal));
            // TEST ARGUMENTS IN METHOD
            fakeLicenseCrudValidator.Invocations.AssertArgument(0, nameof(ILicensingCrudValidator.ValidateLicensingForCreate), input);
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenant), input.Identifier);
            var argumentsInFakeServiceBusPublish = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.Publish)).Arguments;
            var fakePublish = (LicensedTenantCreated)argumentsInFakeServiceBusPublish[0];
            fakePublish.Should().BeEquivalentTo(expectedServiceBusPublish);
            var argumentsInFakeServiceBusSendLocal = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.SendLocal)).Arguments;
            var fakeSendLocal = (LicensedTenantCreatedCommand)argumentsInFakeServiceBusSendLocal[0];
            fakeSendLocal.Should().BeEquivalentTo(expectedServiceBusSendLocal);
        }
        
        [Fact(DisplayName = "Cria uma nova licença checa o retorno e as chamadas quando existem default apps")]
        public async Task Test_Create_New_License_With_Default_Apps()
        {
            // prepare data
            var input = new LicenseTenantCreateInput
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid(),
                Status = LicensingStatus.Active,
                AccountId = Guid.NewGuid(),
                AdministratorEmail = "admin@korp",
                LicensedCnpjs = "",
                LicenseConsumeType = LicenseConsumeType.Access,
                Notes = ""
            };
            var expectedOutput = new LicenseTenantCreateOutput
            {
                Id = input.Id,
                Identifier = input.Identifier,
                Status = LicensingStatus.Blocked,
                AccountId = input.AccountId,
                AdministratorEmail = "admin@korp",
                LicensedCnpjs = "",
                LicenseConsumeType = LicenseConsumeType.Access,
                Notes = ""
            };
            var expectedServiceBusSendLocal = new LicensedTenantCreatedCommand
            {
                Identifier = input.Identifier,
                Status = expectedOutput.Status,
                AccountId = input.AccountId,
                AdministratorEmail = input.AdministratorEmail,
                LicensedCnpjs = input.LicensedCnpjs,
                ExpirationDateTime = null,
                LicensedTenantId = input.Id,
                LicenseConsumeType = input.LicenseConsumeType
            };
            var expectedServiceBusPublish = new LicensedTenantCreated
            {
                Id = input.Id,
                TenantId = input.Identifier,
                Status = expectedOutput.Status,
                AdministratorEmail = input.AdministratorEmail,
                AccountName = null
            };
            var defaultApp = new App {Id = Guid.NewGuid()};
            var expectedLicensedAppCreated = new LicensedAppCreateInput
            {
                AppId = defaultApp.Id,
                LicensedTenantId = input.Id,
                Status = LicensedAppStatus.AppActive,
                NumberOfLicenses = int.MaxValue,
                AdditionalNumberOfLicenses = 0
            };
            var memoryLicensedTenantRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            var memoryAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var fakeLicenseCrudValidator = new Mock<ILicensingCrudValidator>();
            var output = new LicenseTenantCreateOutput { OperationValidation = OperationValidation.DuplicatedIdentifier};
            fakeLicenseCrudValidator.Setup(r => r.ValidateLicensingForCreate(input)).ReturnsAsync(() => (true, null));
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService.Setup(r => r.InvalidateCacheForTenant(input.Identifier));
            var fakeAppRepository= new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAllDefaultApps()).ReturnsAsync(new List<App> {defaultApp});
            var fakeServiceBus = new Mock<IServiceBus>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider, fakeLicenseCrudValidator.Object, fakeLicensedTenantCacheService.Object, fakeServiceBus.Object, fakeAppRepository.Object);
            // execute
            var result = await service.CreateNewTenantLicensing(input, true);
            // TEST METHOD RETURNS
            expectedOutput.Should().BeEquivalentTo(result);
            // TEST MEMORY REPOSITORY DATA 
            var dataLicensedTenant = await memoryLicensedTenantRepository.SingleAsync();
            var dataLicensedApp = await memoryAppRepository.SingleAsync();
            await AssertLicensedTenantCreated(expectedOutput, dataLicensedTenant);
            await AssertLicensedApp(dataLicensedApp, expectedLicensedAppCreated);
            // TEST METHOD CALLS COUNT
            fakeLicenseCrudValidator.Invocations.AssertSingle(nameof(ILicensingCrudValidator.ValidateLicensingForCreate));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenant));
            fakeServiceBus.Invocations.AssertSingle(nameof(IServiceBus.Publish));
            fakeServiceBus.Invocations.AssertCount(2, nameof(IServiceBus.SendLocal));
            // TEST ARGUMENTS IN METHOD
            fakeLicenseCrudValidator.Invocations.AssertArgument(0, nameof(ILicensingCrudValidator.ValidateLicensingForCreate), input);
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenant), input.Identifier);
            var argumentsInFakeServiceBusPublish = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.Publish)).Arguments;
            var fakePublish = (LicensedTenantCreated)argumentsInFakeServiceBusPublish[0];
            fakePublish.Should().BeEquivalentTo(expectedServiceBusPublish);
            var argumentsInFakeServiceBusSendLocal = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.SendLocal)).Arguments;
            var fakeSendLocal = (LicensedTenantCreatedCommand)argumentsInFakeServiceBusSendLocal[0];
            fakeSendLocal.Should().BeEquivalentTo(expectedServiceBusSendLocal);
        }
        
        [Fact(DisplayName = "Testa o método de atualizar uma nova licença e suas chamadas com erros na validação")]
        public async Task Test_Update_License_With_Errors_In_Validation()
        {
            // prepare data
            var updateLicensedTenant = new LicenseTenantUpdateInput
            {
                Id = Guid.NewGuid(),
                Identifier = Guid.NewGuid()
            };
            var output = new LicenseTenantUpdateOutput { OperationValidation = OperationValidation.DuplicatedIdentifier};
            var fakeLicenseCrudValidator = new Mock<ILicensingCrudValidator>();
            fakeLicenseCrudValidator.Setup(r => r.ValidateLicensingForUpdate(updateLicensedTenant))
                .ReturnsAsync(() => (false, output));
            var fakeLicenseTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider,  fakeLicenseCrudValidator.Object, fakeLicenseTenantCacheService.Object);
            // execute
            var result = await service.UpdateTenantLicensing(updateLicensedTenant);
            // TEST METHOD RETURNS
            output.Should().BeEquivalentTo(result);
            // TEST METHOD CALLS COUNT
            fakeLicenseCrudValidator.Invocations.AssertSingle(nameof(ILicensingCrudValidator.ValidateLicensingForUpdate));
            // TEST ARGUMENTS IN METHOD
            fakeLicenseCrudValidator.Invocations.AssertArgument(0, nameof(ILicensingCrudValidator.ValidateLicensingForUpdate), updateLicensedTenant);
        }
        
        [Fact(DisplayName = "Testa o método de atualizar uma nova licença e suas chamadas")]
        public async Task Test_Update_License()
        {
            // prepare data
            var licensedTenant = (await CreateLicensedTenants(new List<Guid> {Guid.NewGuid()}))[0];
            var updateLicensedTenant = new LicenseTenantUpdateInput
            {
                Id = licensedTenant.Id,
                Identifier = licensedTenant.Identifier,
                AccountId = Guid.NewGuid(),
                Notes = ""
            };
            var expectedOutput = new LicenseTenantUpdateOutput
            {
                Id = updateLicensedTenant.Id,
                Identifier = updateLicensedTenant.Identifier,
                AccountId = updateLicensedTenant.AccountId,
                Notes = ""
            };
            var expectedTenantUpdatedLocal = new LicensedTenantUpdatedCommand
            {
                Identifier = updateLicensedTenant.Identifier,
                LicensedTenantId = updateLicensedTenant.Id,
                Status = updateLicensedTenant.Status,
                AccountId = updateLicensedTenant.AccountId,
                AdministratorEmail = updateLicensedTenant.AdministratorEmail,
                LicensedCnpjs = updateLicensedTenant.LicensedCnpjs,
                ExpirationDateTime = updateLicensedTenant.ExpirationDateTime,
                LicenseConsumeType = updateLicensedTenant.LicenseConsumeType
            };
            var output = new LicenseTenantUpdateOutput { OperationValidation = OperationValidation.DuplicatedIdentifier};
            var memoryLicensedTenantRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            var fakeLicenseCrudValidator = new Mock<ILicensingCrudValidator>();
            fakeLicenseCrudValidator.Setup(r => r.ValidateLicensingForUpdate(updateLicensedTenant))
                .ReturnsAsync(() => (true, null));
            var fakeLicenseTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeServiceBus = new Mock<IServiceBus>();
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider,  fakeLicenseCrudValidator.Object, fakeLicenseTenantCacheService.Object, fakeServiceBus.Object, fakeLicenseServerService.Object);
            // execute
            var result = await service.UpdateTenantLicensing(updateLicensedTenant);
            // TEST METHOD RETURNS
            var data = await memoryLicensedTenantRepository.SingleAsync();
            await AssertLicensedTenantUpdated(expectedOutput, data);
            expectedOutput.Should().BeEquivalentTo(result);
            // TEST METHOD CALLS COUNT
            fakeLicenseCrudValidator.Invocations.AssertSingle(nameof(ILicensingCrudValidator.ValidateLicensingForUpdate));
            fakeServiceBus.Invocations.AssertSingle(nameof(IServiceBus.SendLocal));
            fakeServiceBus.Invocations.AssertCount(0,nameof(IServiceBus.Publish));
            fakeServiceBus.Invocations.AssertCount(0,nameof(ILicenseServerService.GetLicenseByIdentifier));
            // TEST ARGUMENTS IN METHOD
            fakeLicenseCrudValidator.Invocations.AssertArgument(0, nameof(ILicensingCrudValidator.ValidateLicensingForUpdate), updateLicensedTenant);
            var argumentsInFakeServiceBusSendLocal = fakeServiceBus.Invocations
                .First(r => r.Method.Name == nameof(IServiceBus.SendLocal)).Arguments;
            var fakeSendLocal = (LicensedTenantUpdatedCommand)argumentsInFakeServiceBusSendLocal[0];
            fakeSendLocal.Should().BeEquivalentTo(expectedTenantUpdatedLocal);
        }
        
        [Fact(DisplayName = "Testa o método de atualizar uma nova licença e suas chamadas quando seu status e o email de administrator foi alterado")]
        public async Task Test_Update_License_When_Status_And_Email_Changed()
        {
            // prepare data
            var licensedTenant = (await CreateLicensedTenants(new List<Guid> {Guid.NewGuid()}))[0];
            var updateLicensedTenant = new LicenseTenantUpdateInput
            {
                Id = licensedTenant.Id,
                Identifier = licensedTenant.Identifier,
                AccountId = Guid.NewGuid(),
                Notes = "",
                Status =  LicensingStatus.Active,
                AdministratorEmail = "teste@korp.com.br"
            };
            var expectedOutput = new LicenseTenantUpdateOutput
            {
                Id = updateLicensedTenant.Id,
                Identifier = updateLicensedTenant.Identifier,
                AccountId = updateLicensedTenant.AccountId,
                Notes = updateLicensedTenant.Notes,
                Status = LicensingStatus.Active,
                AdministratorEmail = updateLicensedTenant.AdministratorEmail
            };
            var expectedTenantUpdatedLocal = new LicensedTenantUpdatedCommand
            {
                Identifier = updateLicensedTenant.Identifier,
                LicensedTenantId = updateLicensedTenant.Id,
                Status = updateLicensedTenant.Status,
                AccountId = updateLicensedTenant.AccountId,
                AdministratorEmail = updateLicensedTenant.AdministratorEmail,
                LicensedCnpjs = updateLicensedTenant.LicensedCnpjs,
                ExpirationDateTime = updateLicensedTenant.ExpirationDateTime,
                LicenseConsumeType = updateLicensedTenant.LicenseConsumeType
            };

            var expectedUpdateAdministratorEmailCommand = new UpdateAdministratorEmailCommand
            {
                NewEmail = updateLicensedTenant.AdministratorEmail,
                OldEmail = licensedTenant.AdministratorEmail,
                TenantId = updateLicensedTenant.Identifier
            };
            
            var tenantIdentifierToPublish = new LicenseByIdentifier();
            var output = new LicenseTenantUpdateOutput { OperationValidation = OperationValidation.DuplicatedIdentifier};
            var memoryLicensedTenantRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            var fakeLicenseCrudValidator = new Mock<ILicensingCrudValidator>();
            fakeLicenseCrudValidator.Setup(r => r.ValidateLicensingForUpdate(updateLicensedTenant))
                .ReturnsAsync(() => (true, null));
            var fakeLicenseServerService = new Mock<ILicenseServerService>();
            fakeLicenseServerService.Setup(r => r.GetLicenseByIdentifier(updateLicensedTenant.Identifier))
                .ReturnsAsync(tenantIdentifierToPublish);
            var fakeLicenseTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeServiceBus = new Mock<IServiceBus>();
            var service = ActivatorUtilities.CreateInstance<LicensedTenantService>(ServiceProvider,  fakeLicenseCrudValidator.Object, fakeLicenseTenantCacheService.Object, fakeServiceBus.Object, fakeLicenseServerService.Object);
            // execute
            var result = await service.UpdateTenantLicensing(updateLicensedTenant);
            // TEST METHOD RETURNS
            var data = await memoryLicensedTenantRepository.SingleAsync();
            await AssertLicensedTenantUpdated(expectedOutput, data);
            expectedOutput.Should().BeEquivalentTo(result);
            // TEST METHOD CALLS COUNT
            fakeLicenseCrudValidator.Invocations.AssertSingle(nameof(ILicensingCrudValidator.ValidateLicensingForUpdate));
            fakeServiceBus.Invocations.AssertCount(2, nameof(IServiceBus.SendLocal));
            fakeServiceBus.Invocations.AssertCount(1, nameof(IServiceBus.Publish));
            fakeLicenseServerService.Invocations.AssertSingle(nameof(ILicenseServerService.GetLicenseByIdentifier));
            // TEST ARGUMENTS IN METHOD
            fakeLicenseCrudValidator.Invocations.AssertArgument(0, nameof(ILicensingCrudValidator.ValidateLicensingForUpdate), updateLicensedTenant);
            var fakeInvocationsServiceBusSendLocal = fakeServiceBus.Invocations
                .Where(r => r.Method.Name == nameof(IServiceBus.SendLocal)).ToList();
            var fakeArgumentInFirstSendLocalInvocation = (LicensedTenantUpdatedCommand) fakeInvocationsServiceBusSendLocal[0].Arguments.First();
            fakeArgumentInFirstSendLocalInvocation.Should().BeEquivalentTo(expectedTenantUpdatedLocal);
            
            var fakeUpdateAdministratorEmailCommand = (UpdateAdministratorEmailCommand) fakeInvocationsServiceBusSendLocal[1].Arguments.First();
            fakeUpdateAdministratorEmailCommand.Should().BeEquivalentTo(expectedUpdateAdministratorEmailCommand);
            
            var argumentsInFakeServiceBusPublish = fakeServiceBus.Invocations.Where(r => r.Method.Name == nameof(IServiceBus.Publish)).ToList();
            
            var publishUpdateLicensedTenant = (LicensingDetailsUpdated) argumentsInFakeServiceBusPublish[0].Arguments.First();
            Assert.Equal(publishUpdateLicensedTenant.TenantId, updateLicensedTenant.Identifier);
            Assert.Equal(publishUpdateLicensedTenant.LicenseByIdentifier, tenantIdentifierToPublish);
        }
        
        private Task AssertBundleCreated(LicensedBundleCreateInput createdInput, Domain.Entities.LicensedBundle licensedBundle)
        {
            Assert.Equal(createdInput.BundleId, licensedBundle.BundleId);
            Assert.Equal(createdInput.LicensedTenantId, licensedBundle.LicensedTenantId);
            Assert.Equal(createdInput.NumberOfLicenses, licensedBundle.NumberOfLicenses);
            Assert.Equal(createdInput.NumberOfTemporaryLicenses, licensedBundle.NumberOfTemporaryLicenses);
            Assert.Equal(createdInput.ExpirationDateOfTemporaryLicenses, licensedBundle.ExpirationDateOfTemporaryLicenses);
            return Task.CompletedTask;
        }
        
        private Task AssertBundleAppCreated(LicensedBundleCreateInput createdInput, LicensedApp licensedApp)
        {
            Assert.Equal(createdInput.BundleId, licensedApp.LicensedBundleId);
            Assert.Equal(createdInput.LicensedTenantId, licensedApp.LicensedTenantId);
            Assert.Equal((LicensedAppStatus)createdInput.Status, licensedApp.Status);
            Assert.Equal(createdInput.NumberOfLicenses, licensedApp.NumberOfLicenses);
            Assert.Equal(createdInput.NumberOfTemporaryLicenses, licensedApp.NumberOfTemporaryLicenses);
            Assert.Equal(createdInput.ExpirationDateOfTemporaryLicenses, licensedApp.ExpirationDateOfTemporaryLicenses);
            return Task.CompletedTask;
        }

        private Task AssertBundleUpdated(LicensedBundleUpdateInput updatedInput, Domain.Entities.LicensedBundle licensedBundle)
        {
            Assert.Equal(updatedInput.BundleId, licensedBundle.BundleId);
            Assert.Equal(updatedInput.LicensedTenantId, licensedBundle.LicensedTenantId);
            Assert.Equal(updatedInput.NumberOfLicenses, licensedBundle.NumberOfLicenses);
            Assert.Equal(updatedInput.NumberOfTemporaryLicenses, licensedBundle.NumberOfTemporaryLicenses);
            Assert.Equal(updatedInput.ExpirationDateOfTemporaryLicenses, licensedBundle.ExpirationDateOfTemporaryLicenses);
            return Task.CompletedTask;
        }

        private Task AssertAppUpdatedFromBundle(LicensedBundleUpdateInput licensedBundleInput, LicensedApp licensedApp)
        {
            Assert.Equal(licensedApp.LicensedBundleId, licensedBundleInput.BundleId);
            Assert.Equal(licensedApp.LicensedTenantId, licensedBundleInput.LicensedTenantId);
            Assert.Equal(licensedApp.NumberOfLicenses, licensedBundleInput.NumberOfLicenses);
            Assert.Equal(licensedApp.NumberOfTemporaryLicenses, licensedBundleInput.NumberOfTemporaryLicenses);
            Assert.Equal(licensedApp.ExpirationDateOfTemporaryLicenses, licensedBundleInput.ExpirationDateOfTemporaryLicenses);
            return Task.CompletedTask;
        }

        private Task AssertLooseAppCreated(LicensedAppCreateInput licensedAppUpdateInput, LicensedApp licensedApp)
        {
            Assert.Equal(licensedApp.AppId, licensedAppUpdateInput.AppId);
            Assert.Equal(licensedApp.LicensedTenantId, licensedAppUpdateInput.LicensedTenantId);
            Assert.Equal(licensedApp.NumberOfLicenses, licensedAppUpdateInput.NumberOfLicenses);
            Assert.Equal(licensedApp.NumberOfTemporaryLicenses, licensedAppUpdateInput.NumberOfTemporaryLicenses);
            Assert.Equal(licensedApp.ExpirationDateOfTemporaryLicenses, licensedAppUpdateInput.ExpirationDateOfTemporaryLicenses);
            Assert.Equal(licensedApp.ExpirationDateOfTemporaryLicenses, licensedAppUpdateInput.ExpirationDateOfTemporaryLicenses);
            Assert.Equal(licensedApp.Status, licensedAppUpdateInput.Status);
            return Task.CompletedTask;
        }
        
        private Task AssertLooseAppUpdated(LicensedAppUpdateInput licensedAppUpdateInput, LicensedApp licensedApp)
        {
            Assert.Equal(licensedApp.AppId, licensedAppUpdateInput.AppId);
            Assert.Equal(licensedApp.LicensedTenantId, licensedAppUpdateInput.LicensedTenantId);
            Assert.Equal(licensedApp.NumberOfLicenses, licensedAppUpdateInput.NumberOfLicenses);
            Assert.Equal(licensedApp.NumberOfTemporaryLicenses, licensedAppUpdateInput.NumberOfTemporaryLicenses);
            Assert.Equal(licensedApp.ExpirationDateOfTemporaryLicenses, licensedAppUpdateInput.ExpirationDateOfTemporaryLicenses);
            Assert.Equal(licensedApp.ExpirationDateOfTemporaryLicenses, licensedAppUpdateInput.ExpirationDateOfTemporaryLicenses);
            return Task.CompletedTask;
        }
        
        private Task AssertLicensedTenantCreated(LicenseTenantCreateOutput licenseTenantCreateOutput, Domain.Entities.LicensedTenant licensedTenant)
        {
            Assert.Equal(licenseTenantCreateOutput.Id, licensedTenant.Id);
            Assert.Equal(licenseTenantCreateOutput.Identifier, licensedTenant.Identifier);
            Assert.Equal(licenseTenantCreateOutput.AdministratorEmail, licensedTenant.AdministratorEmail);
            Assert.Equal(licenseTenantCreateOutput.Status, licensedTenant.Status);
            Assert.Equal(licenseTenantCreateOutput.AccountId, licensedTenant.AccountId);
            Assert.Equal(licenseTenantCreateOutput.LicenseConsumeType, licensedTenant.LicenseConsumeType);
            Assert.Equal(licenseTenantCreateOutput.LicensedCnpjs, licensedTenant.LicensedCnpjs);
            return Task.CompletedTask;
        }
        
        private Task AssertLicensedTenantUpdated(LicenseTenantUpdateOutput licenseTenantUpdateOutput, Domain.Entities.LicensedTenant licensedTenant)
        {
            Assert.Equal(licenseTenantUpdateOutput.Id, licensedTenant.Id);
            Assert.Equal(licenseTenantUpdateOutput.Identifier, licensedTenant.Identifier);
            Assert.Equal(licenseTenantUpdateOutput.AccountId, licensedTenant.AccountId);
            return Task.CompletedTask;
        }
        
        private Task AssertLicensedApp(LicensedApp licensedApp, LicensedAppCreateInput licensedAppCreateInput)
        {
            Assert.Equal(licensedApp.AppId, licensedAppCreateInput.AppId);
            Assert.Equal(licensedApp.LicensedTenantId, licensedAppCreateInput.LicensedTenantId);
            Assert.Equal(licensedApp.Status, licensedAppCreateInput.Status);
            Assert.Equal(licensedApp.NumberOfLicenses, licensedAppCreateInput.NumberOfLicenses);
            Assert.Equal(licensedApp.AdditionalNumberOfLicenses, licensedAppCreateInput.AdditionalNumberOfLicenses);
            return Task.CompletedTask;
        }
        
        private LicensedBundleCreateInput GetLicensedBundle()
        {
            return new LicensedBundleCreateInput
            {
                LicensedTenantId = Guid.NewGuid(),
                Status = LicensedBundleStatus.BundleActive,
                BundleId = Guid.NewGuid(),
                NumberOfLicenses = 2
            };
        }
        
        private async Task<List<LicensedApp>> CreateLicensedApps(List<Guid> licensedTenantsId, Guid licensedBundleId = new Guid())
        {
            var output = new List<LicensedApp>();
            var memoryRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            foreach (var licensedTenantId in licensedTenantsId)
            {
                var newFakeLicensedApp = new LicensedApp
                {
                    Id = Guid.NewGuid(),
                    LicensedTenantId = licensedTenantId,
                    AppId = Guid.NewGuid(),
                    Status = LicensedAppStatus.AppActive,
                    LicensedBundleId = licensedBundleId
                };
                output.Add(newFakeLicensedApp);
                await memoryRepository.InsertAsync(newFakeLicensedApp, true);
            }
            return output;
        }
        
        private async Task<List<BundledApp>> CreateBundledApps(List<Guid> bundleIds)
        {
            var output = new List<BundledApp>();
            var memoryRepository = ServiceProvider.GetService<IRepository<BundledApp>>();
            foreach (var bundleId in bundleIds)
            {
                var newBundleApp = new BundledApp
                {
                    Id = Guid.NewGuid(),
                    AppId = Guid.NewGuid(),
                    BundleId = bundleId
                };
                output.Add(newBundleApp);
                await memoryRepository.InsertAsync(newBundleApp, true);
            }
            return output;
        }
        
        private async Task<List<Domain.Entities.LicensedTenant>> CreateLicensedTenants(List<Guid> licensedTenantIds)
        {
            var output = new List<Domain.Entities.LicensedTenant>();
            var memoryRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            foreach (var licensedTenantId in licensedTenantIds)
            {
                var newLicensedTenant = new Domain.Entities.LicensedTenant
                {
                    Id = licensedTenantId,
                    Identifier = Guid.NewGuid()
                };
                output.Add(newLicensedTenant);
                await memoryRepository.InsertAsync(newLicensedTenant, true);
            }
            return output;
        }
        
        private async Task<List<Domain.Entities.LicensedBundle>> CreateLicensedBundles(List<Guid> bundleIds, Guid licensedTenantId)
        {
            var output = new List<Domain.Entities.LicensedBundle>();
            var memoryRepository = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedBundle>>();
            foreach (var bundleId in bundleIds)
            {
                var newLicensedBundle = new Domain.Entities.LicensedBundle
                {
                    Id = Guid.NewGuid(),
                    BundleId = bundleId,
                    LicensedTenantId = licensedTenantId
                };
                output.Add(newLicensedBundle);
                await memoryRepository.InsertAsync(newLicensedBundle, true);
            }
            return output;
        }
        
        private Dictionary<Guid, Guid> LicensedDictionary(List<Guid> licensedTenantsId)
        {
            var output = new Dictionary<Guid, Guid>();
            foreach (var licensedTenantId in licensedTenantsId)
            {
                output.Add(licensedTenantId, Guid.NewGuid());
            }
            return output;
        }
    }
}