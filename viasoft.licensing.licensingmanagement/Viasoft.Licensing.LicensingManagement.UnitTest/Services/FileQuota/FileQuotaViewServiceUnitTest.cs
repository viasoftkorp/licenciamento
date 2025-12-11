using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota;
using Viasoft.Licensing.LicensingManagement.Domain.Services.FileQuota;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.FileQuota
{
    public class FileQuotaViewManagerUnitTest: LicensingManagementTestBase
    {
        [Fact(DisplayName = "Testa o get de cota de App")]
        public async Task Test_Get_App_Quota()
        {
            // Initialize Dependencies
            var memoryRepositoryAppQuotaView = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryRepositoryTenantQuota = ServiceProvider.GetService<IRepository<FileTenantQuota>>();
            
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
            var controller = GetService(memoryRepositoryAppQuotaView, memoryRepositoryTenantQuota);
            
            // Method Calls
            var result = await controller.GetAppQuota(licensedTenantId, appId);
            
            result.Should().BeEquivalentTo(expectedAppQuota);
        }
        
        [Fact(DisplayName = "Testa o get de cota de App não encontrada")]
        public async Task Test_Get_Not_Found_App_Quota()
        {
            // Initialize Dependencies
            var memoryRepositoryAppQuotaView = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryRepositoryTenantQuota = ServiceProvider.GetService<IRepository<FileTenantQuota>>();
            
            // Initialize Data
            var licensedTenantId = Guid.NewGuid();
            var appId = Guid.NewGuid();

            // Get Testing Objects 
            var controller = GetService(memoryRepositoryAppQuotaView, memoryRepositoryTenantQuota);
            
            // Method Calls
            var result = await controller.GetAppQuota(licensedTenantId, appId);
            Assert.Null(result);
        }

        [Fact(DisplayName = "Testa o get de cota de Tenant")]
        public async Task Test_Get_Tenant_Quota()
        {
            // Initialize Dependencies
            var memoryRepositoryAppQuotaView = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryRepositoryTenantQuota = ServiceProvider.GetService<IRepository<FileTenantQuota>>();
            
            // Initialize Data
            var licenseTenantId = Guid.NewGuid();
            var dataToFill = new FileTenantQuota
            {
                Id = Guid.NewGuid(),
                QuotaLimit = 1024,
                LicenseTenantId = licenseTenantId
            };
            var expectedTenantQuota = await memoryRepositoryTenantQuota.InsertAsync(dataToFill, true);

            // Get Testing Objects 
            var controller = GetService(memoryRepositoryAppQuotaView, memoryRepositoryTenantQuota);
            
            // Method Calls
            var result = await controller.GetTenantQuota(licenseTenantId);
            
            result.Should().BeEquivalentTo(expectedTenantQuota);
        }

        [Fact(DisplayName = "Testa o get de cota de Tenant não encontrada")]
        public async Task Test_Get_Not_Found_Tenant_Quota()
        {
            // Initialize Dependencies
            var memoryRepositoryAppQuotaView = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryRepositoryTenantQuota = ServiceProvider.GetService<IRepository<FileTenantQuota>>();
            
            // Initialize Data
            var licenseTenantId = Guid.NewGuid();

            // Get Testing Objects 
            var controller = GetService(memoryRepositoryAppQuotaView, memoryRepositoryTenantQuota);
            
            // Method Calls
            var result = await controller.GetTenantQuota(licenseTenantId);
            Assert.Null(result);
        }
        
        [Fact(DisplayName = "Testa o update da cota de App")]
        public async Task Test_Update_App_Quota_View()
        {
            // Initialize Dependencies
            var memoryRepositoryAppQuotaView = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryRepositoryTenantQuota = ServiceProvider.GetService<IRepository<FileTenantQuota>>();
            
            // Initialize Data
            var dataToFill = new FileAppQuotaView
            {
                AppId = Guid.NewGuid(),
                AppName = nameof(FileAppQuotaView.AppName), 
                QuotaLimit = 0,
                LicensedTenantId = Guid.NewGuid()
            };
            var expectedAppQuota = await memoryRepositoryAppQuotaView.InsertAsync(dataToFill, true);

            // Get Testing Objects 
            var controller = GetService(memoryRepositoryAppQuotaView, memoryRepositoryTenantQuota);
            
            // Modifying Objects
            var random = new Random();
            var newQuota = Convert.ToInt64(random.Next(-1, 1048576));
            
            // Method Calls
            await controller.AddOrUpdateAppQuotaView(dataToFill.LicensedTenantId, dataToFill.AppId, dataToFill.AppName, newQuota);
            var result = memoryRepositoryAppQuotaView.First();
            expectedAppQuota.QuotaLimit = newQuota;
            result.Should().BeEquivalentTo(expectedAppQuota);
        }
        
        [Fact(DisplayName = "Testa o create da cota de App")]
        public async Task Test_Create_App_Quota_View()
        {
            // Initialize Dependencies
            var memoryRepositoryAppQuotaView = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryRepositoryTenantQuota = ServiceProvider.GetService<IRepository<FileTenantQuota>>();
            
            // Initialize Data
            var random = new Random();
            var dataToFill = new FileAppQuotaView
            {
                AppId = Guid.NewGuid(),
                AppName = nameof(FileAppQuotaView.AppName), 
                QuotaLimit = Convert.ToInt64(random.Next(-1, 1048576)),
                LicensedTenantId = Guid.NewGuid()
            };

            // Get Testing Objects 
            var controller = GetService(memoryRepositoryAppQuotaView, memoryRepositoryTenantQuota);
            
            // Method Calls
            await controller.AddOrUpdateAppQuotaView(dataToFill.LicensedTenantId, dataToFill.AppId, dataToFill.AppName, dataToFill.QuotaLimit);
            var result = await memoryRepositoryAppQuotaView.FirstOrDefaultAsync();
            result.Should().BeEquivalentTo(
                dataToFill,
                opt => opt.Excluding(x => x.Id));
        }
        
        [Fact(DisplayName = "Testa o update da cota de Tenant")]
        public async Task Test_Update_Tenant_Quota_View()
        {
            // Initialize Dependencies
            var memoryRepositoryAppQuotaView = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryRepositoryTenantQuota = ServiceProvider.GetService<IRepository<FileTenantQuota>>();
            
            // Initialize Data
            var dataToFill = new FileTenantQuota
            {
                QuotaLimit = 0,
                LicenseTenantId = Guid.NewGuid()
            };
            var expectedTenantQuota = await memoryRepositoryTenantQuota.InsertAsync(dataToFill, true);

            // Get Testing Objects 
            var controller = GetService(memoryRepositoryAppQuotaView, memoryRepositoryTenantQuota);
            
            // Modifying Objects
            var random = new Random();
            var newQuota = Convert.ToInt64(random.Next(-1, 1048576));
            
            // Method Calls
            await controller.AddOrUpdateTenantQuotaView(dataToFill.LicenseTenantId, newQuota);
            var result = memoryRepositoryTenantQuota.First();
            expectedTenantQuota.QuotaLimit = newQuota;
            result.Should().BeEquivalentTo(expectedTenantQuota);
        }
        
        [Fact(DisplayName = "Testa o create da cota de Tenant")]
        public async Task Test_Create_Tenant_Quota_View()
        {
            // Initialize Dependencies
            var memoryRepositoryAppQuotaView = ServiceProvider.GetService<IRepository<FileAppQuotaView>>();
            var memoryRepositoryTenantQuota = ServiceProvider.GetService<IRepository<FileTenantQuota>>();
            
            // Initialize Data
            var random = new Random();
            var dataToFill = new FileTenantQuota
            {
                LicenseTenantId = Guid.NewGuid(),
                QuotaLimit = Convert.ToInt64(random.Next(-1, 1048576))
            };

            // Get Testing Objects 
            var controller = GetService(memoryRepositoryAppQuotaView, memoryRepositoryTenantQuota);
            
            // Method Calls
            await controller.AddOrUpdateTenantQuotaView(dataToFill.LicenseTenantId, dataToFill.QuotaLimit);
            var result = await memoryRepositoryTenantQuota.FirstOrDefaultAsync();
            result.Should().BeEquivalentTo(
                dataToFill,
                opt => opt.Excluding(x => x.Id));
        }
        
        private static FileQuotaViewService GetService(IRepository<FileAppQuotaView> memoryRepositoryAppQuotaView, IRepository<FileTenantQuota> memoryRepositoryTenantQuota)
        {
            return new FileQuotaViewService(memoryRepositoryAppQuotaView, memoryRepositoryTenantQuota);
        }
    }
}