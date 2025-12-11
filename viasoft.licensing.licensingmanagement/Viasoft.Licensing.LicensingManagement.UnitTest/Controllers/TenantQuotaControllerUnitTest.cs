using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.FileQuota;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.Services.FileQuota;
using Viasoft.Licensing.LicensingManagement.Host.Controllers.FileQuota;
using Xunit;
using FileTenantQuota = Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota.FileTenantQuota;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Controllers
{
    public class TenantQuotaControllerUnitTest: LicensingManagementTestBase
    {
        [Fact(DisplayName = "Testa se esta realizando o add de cota de Tenant")]
        public async Task Test_Add_Tenant_Quota()
        {
            // Initialize Dependencies
            var mockFileQuotaCallerService = new Mock<IFileQuotaCallerService>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockFileQuotaViewService = new Mock<IFileQuotaViewService>();
            
            // Initialize Data
            var input = new FileTenantQuotaInput
            {
                LicenseTenantId = Guid.NewGuid(),
                QuotaLimit = 0
            };
            
            mockLicenseRepository.Setup(method => method
                    .GetIdentifierFromLicenseTenantIdExistence(input.LicenseTenantId))
                .ReturnsAsync(Guid.Empty);

            // Get Testing Objects 
            var controller = GetController(mockFileQuotaCallerService.Object, mockLicenseRepository.Object, mockFileQuotaViewService.Object);
            
            // Method Calls
            await Assert.ThrowsAsync<ArgumentException>(() => controller.AddOrUpdateTenantQuota(input));
            
            // Tests
            mockLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetIdentifierFromLicenseTenantIdExistence));
            mockLicenseRepository.Invocations.AssertArgument(0, nameof(ILicenseRepository.GetIdentifierFromLicenseTenantIdExistence), input.LicenseTenantId);
            mockFileQuotaCallerService.Invocations.AssertCount(0, nameof(IFileQuotaCallerService.AddOrUpdateFileTenantQuota));
            mockFileQuotaViewService.Invocations.AssertCount(0, nameof(IFileQuotaViewService.AddOrUpdateTenantQuotaView));
        }

        [Fact(DisplayName = "Testa se esta realizando o update de cota de Tenant")]
        public async Task Test_Update_Tenant_Quota()
        {
            // Initialize Dependencies
            var mockFileQuotaCallerService = new Mock<IFileQuotaCallerService>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockFileQuotaViewService = new Mock<IFileQuotaViewService>();
            var getTenantQuotaId = Guid.NewGuid();
            
            // Initialize Data
            var input = new FileTenantQuotaInput
            {
                LicenseTenantId = Guid.NewGuid(),
                QuotaLimit = 0
            };
            var callOutput = new Domain.DTOs.FileQuota.FileTenantQuota
            {
                TenantId = getTenantQuotaId,
                QuotaLimit = 0,
            };
            
            mockLicenseRepository.Setup(method => method
                    .GetIdentifierFromLicenseTenantIdExistence(input.LicenseTenantId))
                .ReturnsAsync(getTenantQuotaId);
            mockFileQuotaCallerService.Setup(method => method
                .AddOrUpdateFileTenantQuota(getTenantQuotaId, input.QuotaLimit)
            ).ReturnsAsync(callOutput);

            // Get Testing Objects 
            var controller = GetController(mockFileQuotaCallerService.Object, mockLicenseRepository.Object, mockFileQuotaViewService.Object);
            
            // Method Calls
            await controller.AddOrUpdateTenantQuota(input);
            
            // Tests
            mockLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetIdentifierFromLicenseTenantIdExistence));
            mockLicenseRepository.Invocations.AssertArgument(0, nameof(ILicenseRepository.GetIdentifierFromLicenseTenantIdExistence), input.LicenseTenantId);
            mockFileQuotaCallerService.Invocations.AssertSingle(nameof(IFileQuotaCallerService.AddOrUpdateFileTenantQuota));
            mockFileQuotaViewService.Invocations.AssertSingle(nameof(IFileQuotaViewService.AddOrUpdateTenantQuotaView));
        }
        
        [Fact(DisplayName = "Testa a falha ao realizar a call para o fileProvider durante o processo de adição de cota de Tenant")]
        public async Task Test_FileProvider_Call_Failed_During_Add_Tenant_Quota()
        {
            // Initialize Dependencies
            var mockFileQuotaCallerService = new Mock<IFileQuotaCallerService>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockFileQuotaViewService = new Mock<IFileQuotaViewService>();
            var identifier = Guid.NewGuid();
            
            // Initialize Data
            var input = new FileTenantQuotaInput
            {
                LicenseTenantId = Guid.NewGuid(),
                QuotaLimit = 0
            };
            
            mockLicenseRepository.Setup(method => method
                    .GetIdentifierFromLicenseTenantIdExistence(input.LicenseTenantId))
                .ReturnsAsync(identifier);
            mockFileQuotaCallerService.Setup(method => method
                .AddOrUpdateFileTenantQuota(identifier, input.QuotaLimit)
            ).ReturnsAsync((Domain.DTOs.FileQuota.FileTenantQuota) null);

            // Get Testing Objects 
            var controller = GetController(mockFileQuotaCallerService.Object, mockLicenseRepository.Object, mockFileQuotaViewService.Object);
            
            // Method Calls
            await Assert.ThrowsAsync<HttpRequestException>(() => controller.AddOrUpdateTenantQuota(input));
            
            // Tests
            mockLicenseRepository.Invocations.AssertSingle(nameof(ILicenseRepository.GetIdentifierFromLicenseTenantIdExistence));
            mockLicenseRepository.Invocations.AssertArgument(0, nameof(ILicenseRepository.GetIdentifierFromLicenseTenantIdExistence), input.LicenseTenantId);
            mockFileQuotaCallerService.Invocations.AssertSingle(nameof(IFileQuotaCallerService.AddOrUpdateFileTenantQuota));
            mockFileQuotaViewService.Invocations.AssertCount(0, nameof(IFileQuotaViewService.AddOrUpdateTenantQuotaView));
        }
        
        [Fact(DisplayName = "Testa o retorno de cota de Tenant")]
        public async Task Test_Get_Tenant_Quota()
        {
            // Initialize Dependencies
            var mockFileQuotaCallerService = new Mock<IFileQuotaCallerService>();
            var mockLicenseRepository = new Mock<ILicenseRepository>();
            var mockFileQuotaViewService = new Mock<IFileQuotaViewService>();
            mockFileQuotaViewService.Setup(method => method
                .GetTenantQuota(Guid.Empty))
                .ReturnsAsync(new FileTenantQuota());
            
            // Initialize Data
            var licenseTenantIdentifier = Guid.NewGuid();

            // Get Testing Objects 
            var controller = GetController(mockFileQuotaCallerService.Object, mockLicenseRepository.Object, mockFileQuotaViewService.Object);
            
            // Method Calls
            var result = await controller.GetTenantQuota(licenseTenantIdentifier);
            
            // Tests
            mockFileQuotaViewService.Invocations.AssertSingle(nameof(IFileQuotaViewService.GetTenantQuota));
            mockFileQuotaViewService.Invocations.AssertArgument(0, nameof(IFileQuotaViewService.GetTenantQuota), licenseTenantIdentifier);
        }

        private TenantQuotaController GetController(IFileQuotaCallerService mockFileQuotaCallerService, ILicenseRepository mockLicenseRepository, IFileQuotaViewService mockFileQuotaViewService)
        {
            return ActivatorUtilities.CreateInstance<TenantQuotaController>(ServiceProvider, mockFileQuotaCallerService, mockLicenseRepository, mockFileQuotaViewService);
        }
    }
}