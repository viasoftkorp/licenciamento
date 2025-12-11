using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.App;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.LicenseBatchRepository;
using Viasoft.Licensing.LicensingManagement.Domain.Services.BatchOperationServices;
using Viasoft.Licensing.LicensingManagement.Domain.Services.BatchOperationServices.BatchOperationLoggerService;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.BatchOperations
{
    public class LicensedTenantBatchOperationsServiceUnitTest: LicensingManagementTestBase
    {
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando um app é removido")]
        public async Task Remove_App_In_License_Check_Services_Calls()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var fakeIdInvoke = Guid.NewGuid();
            var fakeBundleList = FakeLicensedBundleList(fakeIdInvoke);
            var fakeLicenseTenantIds = fakeBundleList.Select(r => r.LicensedTenantId).ToList();
            var fakeDictionary = new Dictionary<Guid, Guid>();
            var expectedLicensedTenantIdentifier = Guid.NewGuid();
            fakeDictionary.Add(fakeBundleList[0].LicensedTenantId, expectedLicensedTenantIdentifier);
            var fakeAppIdentifier = "sou fake";
            var fakeLicensedApps = new List<LicensedApp>
            {
                new LicensedApp
                {
                    Id = fakeIdInvoke
                }
            };

            var fakeLicenseRepository =new Mock<ILicenseRepository>();
            fakeLicenseRepository
                .Setup(r => r.CheckIfLicensedAppIsDefault(appId))
                .ReturnsAsync(false);
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository
                .Setup(r => r.GetAppIdentifiersByAppIds(new List<Guid>{ appId })).ReturnsAsync(new List<AppsGetForBatchOperations>{ new AppsGetForBatchOperations{ Id = appId, Identifier = fakeAppIdentifier}});
            fakeAppRepository
                .Setup(r => r.GetLicensedAppsForLicenses(fakeLicenseTenantIds, appId))
                .ReturnsAsync(fakeLicensedApps);
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            fakeLicenseBatchRepository
                .Setup(r => r.GetLicensedBundlesWithAppsLicensed(bundleId, appId))
                .ReturnsAsync(fakeBundleList);
            fakeLicenseBatchRepository
                .Setup(r => r.GetLicensedTenantToIdentifierDictionary(fakeLicenseTenantIds))
                .ReturnsAsync(fakeDictionary);
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();;
            fakeLicensedTenantService
                .Setup(r => r.RemoveAppsFromLicenses(fakeLicensedApps, fakeDictionary))
                .ReturnsAsync(new List<Guid>{ expectedLicensedTenantIdentifier });
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            fakeBathOperationLogger.Setup(r =>
                r.LogRemoveAppFromBundleInLicenses(fakeAppIdentifier,
                    new List<Guid> {expectedLicensedTenantIdentifier}));
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeBundleRepository = new Mock<IBundleRepository>();
            
            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.RemoveAppFromBundleInLicenses(bundleId, appId);
            // test
            // TEST METHOD CALLS COUNT
            fakeLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.CheckIfLicensedAppIsDefault));
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedBundlesWithAppsLicensed));
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetLicensedAppsForLicenses));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.RemoveAppsFromLicenses));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppIdentifiersByAppIds));
            fakeBathOperationLogger.Invocations.AssertSingle(nameof(IBatchOperationLoggerService.LogRemoveAppFromBundleInLicenses));
            // TEST INVOCATION PARAMS OF METHODS
            fakeLicenseRepository.Invocations.AssertArgument(0, nameof(ILicenseRepository.CheckIfLicensedAppIsDefault), appId);
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, nameof(ILicenseBatchRepository.GetLicensedBundlesWithAppsLicensed), bundleId);
            fakeLicenseBatchRepository.Invocations.AssertArgument(1, nameof(ILicenseBatchRepository.GetLicensedBundlesWithAppsLicensed), appId);
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary), fakeLicenseTenantIds);
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetLicensedAppsForLicenses), fakeLicenseTenantIds);
            fakeAppRepository.Invocations.AssertArgument(1, nameof(IAppRepository.GetLicensedAppsForLicenses), appId);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.RemoveAppsFromLicenses), fakeLicensedApps);
            fakeLicensedTenantService.Invocations.AssertArgument(1, nameof(ILicensedTenantService.RemoveAppsFromLicenses), fakeDictionary);
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppIdentifiersByAppIds), new List<Guid>{ appId });
            fakeBathOperationLogger.Invocations.AssertArgument(0, nameof(IBatchOperationLoggerService.LogRemoveAppFromBundleInLicenses), fakeAppIdentifier);
            fakeBathOperationLogger.Invocations.AssertArgument(1, nameof(IBatchOperationLoggerService.LogRemoveAppFromBundleInLicenses), new List<Guid> {expectedLicensedTenantIdentifier});
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando um app é removido porém é um app default")]
        public async Task Remove_App_In_License_Check_Services_Calls_Default_App()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var fakeLicenseRepository =new Mock<ILicenseRepository>();
            fakeLicenseRepository
                .Setup(r => r.CheckIfLicensedAppIsDefault(appId))
                .ReturnsAsync(true);
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();;
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeAppRepository = new Mock<IAppRepository>();
            var fakeBundleRepository = new Mock<IBundleRepository>();
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            
            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.RemoveAppFromBundleInLicenses(bundleId, appId);
            // test
            // TEST METHOD CALLS COUNT
            fakeLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.CheckIfLicensedAppIsDefault));
            fakeLicenseBatchRepository.Invocations.AssertCount(0, nameof(ILicenseBatchRepository.GetLicensedBundlesWithAppsLicensed));
            fakeLicenseBatchRepository.Invocations.AssertCount(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeAppRepository.Invocations.AssertCount(0, nameof(IAppRepository.GetLicensedAppsForLicenses));
            fakeLicensedTenantService.Invocations.AssertCount(0, nameof(ILicensedTenantService.RemoveAppsFromLicenses));
            fakeAppRepository.Invocations.AssertCount(0, nameof(IAppRepository.GetAppIdentifiersByAppIds));
            fakeBathOperationLogger.Invocations.AssertCount(0, nameof(IBatchOperationLoggerService.LogRemoveAppFromBundleInLicenses));
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando um app é removido porém este app não está presente em nenhum licenciamento")]
        public async Task Remove_App_In_License_Check_Services_Calls_No_App_In_Licenses()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var fakeBundleList = new List<Domain.Entities.LicensedBundle>();
            var fakeLicenseRepository =new Mock<ILicenseRepository>();
            fakeLicenseRepository
                .Setup(r => r.CheckIfLicensedAppIsDefault(appId))
                .ReturnsAsync(false);
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            fakeLicenseBatchRepository
                .Setup(r => r.GetLicensedBundlesWithAppsLicensed(bundleId, appId))
                .ReturnsAsync(fakeBundleList);

            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();;
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeAppRepository = new Mock<IAppRepository>();
            var fakeBundleRepository = new Mock<IBundleRepository>();
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            
            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.RemoveAppFromBundleInLicenses(bundleId, appId);
            // test
            // TEST METHOD CALLS COUNT
            fakeLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.CheckIfLicensedAppIsDefault));
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedBundlesWithAppsLicensed));
            fakeLicenseBatchRepository.Invocations.AssertCount(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeAppRepository.Invocations.AssertCount(0, nameof(IAppRepository.GetLicensedAppsForLicenses));
            fakeLicensedTenantService.Invocations.AssertCount(0, nameof(ILicensedTenantService.RemoveAppsFromLicenses));
            fakeAppRepository.Invocations.AssertCount(0, nameof(IAppRepository.GetAppIdentifiersByAppIds));
            fakeBathOperationLogger.Invocations.AssertCount(0, nameof(IBatchOperationLoggerService.LogRemoveAppFromBundleInLicenses));
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando bundles são adicionados para licenciamentos")]
        public async Task Insert_Bundles_In_Licenses_Check_Services_Calls()
        {
            // prepare data
            var fakeInput = new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput{AllSelected = true},
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput{AllSelected = true},
                NumberOfLicenses = 50
            };
            var fakeBundlesList = FakeBundlesList();
            var fakeLicensedTenantsList = FakeLicensedTenantList(1);
            var fakeBundleIdentifiers = fakeBundlesList.Select(b => b.Identifier).ToList();
            var fakeBundleIds = fakeBundlesList.Select(b => b.Id).ToList();
            var fakeLicensedTenantIds = fakeLicensedTenantsList.Select(l => l.Id).ToList();
            var bundleApps = await CreateBundleApps(fakeBundleIds[0], new List<Guid>{ Guid.NewGuid()});
            var bundleAppsIds = bundleApps.Select(b => b.AppId).ToList();
            var fakeIdInvoked = Guid.NewGuid();
            var fakeDictionary = new Dictionary<Guid, Guid>();
            fakeDictionary.Add(fakeLicensedTenantIds[0], fakeIdInvoked);
            var expectedBundleCreate = new LicensedBundleCreateInput
            {
                Status = LicensedBundleStatus.BundleActive,
                BundleId = fakeBundleIds[0],
                LicensedTenantId = fakeLicensedTenantIds[0],
                NumberOfLicenses = 50
            };
            
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(r => r.GetAllBundlesForBatchOperation(null))
                .ReturnsAsync(fakeBundlesList);
            fakeBundleRepository.Setup(r => r.GetBundlesAlreadyInLicenses(fakeBundleIds, fakeLicensedTenantIds))
                .ReturnsAsync(new List<LicensedBundleGetForBatchOperation>());
            var fakeLicenseRepository =new Mock<ILicenseRepository>();
            fakeLicenseRepository.Setup(r => r.GetAllLicensesForBatchOperations(null))
                .ReturnsAsync(fakeLicensedTenantIds);
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAppsAlreadyLicensed(bundleAppsIds, fakeLicensedTenantIds))
                .ReturnsAsync(new List<AlreadyLicensedApp>());
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            fakeLicenseBatchRepository.Setup(r => r.GetLicensedTenantToIdentifierDictionary(fakeLicensedTenantIds))
                .ReturnsAsync(fakeDictionary);
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            fakeLicensedTenantService
                .Setup(r => r.AddBundledAppsToLicense(bundleAppsIds[0], expectedBundleCreate));
            fakeLicensedTenantService
                .Setup(r => r.CreateLicensedBundle(new LicensedBundleCreateInput()));
            fakeLicensedTenantService
               .Setup(r => r.PublishLicensingDetailsUpdatedEvents(new List<Guid>{ fakeIdInvoked }));
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService
                .Setup(r => r.InvalidateCacheForTenants(new List<Guid>{ fakeIdInvoked }));
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            fakeBathOperationLogger.Setup(r => r.LogInsertBundleInLicenses(expectedBundleCreate.NumberOfLicenses, fakeBundleIdentifiers, new List<Guid>{ fakeIdInvoked }));

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.InsertBundlesInLicenses(fakeInput);
            // test
            // TEST METHOD CALLS COUNT
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetAllBundlesForBatchOperation));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesAlreadyInLicenses));
            fakeLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetAllLicensesForBatchOperations));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppsAlreadyLicensed));
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.CreateLicensedBundle));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.AddBundledAppsToLicense));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenants));
            fakeBathOperationLogger.Invocations.AssertSingle(nameof(IBatchOperationLoggerService.LogInsertBundleInLicenses));
            // TEST ARGUMENTS IN METHOD
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), fakeBundleIds);
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), fakeLicensedTenantIds);
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppsAlreadyLicensed), bundleAppsIds);
            fakeAppRepository.Invocations.AssertArgument(1, nameof(IAppRepository.GetAppsAlreadyLicensed), fakeLicensedTenantIds);
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary), fakeLicensedTenantIds);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.CreateLicensedBundle), expectedBundleCreate);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.AddBundledAppsToLicense), bundleAppsIds[0]);
            fakeLicensedTenantService.Invocations.AssertArgument(1, nameof(ILicensedTenantService.AddBundledAppsToLicense), expectedBundleCreate);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents), new List<Guid>{ fakeIdInvoked });
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenants), new List<Guid>{ fakeIdInvoked });
            fakeBathOperationLogger.Invocations.AssertArgument(0, nameof(IBatchOperationLoggerService.LogInsertBundleInLicenses), expectedBundleCreate.NumberOfLicenses);
            fakeBathOperationLogger.Invocations.AssertArgument(1, nameof(IBatchOperationLoggerService.LogInsertBundleInLicenses), fakeBundleIdentifiers);
            fakeBathOperationLogger.Invocations.AssertArgument(2, nameof(IBatchOperationLoggerService.LogInsertBundleInLicenses), new List<Guid>{ fakeIdInvoked });
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando bundles são adicionados para licenciamentos, e as importações adicionam um app" +
                            "para um tenant, e posteriormente outro bundle tenta adicionar este app novamente")]
        public async Task Insert_Bundles_In_Licenses_Check_Services_Calls_When_Some_Apps_Licensed_For_Tenant_In_Loop()
        {
            // prepare data
            var fakeInput = new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput{AllSelected = true},
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput{AllSelected = true},
                NumberOfLicenses = 50
            };
            var fakeBundlesList = FakeBundlesList();
            fakeBundlesList.AddRange(FakeBundlesList());
            var fakeLicensedTenantsList = FakeLicensedTenantList(1);
            var fakeBundleIdentifiers = fakeBundlesList.Select(b => b.Identifier).Distinct().ToList();
            var fakeBundleIds = fakeBundlesList.Select(b => b.Id).ToList();
            var fakeLicensedTenantIds = fakeLicensedTenantsList.Select(l => l.Id).ToList();
            var appIdDuplicatedForTenant = Guid.NewGuid();
            var bundleApps = await CreateBundleApps(fakeBundleIds[0], new List<Guid>{ appIdDuplicatedForTenant });
            bundleApps.AddRange(await CreateBundleApps(fakeBundleIds[1], new List<Guid>{ appIdDuplicatedForTenant }));
            var bundleAppsIds = bundleApps.Select(b => b.AppId).ToList();
            var fakeIdInvoked = Guid.NewGuid();
            var fakeDictionary = new Dictionary<Guid, Guid>();
            fakeDictionary.Add(fakeLicensedTenantIds[0], fakeIdInvoked);
            var expectedBundleCreate = new LicensedBundleCreateInput
            {
                Status = LicensedBundleStatus.BundleActive,
                BundleId = fakeBundleIds[0],
                LicensedTenantId = fakeLicensedTenantIds[0],
                NumberOfLicenses = 50
            };
            
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(r => r.GetAllBundlesForBatchOperation(null))
                .ReturnsAsync(fakeBundlesList);
            fakeBundleRepository.Setup(r => r.GetBundlesAlreadyInLicenses(fakeBundleIds, fakeLicensedTenantIds))
                .ReturnsAsync(new List<LicensedBundleGetForBatchOperation>());
            var fakeLicenseRepository =new Mock<ILicenseRepository>();
            fakeLicenseRepository.Setup(r => r.GetAllLicensesForBatchOperations(null))
                .ReturnsAsync(fakeLicensedTenantIds);
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAppsAlreadyLicensed(bundleAppsIds, fakeLicensedTenantIds))
                .ReturnsAsync(new List<AlreadyLicensedApp>());
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            fakeLicenseBatchRepository.Setup(r => r.GetLicensedTenantToIdentifierDictionary(fakeLicensedTenantIds))
                .ReturnsAsync(fakeDictionary);
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            fakeLicensedTenantService
                .Setup(r => r.AddBundledAppsToLicense(bundleAppsIds[0], expectedBundleCreate));
            fakeLicensedTenantService
                .Setup(r => r.CreateLicensedBundle(new LicensedBundleCreateInput()));
            fakeLicensedTenantService
               .Setup(r => r.PublishLicensingDetailsUpdatedEvents(new List<Guid>{ fakeIdInvoked }));
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService
                .Setup(r => r.InvalidateCacheForTenants(new List<Guid>{ fakeIdInvoked }));
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            fakeBathOperationLogger.Setup(r => r.LogInsertBundleInLicenses(expectedBundleCreate.NumberOfLicenses, fakeBundleIdentifiers, new List<Guid>{ fakeIdInvoked }));


            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.InsertBundlesInLicenses(fakeInput);
            // test
            // TEST METHOD CALLS COUNT
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetAllBundlesForBatchOperation));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesAlreadyInLicenses));
            fakeLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetAllLicensesForBatchOperations));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppsAlreadyLicensed));
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeLicensedTenantService.Invocations.AssertCount(2, nameof(ILicensedTenantService.CreateLicensedBundle));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.AddBundledAppsToLicense));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenants));
            // TEST ARGUMENTS IN METHOD
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), fakeBundleIds);
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), fakeLicensedTenantIds);
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppsAlreadyLicensed), bundleAppsIds);
            fakeAppRepository.Invocations.AssertArgument(1, nameof(IAppRepository.GetAppsAlreadyLicensed), fakeLicensedTenantIds);
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary), fakeLicensedTenantIds);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.CreateLicensedBundle), expectedBundleCreate);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.AddBundledAppsToLicense), bundleAppsIds[0]);
            fakeLicensedTenantService.Invocations.AssertArgument(1, nameof(ILicensedTenantService.AddBundledAppsToLicense), expectedBundleCreate);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents), new List<Guid>{ fakeIdInvoked });
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenants), new List<Guid>{ fakeIdInvoked });
            fakeBathOperationLogger.Invocations.AssertArgument(0, nameof(IBatchOperationLoggerService.LogInsertBundleInLicenses), expectedBundleCreate.NumberOfLicenses);
            fakeBathOperationLogger.Invocations.AssertArgument(1, nameof(IBatchOperationLoggerService.LogInsertBundleInLicenses), fakeBundleIdentifiers);
            fakeBathOperationLogger.Invocations.AssertArgument(2, nameof(IBatchOperationLoggerService.LogInsertBundleInLicenses), new List<Guid>{ fakeIdInvoked });
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando bundles são adicionados para licenciamentos porém todos os apps presentes já estão licenciados")]
        public async Task Insert_Bundles_In_Licenses_Check_Services_Calls_When_All_Apps_Licensed()
        {
            // prepare data
            var fakeInput = new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput
                {
                    AllSelected = false,
                    Ids = new List<Guid> { Guid.NewGuid() },
                    UnselectedList = new List<Guid>()
                },
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput
                {
                    AllSelected = false,
                    Ids = new List<Guid> { Guid.NewGuid() },
                    UnselectedList = new List<Guid>()
                },
                NumberOfLicenses = 50
            };
            var fakeBundlesList = FakeBundlesList();
            var fakeLicensedTenantsList = FakeLicensedTenantList(1);
            var fakeBundleIdentifiers = fakeBundlesList.Select(b => b.Identifier).ToList();
            var fakeBundleIds = fakeBundlesList.Select(b => b.Id).ToList();
            var fakeLicensedTenantIds = fakeLicensedTenantsList.Select(l => l.Id).ToList();
            var bundleApps = await CreateBundleApps(fakeBundleIds[0], new List<Guid>{ Guid.NewGuid()});
            var bundleAppsIds = bundleApps.Select(b => b.AppId).ToList();
            var fakeIdInvoked = Guid.NewGuid();
            var fakeDictionary = new Dictionary<Guid, Guid>();
            fakeDictionary.Add(fakeLicensedTenantIds[0], fakeIdInvoked);
            var expectedBundleCreate = new LicensedBundleCreateInput
            {
                Status = LicensedBundleStatus.BundleActive,
                BundleId = fakeBundleIds[0],
                LicensedTenantId = fakeLicensedTenantIds[0],
                NumberOfLicenses = 50
            };
            var fakeAlreadyLicensedApp = new List<AlreadyLicensedApp>()
            {
                new AlreadyLicensedApp
                {
                    AppId = bundleApps[0].AppId,
                    LicensedTenantId = fakeLicensedTenantIds[0]
                }
            };
            
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(r => r.GetBundlesByIdsForBatchOperations(fakeInput.IdsToInsert.UnselectedList, fakeInput.IdsToInsert.Ids))
                .ReturnsAsync(fakeBundlesList);
            fakeBundleRepository.Setup(r => r.GetBundlesAlreadyInLicenses(fakeBundleIds, fakeLicensedTenantIds))
                .ReturnsAsync(new List<LicensedBundleGetForBatchOperation>());
            var fakeLicenseRepository =new Mock<ILicenseRepository>();
            fakeLicenseRepository.Setup(r => r.GetLicensesByIdsForBatchOperations(fakeInput.IdsWhereTheyWillBeInserted.UnselectedList, fakeInput.IdsWhereTheyWillBeInserted.Ids))
                .ReturnsAsync(fakeLicensedTenantIds);
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAppsAlreadyLicensed(bundleAppsIds, fakeLicensedTenantIds))
                .ReturnsAsync(fakeAlreadyLicensedApp);
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            fakeLicenseBatchRepository.Setup(r => r.GetLicensedTenantToIdentifierDictionary(fakeLicensedTenantIds))
                .ReturnsAsync(fakeDictionary);
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();;
            fakeLicensedTenantService
                .Setup(r => r.CreateLicensedBundle(new LicensedBundleCreateInput()));
            fakeLicensedTenantService
                .Setup(r => r.AddBundledAppsToLicense(Guid.NewGuid(), new LicensedBundleCreateInput()));
            fakeLicensedTenantService
                .Setup(r => r.PublishLicensingDetailsUpdatedEvents(new List<Guid>{fakeIdInvoked}));
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService
                .Setup(r => r.InvalidateCacheForTenants(new List<Guid>{fakeIdInvoked}));
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            fakeBathOperationLogger.Setup(r => r.LogInsertBundleInLicenses(expectedBundleCreate.NumberOfLicenses, fakeBundleIdentifiers, new List<Guid>{ fakeIdInvoked }));

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.InsertBundlesInLicenses(fakeInput);
            // test
            // TEST METHOD CALLS COUNT
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesByIdsForBatchOperations));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesAlreadyInLicenses));
            fakeLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetLicensesByIdsForBatchOperations));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppsAlreadyLicensed));
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.CreateLicensedBundle));
            fakeLicensedTenantService.Invocations.AssertCount(0, nameof(ILicensedTenantService.AddBundledAppsToLicense));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenants));
            // TEST ARGUMENTS IN METHOD
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), fakeBundleIds);
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundlesAlreadyInLicenses), fakeLicensedTenantIds);
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppsAlreadyLicensed), bundleAppsIds);
            fakeAppRepository.Invocations.AssertArgument(1, nameof(IAppRepository.GetAppsAlreadyLicensed), fakeLicensedTenantIds);
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary), fakeLicensedTenantIds);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.CreateLicensedBundle), expectedBundleCreate);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents), new List<Guid>{fakeIdInvoked});
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenants), new List<Guid>{fakeIdInvoked});
            fakeBathOperationLogger.Invocations.AssertArgument(0, nameof(IBatchOperationLoggerService.LogInsertBundleInLicenses), expectedBundleCreate.NumberOfLicenses);
            fakeBathOperationLogger.Invocations.AssertArgument(1, nameof(IBatchOperationLoggerService.LogInsertBundleInLicenses), fakeBundleIdentifiers);
            fakeBathOperationLogger.Invocations.AssertArgument(2, nameof(IBatchOperationLoggerService.LogInsertBundleInLicenses), new List<Guid>{ fakeIdInvoked });
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando bundles são adicionados para licenciamentos porém todos os bundles estão licenciados")]
        public async Task Insert_Bundles_In_Licenses_Check_Services_Calls_When_All_Is_Already_Licensed()
        {
            // prepare data
            var fakeInput = new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput{AllSelected = true},
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput{AllSelected = true},
                NumberOfLicenses = 50
            };
            var fakeBundlesList = FakeBundlesList();
            var fakeLicensedTenantsList = FakeLicensedTenantList(1);
            var fakeBundleIds = fakeBundlesList.Select(b => b.Id).ToList();
            var fakeLicensedTenantIds = fakeLicensedTenantsList.Select(l => l.Id).ToList();
            
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(r => r.GetAllBundlesForBatchOperation(null))
                .ReturnsAsync(fakeBundlesList);
            fakeBundleRepository.Setup(r => r.GetBundlesAlreadyInLicenses(fakeBundleIds, fakeLicensedTenantIds))
                .ReturnsAsync(new List<LicensedBundleGetForBatchOperation>{ new LicensedBundleGetForBatchOperation() });
            var fakeLicenseRepository =new Mock<ILicenseRepository>();
            fakeLicenseRepository.Setup(r => r.GetAllLicensesForBatchOperations(null))
                .ReturnsAsync(fakeLicensedTenantIds);
            var fakeAppRepository = new Mock<IAppRepository>();
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();;
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.InsertBundlesInLicenses(fakeInput);
            // test
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetAllBundlesForBatchOperation));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesAlreadyInLicenses));
            fakeLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetAllLicensesForBatchOperations));
            fakeAppRepository.Invocations.AssertCount(0, nameof(IAppRepository.GetAppsAlreadyLicensed));
            fakeLicenseBatchRepository.Invocations.AssertCount(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeLicensedTenantService.Invocations.AssertCount(0, nameof(ILicensedTenantService.CreateLicensedBundle));
            fakeLicensedTenantService.Invocations.AssertCount(0, nameof(ILicensedTenantService.AddBundledAppsToLicense));
            fakeLicensedTenantService.Invocations.AssertCount(0, nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvent));
            fakeLicensedTenantCacheService.Invocations.AssertCount(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenant));
            fakeBathOperationLogger.Invocations.AssertCount(0, nameof(IBatchOperationLoggerService.LogInsertBundleInLicenses));
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando apps são adicionados para licenciamentos")]
        public async Task Insert_Apps_In_Licenses_Check_Services_Calls()
        {
            // prepare data
            var fakeInput = new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput{AllSelected = true},
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput{AllSelected = true},
                NumberOfLicenses = 50
            };
            var fakeAppList = FakeAppList(1);
            var fakeAppIds = fakeAppList.Select(l => l.Id).ToList();
            var fakeAppIdentifiers = fakeAppList.Select(b => b.Identifier).ToList();
            var fakeLicensedTenantsList = FakeLicensedTenantList(1);
            var fakeLicensedTenantIds = fakeLicensedTenantsList.Select(l => l.Id).ToList();
            var fakeIdInvoked = Guid.NewGuid();
            var fakeDictionary = new Dictionary<Guid, Guid>();
            fakeDictionary.Add(fakeLicensedTenantIds[0], fakeIdInvoked);
            var expectedLicensedAppCreate = new LicensedAppCreateInput
            {
                Status = LicensedAppStatus.AppActive,
                AppId = fakeAppIds[0],
                LicensedTenantId = fakeLicensedTenantIds[0],
                NumberOfLicenses = 50
            };
            
            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            fakeLicenseRepository.Setup(r => r.GetAllLicensesForBatchOperations(null))
                .ReturnsAsync(fakeLicensedTenantIds);
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAllAppsForBatchOperation(null))
                .ReturnsAsync(fakeAppList);
            fakeAppRepository.Setup(r => r.GetAppsAlreadyLicensed(fakeAppIds, fakeLicensedTenantIds))
                .ReturnsAsync(new List<AlreadyLicensedApp>());
            var fakeBundleRepository = new Mock<IBundleRepository>();
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            fakeLicenseBatchRepository.Setup(r => r.GetLicensedTenantToIdentifierDictionary(fakeLicensedTenantIds))
                .ReturnsAsync(fakeDictionary);
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            fakeBathOperationLogger.Setup(r => r.LogAppsInLicenses(expectedLicensedAppCreate.NumberOfLicenses, fakeAppIdentifiers, new List<Guid> {fakeIdInvoked}));

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.InsertAppsInLicenses(fakeInput);
            // test
            // TEST METHOD CALLS COUNT
            fakeLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetAllLicensesForBatchOperations));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAllAppsForBatchOperation));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppsAlreadyLicensed));
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.CreateNewLicensedApp));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenants));
            fakeBathOperationLogger.Invocations.AssertSingle(nameof(IBatchOperationLoggerService.LogAppsInLicenses));
            // TEST ARGUMENTS IN METHOD
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppsAlreadyLicensed), fakeAppIds);
            fakeAppRepository.Invocations.AssertArgument(1, nameof(IAppRepository.GetAppsAlreadyLicensed), fakeLicensedTenantIds);
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary), fakeLicensedTenantIds);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.CreateNewLicensedApp), expectedLicensedAppCreate);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents), new List<Guid>{fakeIdInvoked});
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenants), new List<Guid>{fakeIdInvoked});
            fakeBathOperationLogger.Invocations.AssertArgument(0, nameof(IBatchOperationLoggerService.LogAppsInLicenses), expectedLicensedAppCreate.NumberOfLicenses);
            fakeBathOperationLogger.Invocations.AssertArgument(1, nameof(IBatchOperationLoggerService.LogAppsInLicenses), fakeAppIdentifiers);
            fakeBathOperationLogger.Invocations.AssertArgument(2, nameof(IBatchOperationLoggerService.LogAppsInLicenses), new List<Guid>{fakeIdInvoked});
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando apps são adicionados para licenciamentos e alguns já estão licenciados")]
        public async Task Insert_Apps_In_Licenses_Check_Services_Calls_When_Some_Apps_Already_Licensed()
        {
            // prepare data
            var fakeInput = new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput
                {
                    AllSelected = false,
                    Ids = new List<Guid> { Guid.NewGuid() },
                    UnselectedList = new List<Guid>()
                },
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput
                {
                    AllSelected = false,
                    Ids = new List<Guid> { Guid.NewGuid() },
                    UnselectedList = new List<Guid>()
                },
                NumberOfLicenses = 50
            };
            var fakeAppList = FakeAppList(1);
            var fakeAppIds = fakeAppList.Select(l => l.Id).ToList();
            var fakeAppIdentifiers = fakeAppList.Select(b => b.Identifier).ToList();
            var fakeLicensedTenantsList = FakeLicensedTenantList(2);
            var fakeLicensedTenantIds = fakeLicensedTenantsList.Select(l => l.Id).ToList();
            var fakeIdInvoked = Guid.NewGuid();
            var fakeDictionary = new Dictionary<Guid, Guid>();
            fakeDictionary.Add(fakeLicensedTenantIds[0], fakeIdInvoked);
            fakeDictionary.Add(fakeLicensedTenantIds[1], fakeIdInvoked);
            var expectedLicensedAppCreate = new LicensedAppCreateInput
            {
                Status = LicensedAppStatus.AppActive,
                AppId = fakeAppIds[0],
                LicensedTenantId = fakeLicensedTenantIds[0],
                NumberOfLicenses = 50
            };
            
            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            fakeLicenseRepository.Setup(r => r.GetLicensesByIdsForBatchOperations(fakeInput.IdsWhereTheyWillBeInserted.UnselectedList, fakeInput.IdsWhereTheyWillBeInserted.Ids))
                .ReturnsAsync(fakeLicensedTenantIds);
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAppsByIdsForBatchOperations(fakeInput.IdsToInsert.UnselectedList, fakeInput.IdsToInsert.Ids))
                .ReturnsAsync(fakeAppList);
            fakeAppRepository.Setup(r => r.GetAppsAlreadyLicensed(fakeAppIds, fakeLicensedTenantIds))
                .ReturnsAsync(new List<AlreadyLicensedApp>{ new AlreadyLicensedApp { AppId = fakeAppIds[0], LicensedTenantId = fakeLicensedTenantIds[1]}});
            var fakeBundleRepository = new Mock<IBundleRepository>();
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            fakeLicenseBatchRepository.Setup(r => r.GetLicensedTenantToIdentifierDictionary(fakeLicensedTenantIds))
                .ReturnsAsync(fakeDictionary);
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            fakeBathOperationLogger.Setup(r => r.LogAppsInLicenses(expectedLicensedAppCreate.NumberOfLicenses, fakeAppIdentifiers, new List<Guid> {fakeIdInvoked}));

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.InsertAppsInLicenses(fakeInput);
            // test
            // TEST METHOD CALLS COUNT
            fakeLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetLicensesByIdsForBatchOperations));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppsByIdsForBatchOperations));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppsAlreadyLicensed));
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.CreateNewLicensedApp));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenants));
            fakeBathOperationLogger.Invocations.AssertSingle(nameof(IBatchOperationLoggerService.LogAppsInLicenses));
            // TEST ARGUMENTS IN METHOD
            fakeLicenseRepository.Invocations.AssertArgument(0, nameof(ILicenseRepository.GetLicensesByIdsForBatchOperations), fakeInput.IdsWhereTheyWillBeInserted.UnselectedList);
            fakeLicenseRepository.Invocations.AssertArgument(1, nameof(ILicenseRepository.GetLicensesByIdsForBatchOperations), fakeInput.IdsWhereTheyWillBeInserted.Ids);
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppsByIdsForBatchOperations), fakeInput.IdsToInsert.UnselectedList);
            fakeAppRepository.Invocations.AssertArgument(1, nameof(IAppRepository.GetAppsByIdsForBatchOperations), fakeInput.IdsToInsert.Ids);
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppsAlreadyLicensed), fakeAppIds);
            fakeAppRepository.Invocations.AssertArgument(1, nameof(IAppRepository.GetAppsAlreadyLicensed), fakeLicensedTenantIds);
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary), fakeLicensedTenantIds);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.CreateNewLicensedApp), expectedLicensedAppCreate);
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents), new List<Guid>{fakeIdInvoked});
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenants), new List<Guid>{fakeIdInvoked});
            fakeBathOperationLogger.Invocations.AssertArgument(0, nameof(IBatchOperationLoggerService.LogAppsInLicenses), expectedLicensedAppCreate.NumberOfLicenses);
            fakeBathOperationLogger.Invocations.AssertArgument(1, nameof(IBatchOperationLoggerService.LogAppsInLicenses), fakeAppIdentifiers);
            fakeBathOperationLogger.Invocations.AssertArgument(2, nameof(IBatchOperationLoggerService.LogAppsInLicenses), new List<Guid>{fakeIdInvoked});
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando apps são adicionados para licenciamentos e todos já estão licenciados")]
        public async Task Insert_Apps_In_Licenses_Check_Services_Calls_When_All_Apps_Already_Licensed()
        {
            // prepare data
            var fakeInput = new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput{AllSelected = true},
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput{AllSelected = true},
                NumberOfLicenses = 50
            };
            var fakeAppList = FakeAppList(1);
            var fakeAppIds = fakeAppList.Select(l => l.Id).ToList();
            var fakeLicensedTenantsList = FakeLicensedTenantList(1);
            var fakeLicensedTenantIds = fakeLicensedTenantsList.Select(l => l.Id).ToList();

            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            fakeLicenseRepository.Setup(r => r.GetAllLicensesForBatchOperations(null))
                .ReturnsAsync(fakeLicensedTenantIds);
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAllAppsForBatchOperation(null))
                .ReturnsAsync(fakeAppList);
            fakeAppRepository.Setup(r => r.GetAppsAlreadyLicensed(fakeAppIds, fakeLicensedTenantIds))
                .ReturnsAsync(new List<AlreadyLicensedApp>{ new AlreadyLicensedApp()});
            var fakeBundleRepository = new Mock<IBundleRepository>();
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.InsertAppsInLicenses(fakeInput);
            // test
            // TEST METHOD CALLS COUNT
            fakeLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetAllLicensesForBatchOperations));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAllAppsForBatchOperation));
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppsAlreadyLicensed));
            fakeLicenseBatchRepository.Invocations.AssertCount(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeLicensedTenantService.Invocations.AssertCount(0, nameof(ILicensedTenantService.CreateNewLicensedApp));
            fakeLicensedTenantService.Invocations.AssertCount(0, nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents));
            fakeLicensedTenantCacheService.Invocations.AssertCount(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenants));
            fakeBathOperationLogger.Invocations.AssertCount(0, nameof(IBatchOperationLoggerService.LogAppsInLicenses));
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando apps são adicionados para bundles e os bundles não são licenciados")]
        public async Task Insert_Apps_In_Bundles_Check_Method_Returns_And_Services_Calls_When_All_Bundles_Not_Licensed()
        {
            // prepare data
            var fakeInput = new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput{AllSelected = true},
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput{AllSelected = true},
                NumberOfLicenses = 0
            };
            var fakeAppList = FakeAppList(1);
            var fakeAppIds = fakeAppList.Select(ac => ac.Id).ToList();
            var fakeAppIdentifiers = fakeAppList.Select(b => b.Identifier).ToList();
            var fakeBundlesList = FakeBundlesList();
            var fakeBundlesIds = fakeBundlesList.Select(ac => ac.Id).ToList();
            var fakeBundleIdentifiers = fakeBundlesList.Select(b => b.Identifier).ToList();
            var memoryRepository = ServiceProvider.GetService<IRepository<BundledApp>>();
            
            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAllAppsForBatchOperation(null))
                .ReturnsAsync(fakeAppList);
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(r => r.GetAllBundlesForBatchOperation(null))
                .ReturnsAsync(fakeBundlesList);
            fakeBundleRepository.Setup(b => b.GetBundlesIdsAlreadyLicensed(fakeBundlesIds))
                .ReturnsAsync(new List<Guid>());
            fakeBundleRepository.Setup(b => b.GetBundledApps(fakeAppIds, fakeBundlesIds))
                .ReturnsAsync(new List<BundledApp>());
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            fakeBathOperationLogger.Setup(r => r.LogInsertAppsInBundles(fakeAppIdentifiers, fakeBundleIdentifiers));

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            var result = await service.InsertAppsInBundles(fakeInput);
            // TEST METHOD RETURN 
            var singleInsertion = await memoryRepository.SingleAsync();
            Assert.Equal(singleInsertion.BundleId, fakeBundlesIds[0]);
            Assert.Equal(singleInsertion.AppId, fakeAppIds[0]);
            new List<LicensedBundleApp>().Should().BeEquivalentTo(result);
            // TEST METHOD CALLS COUNT
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAllAppsForBatchOperation));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetAllBundlesForBatchOperation));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesIdsAlreadyLicensed));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundledApps));
            fakeBathOperationLogger.Invocations.AssertSingle(nameof(IBatchOperationLoggerService.LogInsertAppsInBundles));
            // TEST METHOD INVOCATION
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesIdsAlreadyLicensed), fakeBundlesIds);
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundledApps), fakeAppIds);
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundledApps), fakeBundlesIds);
            fakeBathOperationLogger.Invocations.AssertArgument(0, nameof(IBatchOperationLoggerService.LogInsertAppsInBundles), fakeAppIdentifiers);
            fakeBathOperationLogger.Invocations.AssertArgument(1, nameof(IBatchOperationLoggerService.LogInsertAppsInBundles), fakeBundleIdentifiers);
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando apps são adicionados para bundles e bundles são licenciados")]
        public async Task Insert_Apps_In_Bundles_Check_Method_Returns_And_Services_Calls_When_Some_Bundles_Are_Licensed()
        {
            // prepare data
            var fakeInput = new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput{AllSelected = false, Ids = new List<Guid>(), UnselectedList = new List<Guid>()},
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput{AllSelected = false, Ids = new List<Guid>(), UnselectedList = new List<Guid>()},
                NumberOfLicenses = 0
            };
            var fakeAppList = FakeAppList(1);
            var fakeAppIds = fakeAppList.Select(ac => ac.Id).ToList();
            var fakeAppIdentifiers = fakeAppList.Select(b => b.Identifier).ToList();
            var fakeBundlesList = FakeBundlesList();
            var fakeBundlesIds = fakeBundlesList.Select(ac => ac.Id).ToList();
            var fakeBundleIdentifiers = fakeBundlesList.Select(b => b.Identifier).ToList();
            var memoryRepository = ServiceProvider.GetService<IRepository<BundledApp>>();
            var expectedOutput = new List<LicensedBundleApp>{new LicensedBundleApp {AppId = fakeAppIds[0], BundleId = fakeBundlesIds[0]}};
            
            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAppsByIdsForBatchOperations(fakeInput.IdsToInsert.UnselectedList, fakeInput.IdsToInsert.Ids))
                .ReturnsAsync(fakeAppList);
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(r => r.GetBundlesByIdsForBatchOperations(fakeInput.IdsWhereTheyWillBeInserted.UnselectedList, fakeInput.IdsWhereTheyWillBeInserted.Ids))
                .ReturnsAsync(fakeBundlesList);
            fakeBundleRepository.Setup(b => b.GetBundlesIdsAlreadyLicensed(fakeBundlesIds))
                .ReturnsAsync(new List<Guid>{ fakeBundlesIds[0] });
            fakeBundleRepository.Setup(b => b.GetBundledApps(fakeAppIds, fakeBundlesIds))
                .ReturnsAsync(new List<BundledApp>());
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            fakeBathOperationLogger.Setup(r => r.LogInsertAppsInBundles(fakeAppIdentifiers, fakeBundleIdentifiers));

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            var result = await service.InsertAppsInBundles(fakeInput);
            // TEST METHOD RETURN 
            var singleInsertion = await memoryRepository.SingleAsync();
            Assert.Equal(singleInsertion.BundleId, fakeBundlesIds[0]);
            Assert.Equal(singleInsertion.AppId, fakeAppIds[0]);
            expectedOutput.Should().BeEquivalentTo(result);
            // TEST METHOD CALLS COUNT
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppsByIdsForBatchOperations));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesByIdsForBatchOperations));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesIdsAlreadyLicensed));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundledApps));
            fakeBathOperationLogger.Invocations.AssertSingle(nameof(IBatchOperationLoggerService.LogInsertAppsInBundles));
            // TEST METHOD INVOCATION
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesByIdsForBatchOperations), fakeInput.IdsWhereTheyWillBeInserted.UnselectedList);
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundlesByIdsForBatchOperations), fakeInput.IdsWhereTheyWillBeInserted.Ids);
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppsByIdsForBatchOperations), fakeInput.IdsToInsert.UnselectedList);
            fakeAppRepository.Invocations.AssertArgument(1, nameof(IAppRepository.GetAppsByIdsForBatchOperations), fakeInput.IdsToInsert.Ids);
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesIdsAlreadyLicensed), fakeBundlesIds);
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundledApps), fakeAppIds);
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundledApps), fakeBundlesIds);
            fakeBathOperationLogger.Invocations.AssertArgument(0, nameof(IBatchOperationLoggerService.LogInsertAppsInBundles), fakeAppIdentifiers);
            fakeBathOperationLogger.Invocations.AssertArgument(1, nameof(IBatchOperationLoggerService.LogInsertAppsInBundles), fakeBundleIdentifiers);
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando apps são adicionados para bundles e alguns apps já estão em bundles")]
        public async Task Insert_Apps_In_Bundles_Check_Method_Returns_And_Services_Calls_When_Some_Apps_Is_In_Bundles()
        {
            // prepare data
            var fakeInput = new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput{AllSelected = false, Ids = new List<Guid>(), UnselectedList = new List<Guid>()},
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput{AllSelected = false, Ids = new List<Guid>(), UnselectedList = new List<Guid>()},
                NumberOfLicenses = 0
            };
            var fakeAppList = FakeAppList(1);
            var fakeAppIds = fakeAppList.Select(ac => ac.Id).ToList();
            var fakeAppIdentifiers = fakeAppList.Select(b => b.Identifier).ToList();
            var fakeBundlesList = FakeBundlesList();
            var fakeBundleIdentifiers = fakeBundlesList.Select(b => b.Identifier).ToList();
            fakeBundlesList.AddRange(FakeBundlesList());
            var fakeBundlesIds = fakeBundlesList.Select(ac => ac.Id).ToList();
            var memoryRepository = ServiceProvider.GetService<IRepository<BundledApp>>();

            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAppsByIdsForBatchOperations(fakeInput.IdsToInsert.UnselectedList, fakeInput.IdsToInsert.Ids))
                .ReturnsAsync(fakeAppList);
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(r => r.GetBundlesByIdsForBatchOperations(fakeInput.IdsWhereTheyWillBeInserted.UnselectedList, fakeInput.IdsWhereTheyWillBeInserted.Ids))
                .ReturnsAsync(fakeBundlesList);
            fakeBundleRepository.Setup(b => b.GetBundlesIdsAlreadyLicensed(fakeBundlesIds))
                .ReturnsAsync(new List<Guid>());
            fakeBundleRepository.Setup(b => b.GetBundledApps(fakeAppIds, fakeBundlesIds))
                .ReturnsAsync(new List<BundledApp> {new BundledApp { BundleId = fakeBundlesIds[1], AppId = fakeAppIds[0]}});
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            fakeBathOperationLogger.Setup(r => r.LogInsertAppsInBundles(fakeAppIdentifiers, fakeBundleIdentifiers));

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            var result = await service.InsertAppsInBundles(fakeInput);
            // TEST METHOD RETURN 
            var singleInsertion = await memoryRepository.SingleAsync();
            Assert.Equal(singleInsertion.BundleId, fakeBundlesIds[0]);
            Assert.Equal(singleInsertion.AppId, fakeAppIds[0]);
            new List<LicensedBundleApp>().Should().BeEquivalentTo(result);
            // TEST METHOD CALLS COUNT
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAppsByIdsForBatchOperations));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesByIdsForBatchOperations));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesIdsAlreadyLicensed));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundledApps));
            fakeBathOperationLogger.Invocations.AssertSingle(nameof(IBatchOperationLoggerService.LogInsertAppsInBundles));
            // TEST METHOD INVOCATION
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesByIdsForBatchOperations), fakeInput.IdsWhereTheyWillBeInserted.UnselectedList);
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundlesByIdsForBatchOperations), fakeInput.IdsWhereTheyWillBeInserted.Ids);
            fakeAppRepository.Invocations.AssertArgument(0, nameof(IAppRepository.GetAppsByIdsForBatchOperations), fakeInput.IdsToInsert.UnselectedList);
            fakeAppRepository.Invocations.AssertArgument(1, nameof(IAppRepository.GetAppsByIdsForBatchOperations), fakeInput.IdsToInsert.Ids);
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesIdsAlreadyLicensed), fakeBundlesIds);
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundledApps), fakeAppIds);
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundledApps), fakeBundlesIds);
            fakeBathOperationLogger.Invocations.AssertArgument(0, nameof(IBatchOperationLoggerService.LogInsertAppsInBundles), fakeAppIdentifiers);
            fakeBathOperationLogger.Invocations.AssertArgument(1, nameof(IBatchOperationLoggerService.LogInsertAppsInBundles), fakeBundleIdentifiers);
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando apps são adicionados para bundles e os apps já estão presentes em todos os bundles")]
        public async Task Insert_Apps_In_Bundles_Check_Method_Returns_And_Services_Calls_When_All_Apps_Already_In_Bundles()
        {
            // prepare data
            var fakeInput = new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput{AllSelected = true},
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput{AllSelected = true},
                NumberOfLicenses = 0
            };
            var fakeAppList = FakeAppList(1);
            var fakeAppIds = fakeAppList.Select(ac => ac.Id).ToList();
            var fakeBundlesList = FakeBundlesList();
            var fakeBundlesIds = fakeBundlesList.Select(ac => ac.Id).ToList();
            var memoryRepository = ServiceProvider.GetService<IRepository<BundledApp>>();
            
            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            var fakeAppRepository = new Mock<IAppRepository>();
            fakeAppRepository.Setup(r => r.GetAllAppsForBatchOperation(null))
                .ReturnsAsync(fakeAppList);
            var fakeBundleRepository = new Mock<IBundleRepository>();
            fakeBundleRepository.Setup(r => r.GetAllBundlesForBatchOperation(null))
                .ReturnsAsync(fakeBundlesList);
            fakeBundleRepository.Setup(b => b.GetBundlesIdsAlreadyLicensed(fakeBundlesIds))
                .ReturnsAsync(new List<Guid>());
            fakeBundleRepository.Setup(b => b.GetBundledApps(fakeAppIds, fakeBundlesIds))
                .ReturnsAsync(new List<BundledApp>{ new BundledApp()});
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            var result = await service.InsertAppsInBundles(fakeInput);
            // TEST METHOD RETURN 
            var singleInsertion = await memoryRepository.ToListAsync();
            Assert.Empty(singleInsertion);
            new List<LicensedBundleApp>().Should().BeEquivalentTo(result);
            // TEST METHOD CALLS COUNT
            fakeAppRepository.Invocations.AssertSingle(nameof(IAppRepository.GetAllAppsForBatchOperation));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetAllBundlesForBatchOperation));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundlesIdsAlreadyLicensed));
            fakeBundleRepository.Invocations.AssertSingle(nameof(IBundleRepository.GetBundledApps));
            // TEST METHOD INVOCATION
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundlesIdsAlreadyLicensed), fakeBundlesIds);
            fakeBundleRepository.Invocations.AssertArgument(0, nameof(IBundleRepository.GetBundledApps), fakeAppIds);
            fakeBundleRepository.Invocations.AssertArgument(1, nameof(IBundleRepository.GetBundledApps), fakeBundlesIds);
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando apps são adicionados para licenciamentos")]
        public async Task Insert_Apps_In_Licenses_From_Bundles_Check_Method_Returns_And_Services_Calls()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var fakeIdIdentifier = Guid.NewGuid();
            var fakeInput = new List<LicensedBundleApp> { new LicensedBundleApp{ BundleId = bundleId, AppId = appId}};
            var fakeTenantDictionary = new Dictionary<Guid, Guid>();
            fakeTenantDictionary.Add(licensedTenantId, fakeIdIdentifier);
            var fakeAppUnlicensed = new AppsGetForBatchOperations {Id = appId, Identifier = "Faint - Linkin Park"};
            var fakeApsUnLicensed = new List<LicensedBundlesWithUnLicensedAppsForBatchOperations>
            {
                new LicensedBundlesWithUnLicensedAppsForBatchOperations
                {
                    BundleId = bundleId,
                    LicensedTenantId = licensedTenantId,
                    AppsGetForBatchOperations = new List<AppsGetForBatchOperations>
                        {fakeAppUnlicensed}
                }
            };

            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            var fakeAppRepository = new Mock<IAppRepository>();
            var fakeBundleRepository = new Mock<IBundleRepository>();
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            fakeLicenseBatchRepository.Setup(l => l.GetLicensedBundlesWithUnLicensedAppsForBatchOperations(bundleId, new List<Guid>{ appId }))
                .ReturnsAsync(fakeApsUnLicensed);
            fakeLicenseBatchRepository
                .Setup(l => l.GetLicensedTenantToIdentifierDictionary(new List<Guid> {licensedTenantId}))
                .ReturnsAsync(fakeTenantDictionary);
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            fakeLicensedTenantService.Setup(l => l.AddBundledAppsToLicense(Guid.NewGuid(), new LicensedBundleCreateInput()));
            fakeLicensedTenantService
                .Setup(r => r.PublishLicensingDetailsUpdatedEvents(new List<Guid>{ fakeIdIdentifier }));
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService
                .Setup(r => r.InvalidateCacheForTenants(new List<Guid>{ fakeIdIdentifier }));
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            fakeBathOperationLogger.Setup(r => r.LogInsertAppsFromBundlesInLicenses(new List<string>{ fakeAppUnlicensed.Identifier }, new List<Guid>{ fakeIdIdentifier }));

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.InsertAppsFromBundlesInLicenses(fakeInput);
            // TEST METHOD CALLS COUNT
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations));
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.AddBundledAppsToLicense));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenants));
            fakeBathOperationLogger.Invocations.AssertSingle(nameof(IBatchOperationLoggerService.LogInsertAppsFromBundlesInLicenses));
            // TEST METHOD INVOCATION
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, nameof(ILicenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations), bundleId);
            fakeLicenseBatchRepository.Invocations.AssertArgument(1, nameof(ILicenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations), new List<Guid>{ appId });
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary), new List<Guid>{ licensedTenantId });
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenants), new List<Guid>{ fakeIdIdentifier });
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents), new List<Guid>{ fakeIdIdentifier });
            fakeBathOperationLogger.Invocations.AssertArgument(0, nameof(IBatchOperationLoggerService.LogInsertAppsFromBundlesInLicenses), new List<string>{ fakeAppUnlicensed.Identifier });
            fakeBathOperationLogger.Invocations.AssertArgument(1, nameof(IBatchOperationLoggerService.LogInsertAppsFromBundlesInLicenses), new List<Guid>{ fakeIdIdentifier });
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando apps são adicionados para licenciamentos quando alguns apps são adicinados pela iteração de um bundle")]
        public async Task Insert_Apps_In_Licenses_From_Bundles_Check_Method_Returns_And_Services_Calls_When_Apps_Is_Licensed_In_Loop()
        {
            // prepare data
            var bundleIdFirst = Guid.NewGuid();
            var bundleIdSecond = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var licensedTenantId = Guid.NewGuid();
            var fakeIdIdentifier = Guid.NewGuid();
            var fakeInput = new List<LicensedBundleApp> { new LicensedBundleApp{ BundleId = bundleIdFirst, AppId = appId}, new LicensedBundleApp{ BundleId = bundleIdSecond, AppId = appId}};
            var fakeTenantDictionary = new Dictionary<Guid, Guid>();
            fakeTenantDictionary.Add(licensedTenantId, fakeIdIdentifier);
            var fakeAppUnlicensed = new AppsGetForBatchOperations {Id = appId, Identifier = "Faint - Linkin Park"};
            var fakeApsUnLicensed = new List<LicensedBundlesWithUnLicensedAppsForBatchOperations>
            {
                new LicensedBundlesWithUnLicensedAppsForBatchOperations
                {
                    BundleId = bundleIdFirst,
                    LicensedTenantId = licensedTenantId,
                    AppsGetForBatchOperations = new List<AppsGetForBatchOperations>
                        {fakeAppUnlicensed}
                },
                new LicensedBundlesWithUnLicensedAppsForBatchOperations
                {
                    BundleId = bundleIdSecond,
                    LicensedTenantId = licensedTenantId,
                    AppsGetForBatchOperations = new List<AppsGetForBatchOperations>
                        {fakeAppUnlicensed}
                }
            };

            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            var fakeAppRepository = new Mock<IAppRepository>();
            var fakeBundleRepository = new Mock<IBundleRepository>();
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            fakeLicenseBatchRepository.Setup(l => l.GetLicensedBundlesWithUnLicensedAppsForBatchOperations(bundleIdFirst, new List<Guid>{appId}))
                .ReturnsAsync(new List<LicensedBundlesWithUnLicensedAppsForBatchOperations>{ fakeApsUnLicensed[0]});
            fakeLicenseBatchRepository.Setup(l => l.GetLicensedBundlesWithUnLicensedAppsForBatchOperations(bundleIdSecond, new List<Guid>{appId}))
                .ReturnsAsync(new List<LicensedBundlesWithUnLicensedAppsForBatchOperations>{ fakeApsUnLicensed[1]});
            fakeLicenseBatchRepository
                .Setup(l => l.GetLicensedTenantToIdentifierDictionary(new List<Guid> {licensedTenantId}))
                .ReturnsAsync(fakeTenantDictionary);
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            fakeLicensedTenantService.Setup(l => l.AddBundledAppsToLicense(Guid.NewGuid(), new LicensedBundleCreateInput()));
            fakeLicensedTenantService
                .Setup(r => r.PublishLicensingDetailsUpdatedEvents(new List<Guid>{ fakeIdIdentifier }));
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            fakeLicensedTenantCacheService
                .Setup(r => r.InvalidateCacheForTenants(new List<Guid>{ fakeIdIdentifier }));
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();
            fakeBathOperationLogger.Setup(r => r.LogInsertAppsFromBundlesInLicenses(new List<string>{ fakeAppUnlicensed.Identifier }, new List<Guid>{ fakeIdIdentifier }));

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.InsertAppsFromBundlesInLicenses(fakeInput);
            // TEST METHOD CALLS COUNT
            fakeLicenseBatchRepository.Invocations.AssertCount(2, nameof(ILicenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations));
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.AddBundledAppsToLicense));
            fakeLicensedTenantService.Invocations.AssertSingle(nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents));
            fakeLicensedTenantCacheService.Invocations.AssertSingle(nameof(ILicensedTenantCacheService.InvalidateCacheForTenants));
            fakeBathOperationLogger.Invocations.AssertSingle(nameof(IBatchOperationLoggerService.LogInsertAppsFromBundlesInLicenses));
            // TEST METHOD INVOCATION
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, 0, nameof(ILicenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations), bundleIdFirst);
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, 1, nameof(ILicenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations), new List<Guid>{ appId });
            fakeLicenseBatchRepository.Invocations.AssertArgument(1, 0, nameof(ILicenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations), bundleIdSecond);
            fakeLicenseBatchRepository.Invocations.AssertArgument(1, 1, nameof(ILicenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations), new List<Guid>{ appId });
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary), new List<Guid>{ licensedTenantId });
            fakeLicensedTenantCacheService.Invocations.AssertArgument(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenants), new List<Guid>{ fakeIdIdentifier });
            fakeLicensedTenantService.Invocations.AssertArgument(0, nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents), new List<Guid>{ fakeIdIdentifier });
            fakeBathOperationLogger.Invocations.AssertArgument(0, nameof(IBatchOperationLoggerService.LogInsertAppsFromBundlesInLicenses), new List<string>{ fakeAppUnlicensed.Identifier });
            fakeBathOperationLogger.Invocations.AssertArgument(1, nameof(IBatchOperationLoggerService.LogInsertAppsFromBundlesInLicenses), new List<Guid>{ fakeIdIdentifier });
        }
        
        [Fact(DisplayName = "Faz o teste das chamadas para outros serviços realizados pelo BatchOperationService quando apps são adicionados para licenciamentos porém todos já estão licenciados")]
        public async Task Insert_Apps_In_Licenses_From_Bundles_Check_Method_Returns_And_Services_Calls_When_No_Have_UnLicensed_Apps()
        {
            // prepare data
            var bundleId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var fakeInput = new List<LicensedBundleApp> { new LicensedBundleApp{ BundleId = bundleId, AppId = appId}};
            var fakeApsUnLicensed = new List<LicensedBundlesWithUnLicensedAppsForBatchOperations>();

            var fakeLicenseRepository = new Mock<ILicenseRepository>();
            var fakeAppRepository = new Mock<IAppRepository>();
            var fakeBundleRepository = new Mock<IBundleRepository>();
            var fakeLicenseBatchRepository = new Mock<ILicenseBatchRepository>();
            fakeLicenseBatchRepository.Setup(l => l.GetLicensedBundlesWithUnLicensedAppsForBatchOperations(bundleId, new List<Guid>{ appId }))
                .ReturnsAsync(fakeApsUnLicensed);
            var fakeLicensedTenantService = new Mock<ILicensedTenantService>();
            var fakeLicensedTenantCacheService = new Mock<ILicensedTenantCacheService>();
            var fakeBathOperationLogger = new Mock<IBatchOperationLoggerService>();

            var service = GetServiceWithMocking(fakeLicensedTenantService, fakeLicensedTenantCacheService, fakeAppRepository, fakeLicenseBatchRepository,
                fakeLicenseRepository, fakeBundleRepository, fakeBathOperationLogger);
            // execute
            await service.InsertAppsFromBundlesInLicenses(fakeInput);
            // TEST METHOD CALLS COUNT
            fakeLicenseBatchRepository.Invocations.AssertSingle(nameof(ILicenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations));
            fakeLicenseBatchRepository.Invocations.AssertCount(0, nameof(ILicenseBatchRepository.GetLicensedTenantToIdentifierDictionary));
            fakeLicensedTenantService.Invocations.AssertCount(0, nameof(ILicensedTenantService.AddBundledAppsToLicense));
            fakeLicensedTenantService.Invocations.AssertCount(0, nameof(ILicensedTenantService.PublishLicensingDetailsUpdatedEvents));
            fakeLicensedTenantCacheService.Invocations.AssertCount(0, nameof(ILicensedTenantCacheService.InvalidateCacheForTenants));
            fakeBathOperationLogger.Invocations.AssertCount(0, nameof(IBatchOperationLoggerService.LogInsertAppsFromBundlesInLicenses));
            // TEST METHOD INVOCATION
            fakeLicenseBatchRepository.Invocations.AssertArgument(0, nameof(ILicenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations), bundleId);
            fakeLicenseBatchRepository.Invocations.AssertArgument(1, nameof(ILicenseBatchRepository.GetLicensedBundlesWithUnLicensedAppsForBatchOperations), new List<Guid>{ appId });
        }

        public LicensedTenantBatchOperationsService GetServiceWithMocking(Mock<ILicensedTenantService> licensedTenantService, Mock<ILicensedTenantCacheService> licensedTenantCacheService,
            Mock<IAppRepository> appService, Mock<ILicenseBatchRepository> licenseBatchRepository, Mock<ILicenseRepository> licenseRepository, Mock<IBundleRepository> bundleRepository,
            Mock<IBatchOperationLoggerService> bathOperationLogger)
        {
            return ActivatorUtilities.CreateInstance<LicensedTenantBatchOperationsService>(ServiceProvider, licensedTenantCacheService.Object, licensedTenantService.Object,
                appService.Object, licenseBatchRepository.Object, licenseRepository.Object, bundleRepository.Object, bathOperationLogger.Object);
        }

        private List<Domain.Entities.LicensedBundle> FakeLicensedBundleList(Guid licensedTenant)
        {
            return new List<Domain.Entities.LicensedBundle>
            {
                new Domain.Entities.LicensedBundle
                {
                    Id = Guid.NewGuid(),
                    Status = LicensedBundleStatus.BundleActive,
                    LicensedTenantId = licensedTenant,
                    NumberOfLicenses = 1,
                    BundleId = Guid.NewGuid()
                }
            };
        }
        
        private List<BundlesGetForBatchOperations> FakeBundlesList()
        {
            return new List<BundlesGetForBatchOperations>
            {
                new BundlesGetForBatchOperations
                {
                    Id = Guid.NewGuid(),
                    Identifier = "Teste"
                }
            };
        }
        
        private List<AppsGetForBatchOperations> FakeAppList(int numberApps)
        {
            var output = new List<AppsGetForBatchOperations>();
            for (int i = 0; i < numberApps; i++)
            {
                output.Add(new AppsGetForBatchOperations
                {
                    Id = Guid.NewGuid(),
                    Identifier = "Teste",
                });
            }
            return output;
        }
        private List<Domain.Entities.LicensedTenant> FakeLicensedTenantList(int licensedTenants)
        {
            var output = new List<Domain.Entities.LicensedTenant>();
            for (int i = 0; i < licensedTenants; i++)
            {
                output.Add(new Domain.Entities.LicensedTenant
                {
                    Id = Guid.NewGuid()
                });
            }
            return output;
        }

        private async Task<List<BundledApp>> CreateBundleApps(Guid bundleId, List<Guid> appIds)
        {
            var memoryRepository = ServiceProvider.GetService<IRepository<BundledApp>>();
            var output = new List<BundledApp>();
            foreach (var appId in appIds)
            {
                var newBundleApp = new BundledApp
                {
                    Id = Guid.NewGuid(),
                    BundleId = bundleId,
                    AppId = appId
                };
                output.Add(newBundleApp);
                await memoryRepository.InsertAsync(newBundleApp, true);
            }
            return output;
        }
    }
}