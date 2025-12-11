using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.FileQuota;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.Services.FileQuota;
using Viasoft.Licensing.LicensingManagement.Host.Controllers.FileQuota;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Controllers
{
    public class AppQuotaControllerUnitTest: LicensingManagementTestBase
    {
        [Fact(DisplayName = "Testa se esta realizando o insert de cota de App")]
        public async Task Test_Insert()
        {
            var input = new FileAppQuotaInput
            {
                LicensedTenantId = Guid.NewGuid(),
                AppId = Guid.NewGuid(),
                QuotaLimit = 0
            };
            var getQuotaAppDetailsResult = new QuotaAppDetails
            {
                Identifier = nameof(QuotaAppDetails.Identifier),
                Name = nameof(QuotaAppDetails.Name),
                AppId = Guid.NewGuid(),
                LicencedTenantIdentifier = Guid.NewGuid()
            };
            var call = new FileAppQuota
            {
                Id = Guid.NewGuid(),
                QuotaLimit = 1,
                AppId = getQuotaAppDetailsResult.Identifier,
                TenantId = Guid.NewGuid()
            };
            
            // Initialize Dependencies
            var mockFileQuotaCallerService = new Mock<IFileQuotaCallerService>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockFileQuotaViewService = new Mock<IFileQuotaViewService>();
            var memoryAppQuotaRepository = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var memoryAppRepository = ServiceProvider.GetService<IRepository<App>>();
            mockLicenseRepository.Setup(method => method
                    .GetQuotaAppDetailsByAppIdentifier(input.AppId, input.LicensedTenantId))
                .ReturnsAsync(getQuotaAppDetailsResult);
            mockFileQuotaViewService.Setup(m => m.DoesAppQuotaExists(input.LicensedTenantId, input.AppId))
                .ReturnsAsync(false);
            mockFileQuotaCallerService.Setup(method => method
                .AddOrUpdateFileAppQuota(getQuotaAppDetailsResult.LicencedTenantIdentifier, getQuotaAppDetailsResult.Identifier, input.QuotaLimit)
            ).ReturnsAsync(call);
            
            // Initialize Data

            // Get Testing Objects 
            var controller = GetController(mockFileQuotaCallerService.Object, mockLicenseRepository.Object, mockFileQuotaViewService.Object, memoryAppQuotaRepository, memoryLicensedAppRepository, memoryAppRepository);
            
            // Method Calls
            await controller.Insert(input);

            // Tests
            mockFileQuotaViewService.Invocations.AssertSingle(nameof(IFileQuotaViewService.DoesAppQuotaExists));
            mockFileQuotaViewService.Invocations.AssertArgument(0, nameof(IFileQuotaViewService.DoesAppQuotaExists), input.LicensedTenantId);
            mockFileQuotaViewService.Invocations.AssertArgument(1, nameof(IFileQuotaViewService.DoesAppQuotaExists), input.AppId);
            mockLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier));
            mockLicenseRepository.Invocations.AssertArgument(0, nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier), input.AppId);
            mockLicenseRepository.Invocations.AssertArgument(1, nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier), input.LicensedTenantId);
            mockFileQuotaCallerService.Invocations.AssertSingle(nameof(IFileQuotaCallerService.AddOrUpdateFileAppQuota));
            mockFileQuotaCallerService.Invocations.AssertArgument(0, nameof(IFileQuotaCallerService.AddOrUpdateFileAppQuota), getQuotaAppDetailsResult.LicencedTenantIdentifier);
            mockFileQuotaCallerService.Invocations.AssertArgument(1, nameof(IFileQuotaCallerService.AddOrUpdateFileAppQuota), getQuotaAppDetailsResult.Identifier);
            mockFileQuotaCallerService.Invocations.AssertArgument(2, nameof(IFileQuotaCallerService.AddOrUpdateFileAppQuota), input.QuotaLimit);
            mockFileQuotaViewService.Invocations.AssertSingle(nameof(IFileQuotaViewService.AddAppQuotaView));
            mockFileQuotaViewService.Invocations.AssertArgument(0, nameof(IFileQuotaViewService.AddAppQuotaView), input.LicensedTenantId);
            mockFileQuotaViewService.Invocations.AssertArgument(1, nameof(IFileQuotaViewService.AddAppQuotaView), input.AppId);
            mockFileQuotaViewService.Invocations.AssertArgument(2, nameof(IFileQuotaViewService.AddAppQuotaView), getQuotaAppDetailsResult.Name);
            mockFileQuotaViewService.Invocations.AssertArgument(3, nameof(IFileQuotaViewService.AddAppQuotaView), call.QuotaLimit);
        }
        
        [Fact(DisplayName = "Testa se esta realizando o insert de cota de App já existente")]
        public async Task Test_Insert_Failed()
        {
            var input = new FileAppQuotaInput
            {
                LicensedTenantId = Guid.NewGuid(),
                AppId = Guid.NewGuid(),
                QuotaLimit = 0
            };
            
            // Initialize Dependencies
            var mockFileQuotaCallerService = new Mock<IFileQuotaCallerService>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockFileQuotaViewService = new Mock<IFileQuotaViewService>();
            var memoryAppQuotaRepository = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var memoryAppRepository = ServiceProvider.GetService<IRepository<App>>();
            mockLicenseRepository.Setup(method => method
                    .GetQuotaAppDetailsByAppIdentifier(Guid.Empty, Guid.Empty))
                .ReturnsAsync((QuotaAppDetails) null);
            mockFileQuotaViewService.Setup(m => m.DoesAppQuotaExists(input.LicensedTenantId, input.AppId))
                .ReturnsAsync(true);
            
            // Initialize Data

            // Get Testing Objects 
            var controller = GetController(mockFileQuotaCallerService.Object, mockLicenseRepository.Object, mockFileQuotaViewService.Object, memoryAppQuotaRepository, memoryLicensedAppRepository, memoryAppRepository);
            
            // Method Calls
            await controller.Insert(input);
            
            var result = await memoryAppQuotaRepository.FirstOrDefaultAsync();
            
            // Tests
            Assert.Null(result);
            mockFileQuotaViewService.Invocations.AssertSingle(nameof(IFileQuotaViewService.DoesAppQuotaExists));
            mockFileQuotaViewService.Invocations.AssertArgument(0, nameof(IFileQuotaViewService.DoesAppQuotaExists), input.LicensedTenantId);
            mockFileQuotaViewService.Invocations.AssertArgument(1, nameof(IFileQuotaViewService.DoesAppQuotaExists), input.AppId);
            mockLicenseRepository.Invocations.AssertCount(0, nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier));
            mockFileQuotaCallerService.Invocations.AssertCount(0, nameof(IFileQuotaCallerService.AddOrUpdateFileAppQuota));
            mockFileQuotaViewService.Invocations.AssertCount(0, nameof(IFileQuotaViewService.AddAppQuotaView));
        }
        
        
        [Fact(DisplayName = "Testa se esta realizando o update")]
        public async Task Test_Update()
        {
           // Initialize Dependencies
            var mockFileQuotaCallerService = new Mock<IFileQuotaCallerService>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockFileQuotaViewService = new Mock<IFileQuotaViewService>();
            var memoryAppQuotaRepository = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var memoryAppRepository = ServiceProvider.GetService<IRepository<App>>();
            var getQuotaAppDetailsResult = new QuotaAppDetails
            {
                Identifier = nameof(QuotaAppDetails.Identifier),
                Name = nameof(QuotaAppDetails.Name),
                AppId = Guid.NewGuid(),
                LicencedTenantIdentifier = Guid.NewGuid()
            };
            
            // Initialize Data
            var input = new FileAppQuotaInput
            {
                LicensedTenantId = Guid.NewGuid(),
                AppId = Guid.NewGuid(),
                QuotaLimit = 0
            };
            var call = new FileAppQuota
            {
                Id = Guid.NewGuid(),
                QuotaLimit = 1,
                AppId = getQuotaAppDetailsResult.Identifier,
                TenantId = Guid.NewGuid()
            };
            mockLicenseRepository.Setup(method => method
                    .GetQuotaAppDetailsByAppIdentifier(input.AppId, input.LicensedTenantId))
                .ReturnsAsync(getQuotaAppDetailsResult);
            mockFileQuotaCallerService.Setup(method => method
                .AddOrUpdateFileAppQuota(getQuotaAppDetailsResult.LicencedTenantIdentifier, getQuotaAppDetailsResult.Identifier, input.QuotaLimit)
            ).ReturnsAsync(call);

            // Get Testing Objects 
            var controller = GetController(mockFileQuotaCallerService.Object, mockLicenseRepository.Object, mockFileQuotaViewService.Object, memoryAppQuotaRepository, memoryLicensedAppRepository, memoryAppRepository);
            
            // Method Calls
            await controller.Update(input);
            
            // Tests
            mockLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier));
            mockLicenseRepository.Invocations.AssertArgument(0, nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier), input.AppId);
            mockLicenseRepository.Invocations.AssertArgument(1, nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier), input.LicensedTenantId);
            mockFileQuotaCallerService.Invocations.AssertSingle(nameof(IFileQuotaCallerService.AddOrUpdateFileAppQuota));
            mockFileQuotaViewService.Invocations.AssertSingle(nameof(IFileQuotaViewService.UpdateAppQuotaView));
            mockFileQuotaViewService.Invocations.AssertArgument(0, nameof(IFileQuotaViewService.UpdateAppQuotaView), input.LicensedTenantId);
            mockFileQuotaViewService.Invocations.AssertArgument(1, nameof(IFileQuotaViewService.UpdateAppQuotaView), input.AppId);
            mockFileQuotaViewService.Invocations.AssertArgument(2, nameof(IFileQuotaViewService.UpdateAppQuotaView), call.QuotaLimit);
        }
        
        [Fact(DisplayName = "Testa se esta realizando o add de cota de App")]
        public async Task Test_Add_App_Quota()
        {
            // Initialize Dependencies
            var mockFileQuotaCallerService = new Mock<IFileQuotaCallerService>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockFileQuotaViewService = new Mock<IFileQuotaViewService>();
            var memoryAppQuotaRepository = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var memoryAppRepository = ServiceProvider.GetService<IRepository<App>>();
            mockLicenseRepository.Setup(method => method
                    .GetQuotaAppDetailsByAppIdentifier(Guid.Empty, Guid.Empty))
                .ReturnsAsync((QuotaAppDetails) null);
            
            // Initialize Data
            var input = new FileAppQuotaInput
            {
                LicensedTenantId = Guid.NewGuid(),
                AppId = Guid.NewGuid(),
                QuotaLimit = 0
            };

            // Get Testing Objects 
            var controller = GetController(mockFileQuotaCallerService.Object, mockLicenseRepository.Object, mockFileQuotaViewService.Object, memoryAppQuotaRepository, memoryLicensedAppRepository, memoryAppRepository);
            
            // Method Calls
            await Assert.ThrowsAsync<ArgumentException>(() => controller.AddOrUpdateAppQuota(input));
            
            // Tests
            mockLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier));
            mockLicenseRepository.Invocations.AssertArgument(0, nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier), input.AppId);
            mockLicenseRepository.Invocations.AssertArgument(1, nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier), input.LicensedTenantId);
            mockFileQuotaCallerService.Invocations.AssertCount(0, nameof(IFileQuotaCallerService.AddOrUpdateFileAppQuota));
            mockFileQuotaViewService.Invocations.AssertCount(0, nameof(IFileQuotaViewService.AddOrUpdateAppQuotaView));
        }

        [Fact(DisplayName = "Testa se esta realizando o update de cota de App")]
        public async Task Test_Update_App_Quota()
        {
            // Initialize Dependencies
            var mockFileQuotaCallerService = new Mock<IFileQuotaCallerService>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockFileQuotaViewService = new Mock<IFileQuotaViewService>();
            var memoryAppQuotaRepository = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var memoryAppRepository = ServiceProvider.GetService<IRepository<App>>();
            var getQuotaAppDetailsResult = new QuotaAppDetails
            {
                Identifier = nameof(QuotaAppDetails.Identifier),
                Name = nameof(QuotaAppDetails.Name),
                AppId = Guid.NewGuid(),
                LicencedTenantIdentifier = Guid.NewGuid()
            };
            
            // Initialize Data
            var input = new FileAppQuotaInput
            {
                LicensedTenantId = Guid.NewGuid(),
                AppId = Guid.NewGuid(),
                QuotaLimit = 0
            };
            mockLicenseRepository.Setup(method => method
                    .GetQuotaAppDetailsByAppIdentifier(input.AppId, input.LicensedTenantId))
                .ReturnsAsync(getQuotaAppDetailsResult);
            mockFileQuotaCallerService.Setup(method => method
                .AddOrUpdateFileAppQuota(getQuotaAppDetailsResult.LicencedTenantIdentifier, getQuotaAppDetailsResult.Identifier, input.QuotaLimit)
            ).ReturnsAsync(new FileAppQuota());

            // Get Testing Objects 
            var controller = GetController(mockFileQuotaCallerService.Object, mockLicenseRepository.Object, mockFileQuotaViewService.Object, memoryAppQuotaRepository, memoryLicensedAppRepository, memoryAppRepository);
            
            // Method Calls
            await controller.AddOrUpdateAppQuota(input);
            
            // Tests
            mockLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier));
            mockLicenseRepository.Invocations.AssertArgument(0, nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier), input.AppId);
            mockLicenseRepository.Invocations.AssertArgument(1, nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier), input.LicensedTenantId);
            mockFileQuotaCallerService.Invocations.AssertSingle(nameof(IFileQuotaCallerService.AddOrUpdateFileAppQuota));
            mockFileQuotaViewService.Invocations.AssertSingle(nameof(IFileQuotaViewService.AddOrUpdateAppQuotaView));
        }
        
        [Fact(DisplayName = "Testa a falha ao realizar a call para o fileProvider durante o processo de adição de cota de App")]
        public async Task Test_FileProvider_Call_Failed_During_Add_App_Quota()
        {
            // Initialize Dependencies
            var mockFileQuotaCallerService = new Mock<IFileQuotaCallerService>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockFileQuotaViewService = new Mock<IFileQuotaViewService>();
            var memoryAppQuotaRepository = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var memoryAppRepository = ServiceProvider.GetService<IRepository<App>>();
            var getQuotaAppDetailsResult = new QuotaAppDetails
            {
                Identifier = nameof(QuotaAppDetails.Identifier),
                Name = nameof(QuotaAppDetails.Name),
                AppId = Guid.NewGuid(),
                LicencedTenantIdentifier = Guid.NewGuid()
            };
            
            // Initialize Data
            var input = new FileAppQuotaInput
            {
                LicensedTenantId = Guid.NewGuid(),
                AppId = Guid.NewGuid(),
                QuotaLimit = 0
            };
            
            mockLicenseRepository.Setup(method => method
                    .GetQuotaAppDetailsByAppIdentifier(input.AppId, input.LicensedTenantId))
                .ReturnsAsync(getQuotaAppDetailsResult);
            mockFileQuotaCallerService.Setup(method => method
                .AddOrUpdateFileAppQuota(Guid.Empty, string.Empty, 0)
            ).ReturnsAsync((FileAppQuota) null);

            // Get Testing Objects 
            var controller = GetController(mockFileQuotaCallerService.Object, mockLicenseRepository.Object, mockFileQuotaViewService.Object, memoryAppQuotaRepository, memoryLicensedAppRepository, memoryAppRepository);
            
            // Method Calls
            await Assert.ThrowsAsync<HttpRequestException>(() => controller.AddOrUpdateAppQuota(input));
            
            // Tests
            mockLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier));
            mockLicenseRepository.Invocations.AssertArgument(0, nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier), input.AppId);
            mockLicenseRepository.Invocations.AssertArgument(1, nameof(ILicenseRepository.GetQuotaAppDetailsByAppIdentifier), input.LicensedTenantId);
            mockFileQuotaCallerService.Invocations.AssertSingle(nameof(IFileQuotaCallerService.AddOrUpdateFileAppQuota));
            mockFileQuotaViewService.Invocations.AssertCount(0, nameof(IFileQuotaViewService.AddOrUpdateAppQuotaView));
        }
        
        [Fact(DisplayName = "Testa o retorno de cota de App")]
        public async Task Test_Get_App_Quota()
        {
            // Initialize Dependencies
            var mockFileQuotaCallerService = new Mock<IFileQuotaCallerService>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockFileQuotaViewService = new Mock<IFileQuotaViewService>();
            var memoryAppQuotaRepository = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryLicensedAppRepository = ServiceProvider.GetService<IRepository<LicensedApp>>();
            var memoryAppRepository = ServiceProvider.GetService<IRepository<App>>();
            mockFileQuotaViewService.Setup(method => method
                    .GetAppQuota(Guid.Empty, Guid.Empty))
                .ReturnsAsync(new FileAppQuotaView());
            
            // Initialize Data
            var licensedTenantId = Guid.NewGuid();
            var appId = Guid.NewGuid();

            // Get Testing Objects 
            var controller = GetController(mockFileQuotaCallerService.Object, mockLicenseRepository.Object, mockFileQuotaViewService.Object, memoryAppQuotaRepository, memoryLicensedAppRepository, memoryAppRepository);
            
            // Method Calls
            var result = await controller.GetAppQuota(licensedTenantId, appId);
            
            // Tests
            mockFileQuotaViewService.Invocations.AssertSingle(nameof(IFileQuotaViewService.GetAppQuota));
            mockFileQuotaViewService.Invocations.AssertArgument(0, nameof(IFileQuotaViewService.GetAppQuota), licensedTenantId);
            mockFileQuotaViewService.Invocations.AssertArgument(1, nameof(IFileQuotaViewService.GetAppQuota), appId);
        }

        private AppQuotaController GetController(IFileQuotaCallerService mockFileQuotaCallerService,
            ILicenseRepository mockLicenseRepository, IFileQuotaViewService mockFileQuotaViewService,
            IRepository<FileAppQuotaView> memoryAppQuotaRepository,
            IRepository<LicensedApp> memoryLicensedAppRepository, IRepository<App> memoryAppRepository)
        {
            return ActivatorUtilities.CreateInstance<AppQuotaController>(ServiceProvider, 
                mockFileQuotaCallerService, mockLicenseRepository, mockFileQuotaViewService, 
                memoryAppQuotaRepository, memoryLicensedAppRepository, memoryAppRepository);
        }
    }
}