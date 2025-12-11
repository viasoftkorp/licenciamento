using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp;
using Viasoft.Licensing.LicensingManagement.Domain.Services.BatchOperationServices;
using Viasoft.Licensing.LicensingManagement.Host.Controllers;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Controllers
{
    public class LicensedTenantBatchOperationsControllerUnitTest: LicensingManagementTestBase
    {
        
        [Fact(DisplayName = "Testa se a controller está chamando corretamente o serviço de inserir um novo app aos licenciamentos")]
        public async Task Check_Controller_Call_Correctly_LicensedTenantBatchOperations_InsertNewApp()
        {
            // prepare data
            var fakeLicensedTenantBatchOperationsService = new Mock<ILicensedTenantBatchOperationsService>();
            var fakeInput = new List<LicensedBundleApp> { new LicensedBundleApp{ BundleId = Guid.NewGuid(), AppId = Guid.NewGuid()} };
            fakeLicensedTenantBatchOperationsService
                .Setup(r => r.InsertAppsFromBundlesInLicenses(fakeInput));
            var controller = GetController(fakeLicensedTenantBatchOperationsService);
            // execute
            await controller.InsertAppsFromBundlesInLicenses(fakeInput);
            // test
            fakeLicensedTenantBatchOperationsService.Invocations.AssertSingle(nameof(ILicensedTenantBatchOperationsService.InsertAppsFromBundlesInLicenses));
            fakeLicensedTenantBatchOperationsService.Invocations.AssertArgument(0, nameof(ILicensedTenantBatchOperationsService.InsertAppsFromBundlesInLicenses), fakeInput);
        }
        
        [Fact(DisplayName = "Testa se a controller está chamando corretamente o serviço de remover app de licenciamentos")]
        public async Task Check_Controller_Call_Correctly_LicensedTenantBatchOperations_RemoveApp()
        {
            // prepare data
            var fakeLicensedTenantBatchOperationsService = new Mock<ILicensedTenantBatchOperationsService>();
            var fakeDeleteInput = FakeBundleAppDelete();
            fakeLicensedTenantBatchOperationsService
                .Setup(r => r.RemoveAppFromBundleInLicenses(fakeDeleteInput.AppId, fakeDeleteInput.BundleId));
            var controller = GetController(fakeLicensedTenantBatchOperationsService);
            // execute
            await controller.RemoveAppFromBundleInLicenses(fakeDeleteInput);
            // test
            fakeLicensedTenantBatchOperationsService.Invocations.AssertSingle(nameof(ILicensedTenantBatchOperationsService.RemoveAppFromBundleInLicenses));
            fakeLicensedTenantBatchOperationsService.Invocations.AssertArgument(1, nameof(ILicensedTenantBatchOperationsService.RemoveAppFromBundleInLicenses), fakeDeleteInput.AppId);
            fakeLicensedTenantBatchOperationsService.Invocations.AssertArgument(0, nameof(ILicensedTenantBatchOperationsService.RemoveAppFromBundleInLicenses), fakeDeleteInput.BundleId);
        }
        
        [Fact(DisplayName = "Testa se a controller está chamando corretamente o serviço de inserir bundles em licenciamentos")]
        public async Task Check_Controller_Call_Correctly_LicensedTenantBatchOperations_Insert_Bundles_In_Licenses()
        {
            // prepare data
            var fakeLicensedTenantBatchOperationsService = new Mock<ILicensedTenantBatchOperationsService>();
            var fakeBatchOperationsInput = FakeBatchOperationsInput();
            fakeLicensedTenantBatchOperationsService
                .Setup(r => r.InsertBundlesInLicenses(fakeBatchOperationsInput));
            var controller = GetController(fakeLicensedTenantBatchOperationsService);
            // execute
            await controller.InsertBundlesInLicenses(fakeBatchOperationsInput);
            // test
            fakeLicensedTenantBatchOperationsService.Invocations.AssertSingle(nameof(ILicensedTenantBatchOperationsService.InsertBundlesInLicenses));
            fakeLicensedTenantBatchOperationsService.Invocations.AssertArgument(0, nameof(ILicensedTenantBatchOperationsService.InsertBundlesInLicenses), fakeBatchOperationsInput);
        }
        
        [Fact(DisplayName = "Testa se a controller está chamando corretamente o serviço de inserir apps em pacotes")]
        public async Task Check_Controller_Call_Correctly_LicensedTenantBatchOperations_Insert_Apps_In_Bundles()
        {
            // prepare data
            var fakeLicensedTenantBatchOperationsService = new Mock<ILicensedTenantBatchOperationsService>();
            var fakeBatchOperationsInput = FakeBatchOperationsInput();
            fakeLicensedTenantBatchOperationsService
                .Setup(r => r.InsertAppsInBundles(fakeBatchOperationsInput));
            var controller = GetController(fakeLicensedTenantBatchOperationsService);
            // execute
            await controller.InsertAppsInBundles(fakeBatchOperationsInput);
            // test
            fakeLicensedTenantBatchOperationsService.Invocations.AssertSingle(nameof(ILicensedTenantBatchOperationsService.InsertAppsInBundles));
            fakeLicensedTenantBatchOperationsService.Invocations.AssertArgument(0, nameof(ILicensedTenantBatchOperationsService.InsertAppsInBundles), fakeBatchOperationsInput);
        }
        
        [Fact(DisplayName = "Testa se a controller está chamando corretamente o serviço de inserir apps em pacotes")]
        public async Task Check_Controller_Call_Correctly_LicensedTenantBatchOperations_Insert_Apps_In_Licenses()
        {
            // prepare data
            var fakeLicensedTenantBatchOperationsService = new Mock<ILicensedTenantBatchOperationsService>();
            var fakeInput = new List<LicensedBundleApp> { new LicensedBundleApp{ BundleId = Guid.NewGuid(), AppId = Guid.NewGuid()} };
            fakeLicensedTenantBatchOperationsService
                .Setup(r => r.InsertAppsFromBundlesInLicenses(fakeInput));
            var controller = GetController(fakeLicensedTenantBatchOperationsService);
            // execute
            await controller.InsertAppsFromBundlesInLicenses(fakeInput);
            // test
            fakeLicensedTenantBatchOperationsService.Invocations.AssertSingle(nameof(ILicensedTenantBatchOperationsService.InsertAppsFromBundlesInLicenses));
            fakeLicensedTenantBatchOperationsService.Invocations.AssertArgument(0, nameof(ILicensedTenantBatchOperationsService.InsertAppsFromBundlesInLicenses), fakeInput);
        }
        

        private LicensedTenantBatchOperationsController GetController(Mock<ILicensedTenantBatchOperationsService> fakeLicensedTenantBatchOperationsService)
        {
            return new LicensedTenantBatchOperationsController(fakeLicensedTenantBatchOperationsService.Object);
        }

        private BundledAppDeleteInput FakeBundleAppDelete()
        {
            return new BundledAppDeleteInput
            {
                AppId = Guid.NewGuid(),
                BundleId = Guid.NewGuid()
            };
        }
            
        private BatchOperationsInput FakeBatchOperationsInput()
        {
            return new BatchOperationsInput
            {
                IdsToInsert = new InsertBatchOperationsInput
                {
                    Ids = new List<Guid>(),
                    AllSelected = false,
                    UnselectedList = new List<Guid>()
                },
                IdsWhereTheyWillBeInserted = new InsertBatchOperationsInput
                {
                    Ids = new List<Guid>(),
                    AllSelected = false,
                    UnselectedList = new List<Guid>()
                },
                NumberOfLicenses = 5
            };
        }
    }
}