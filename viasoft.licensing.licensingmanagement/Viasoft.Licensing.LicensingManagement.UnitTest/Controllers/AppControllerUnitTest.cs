using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.AppMessages;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.App;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Software;
using Viasoft.Licensing.LicensingManagement.Host.Controllers;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Controllers
{
    public class AppControllerUnitTest: LicensingManagementTestBase
    {
        [Fact(DisplayName = "Testa se a mensagem de App atualizado foi emitida")]
        public async Task Test_App_Updated_Publish_Message()
        {
            // Initialize Dependencies
            var memoryRepositoryApp = ServiceProvider.GetService<IRepository<App>>();
            var mockBundleRepository = new Mock<IBundleRepository>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockSoftwareRepository = new Mock<ISoftwareRepository>();
            var memoryRepositoryBundledApp = ServiceProvider.GetService<IRepository<BundledApp>>();
            var mockServiceBus = new Mock<IServiceBus>();
            
            // Initialize Data
            var initialApp = new App
            {
                Identifier = nameof(App.Identifier),
                Name = nameof(App.Name),
                TenantId = Guid.NewGuid(),
                SoftwareId = Guid.NewGuid(),
                IsDeleted = false,
                IsActive = true,
                Domain = Domain.Enums.Domain.Customized,
                Default = false
            };

            var savedApp = await memoryRepositoryApp.InsertAsync(initialApp, true);

            // Get Testing Objects 
            var controller = GetController(
                memoryRepositoryApp,
                mockBundleRepository.Object,
                mockLicenseRepository.Object,
                mockSoftwareRepository.Object,
                memoryRepositoryBundledApp,
                mockServiceBus.Object
            );
            
            var input = new AppUpdateInput
            {
                Id = savedApp.Id,
                Identifier = initialApp.Identifier,
                Name = $"{nameof(AppUpdateInput.Name)} Changed",
                IsActive = initialApp.IsActive,
                IsDefault = initialApp.Default,
                Domain = initialApp.Domain
            };
            
            // Method Calls
            await controller.Update(input);
            // Tests
            var result = await memoryRepositoryApp.FirstOrDefaultAsync();
            var expectedPublishMessage = new AppUpdatedMessage
            {
                Default = result.Default,
                Domain = result.Domain,
                Id = result.Id,
                Identifier = result.Identifier,
                Name = result.Name,
                SoftwareId = result.SoftwareId,
                IsActive = result.IsActive
            };
            mockServiceBus.Invocations.AssertSingle(nameof(IServiceBus.Publish));
            mockServiceBus.Invocations.AssertArgument(0, nameof(IServiceBus.Publish), expectedPublishMessage);
        }

        [Fact(DisplayName = "Testa se a mensagem de App atualizado NÃO foi emitida quando o update não foi realizado")]
        public async Task Test_App_Updated_Not_Publish_Message()
        {
            // Initialize Dependencies
            var memoryRepositoryApp = ServiceProvider.GetService<IRepository<App>>();
            var mockBundleRepository = new Mock<IBundleRepository>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockSoftwareRepository = new Mock<ISoftwareRepository>();
            var memoryRepositoryBundledApp = ServiceProvider.GetService<IRepository<BundledApp>>();
            var mockServiceBus = new Mock<IServiceBus>();
            
            // Initialize Data
            var initialApp = new App
            {
                Identifier = nameof(App.Identifier),
                Name = nameof(App.Name),
                TenantId = Guid.NewGuid(),
                SoftwareId = Guid.NewGuid(),
                IsDeleted = false,
                IsActive = true,
                Domain = Domain.Enums.Domain.Customized,
                Default = false
            };
            var initialApp2 = new App
            {
                Identifier = nameof(App.Identifier),
                Name = nameof(App.Name),
                TenantId = Guid.NewGuid(),
                SoftwareId = Guid.NewGuid(),
                IsDeleted = false,
                IsActive = true,
                Domain = Domain.Enums.Domain.Customized,
                Default = false
            };

            var savedApp = await memoryRepositoryApp.InsertAsync(initialApp, true);
            var savedApp2 = await memoryRepositoryApp.InsertAsync(initialApp2, true);

            // Get Testing Objects 
            var controller = GetController(
                memoryRepositoryApp,
                mockBundleRepository.Object,
                mockLicenseRepository.Object,
                mockSoftwareRepository.Object,
                memoryRepositoryBundledApp,
                mockServiceBus.Object
            );
            
            var input = new AppUpdateInput
            {
                Id = savedApp.Id,
                Identifier = initialApp.Identifier,
                Name = $"{nameof(AppUpdateInput.Name)} Changed",
                IsActive = initialApp.IsActive,
                IsDefault = initialApp.Default,
                Domain = initialApp.Domain
            };
            
            // Method Calls
            await controller.Update(input);
            
            // Tests
            mockServiceBus.Invocations.AssertCount(0, nameof(IServiceBus.Publish));
        }

        private AppController GetController(IRepository<App> memoryRepositoryApp, IBundleRepository mockBundleRepository, ILicenseRepository mockLicenseRepository, ISoftwareRepository licenseRepository, IRepository<BundledApp> memoryRepositoryBundledApp, IServiceBus serviceBus)
        {
            return ActivatorUtilities.CreateInstance<AppController>(ServiceProvider, 
                memoryRepositoryApp,
                mockBundleRepository,
                mockLicenseRepository,
                licenseRepository,
                memoryRepositoryBundledApp,
                serviceBus
            );
        }
    }
}