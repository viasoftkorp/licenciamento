using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.AppMessages;
using Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota;
using Viasoft.Licensing.LicensingManagement.Host.Handlers.FileQuota;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Handlers.FileQuota
{
    public class FileQuotaViewHandlerUnitTest: LicensingManagementTestBase
    {
        [Fact(DisplayName = "Testa o handler de app atualizado")]
        public async Task Test_App_Updated_Handler()
        {
            // Initialize Dependencies
            var memoryRepositoryAppQuotaView = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            
            // Initialize Data
            var licensedTenantId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            const string initialName = nameof(FileAppQuotaView.AppName);
            var dataToFill = new FileAppQuotaView
            {
                Id = Guid.NewGuid(),
                AppId = appId,
                AppName = initialName,
                LicensedTenantId = licensedTenantId
            };
            var initialAppQuotaView = await memoryRepositoryAppQuotaView.InsertAsync(dataToFill, true);

            // Get Testing Objects 
            var controller = GetService(memoryRepositoryAppQuotaView);
            
            // Method Calls
            var appUpdatedMessage = new AppUpdatedMessage
            {
                Default = false,
                Domain = Domain.Enums.Domain.Customized,
                SoftwareId = Guid.NewGuid(),
                Name = $"{nameof(AppUpdatedMessage.Name)} Changed",
                Identifier = nameof(AppUpdatedMessage.Identifier),
                IsActive = true,
                Id = appId
            };
            var result = new FileAppQuotaView
            {
                Id = initialAppQuotaView.Id,
                QuotaLimit = initialAppQuotaView.QuotaLimit,
                AppId = appId,
                AppName = appUpdatedMessage.Name,
                LicensedTenantId = initialAppQuotaView.LicensedTenantId
            };
            initialAppQuotaView.AppName = appUpdatedMessage.Name;
            
            await controller.Handle(appUpdatedMessage);
            var appSaved = await memoryRepositoryAppQuotaView.FirstOrDefaultAsync();
            Assert.NotEqual(initialName, appSaved.AppName);
            appSaved.Should().BeEquivalentTo(result);
        }
        
        [Fact(DisplayName = "Testa o handler de app atualizado não encontrado")]
        public async Task Test_App_Updated_Not_Found_Handler()
        {
            // Initialize Dependencies
            var memoryRepositoryAppQuotaView = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            
            // Initialize Data
            var licensedTenantId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var dataToFill = new FileAppQuotaView
            {
                Id = Guid.NewGuid(),
                AppId = appId,
                AppName = nameof(FileAppQuotaView.AppName),
                LicensedTenantId = licensedTenantId
            };
            var expectedAppQuota = await memoryRepositoryAppQuotaView.InsertAsync(dataToFill, true);

            // Get Testing Objects 
            var controller = GetService(memoryRepositoryAppQuotaView);
            
            // Method Calls
            var appUpdatedMessage = new AppUpdatedMessage
            {
                Default = false,
                Domain = Domain.Enums.Domain.Customized,
                SoftwareId = Guid.NewGuid(),
                Name = $"{nameof(AppUpdatedMessage.Name)} Changed",
                Identifier = nameof(AppUpdatedMessage.Identifier),
                IsActive = true,
                Id = Guid.NewGuid()
            };
            await controller.Handle(appUpdatedMessage);
            var appSaved = await memoryRepositoryAppQuotaView.FirstOrDefaultAsync();
            Assert.Equal(dataToFill.AppName, appSaved.AppName);
            Assert.NotEqual(appUpdatedMessage.Name, appSaved.AppName);
            appSaved.Should().BeEquivalentTo(expectedAppQuota);
        }
        
        private static FileQuotaViewHandler GetService(IRepository<FileAppQuotaView> memoryRepositoryAppQuotaView)
        {
            return new FileQuotaViewHandler(memoryRepositoryAppQuotaView);
        }
    }
}