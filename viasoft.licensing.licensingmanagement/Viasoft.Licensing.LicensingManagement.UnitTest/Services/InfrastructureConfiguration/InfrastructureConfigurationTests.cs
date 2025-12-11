using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.Caching.DistributedCache;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.Const;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.Service;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Services.InfrastructureConfiguration
{
    public class InfrastructureConfigurationTests: LicensingManagementTestBase
    {
        [Fact]
        public async Task TestCreateShouldNotCreate()
        {
            var tenantId = Guid.NewGuid();
            var mock = new Mock<IDistributedCacheService>();
            var repo = ServiceProvider.GetService<IRepository<Domain.Entities.InfrastructureConfiguration>>();
            await repo.InsertAsync(new Domain.Entities.InfrastructureConfiguration
            {
                GatewayAddress = "Teste",
                LicensedTenantId = tenantId,
                DesktopDatabaseName = "teste"
            }, true);
            var service = ActivatorUtilities.CreateInstance<InfrastructureConfigurationService>(ServiceProvider, mock.Object);

            var result = await service.CreateAsync(new InfrastructureConfigurationCreateInput
            {
                LicensedTenantId = tenantId,
                GatewayAddress = "Teste"
            });

            result.Success.Should().BeFalse();
            result.Errors.Should().ContainSingle(error =>
                error.ErrorCode == OperationValidation.InfrastructureConfigurationAlreadyExists);
            var count = await repo.CountAsync();
            count.Should().Be(1);
        }

        [Fact]
        public async Task TestCreateShouldCreate()
        {
            var tenantId = Guid.NewGuid();
            var cacheMock = new Mock<IDistributedCacheService>();
            var repo = ServiceProvider.GetService<IRepository<Domain.Entities.InfrastructureConfiguration>>();
            var service = ActivatorUtilities.CreateInstance<InfrastructureConfigurationService>(ServiceProvider, cacheMock.Object);

            var result = await service.CreateAsync(new InfrastructureConfigurationCreateInput
            {
                LicensedTenantId = tenantId,
                GatewayAddress = "Teste"
            });

            result.Should().NotBeNull();
            Assert.True(await repo.AnyAsync(i => i.LicensedTenantId == tenantId));
        }

        [Fact]
        public async Task TestDelete()
        {
            var tenantId = Guid.NewGuid();
            var licensedTenantCacheMock = new Mock<IDistributedCacheService>();
            var repo = ServiceProvider.GetService<IRepository<Domain.Entities.InfrastructureConfiguration>>();
            await repo.InsertAsync(new Domain.Entities.InfrastructureConfiguration
            {
                GatewayAddress = "Teste",
                LicensedTenantId = tenantId,
                DesktopDatabaseName = "teste"
            }, true);
            var service = ActivatorUtilities.CreateInstance<InfrastructureConfigurationService>(ServiceProvider, licensedTenantCacheMock.Object);

            var result = await service.DeleteAsync(tenantId);

            result.Success.Should().BeTrue();
            Assert.False(await repo.AnyAsync(i => i.LicensedTenantId == tenantId));
        }

        [Fact]
        public async Task TestUpdateShouldUpdate()
        {
            var tenantId = Guid.NewGuid();
            var cacheMock = new Mock<IDistributedCacheService>();
            cacheMock.Setup(l => l.RemoveAsync(InfrastructureConfigurationConsts.CacheKey,
                new TenantDistributedCacheKeyStrategy(tenantId), CancellationToken.None)).Returns(Task.CompletedTask);
            var tenantRepo = ServiceProvider.GetService<IRepository<Domain.Entities.LicensedTenant>>();
            var repo = ServiceProvider.GetService<IRepository<Domain.Entities.InfrastructureConfiguration>>();
            await repo.InsertAsync(new Domain.Entities.InfrastructureConfiguration
            {
                GatewayAddress = "Teste",
                LicensedTenantId = tenantId,
                DesktopDatabaseName = "teste"
            }, true);
            await tenantRepo.InsertAsync(new Domain.Entities.LicensedTenant
            {
                Id = tenantId,
                Identifier = Guid.NewGuid()
            }, true);
            var service = ActivatorUtilities.CreateInstance<InfrastructureConfigurationService>(ServiceProvider, cacheMock.Object);

            var result = await service.UpdateAsync(new InfrastructureConfigurationUpdateInput
            {
                GatewayAddress = "127.0.0.1:9999",
                LicensedTenantId = tenantId
            });

            result.Should().NotBeNull();
            var newEntity = repo.First(i => i.LicensedTenantId == tenantId);
            newEntity.GatewayAddress.Should().Be("127.0.0.1:9999");
            newEntity.DesktopDatabaseName.Should().Be("teste");
        }

        [Fact]
        public async Task TestUpdateShouldNotUpdate()
        {
            var tenantId = Guid.NewGuid();
            var cacheMock = new Mock<IDistributedCacheService>();
            cacheMock.Setup(l => l.RemoveAsync(InfrastructureConfigurationConsts.CacheKey,
                new TenantDistributedCacheKeyStrategy(tenantId), CancellationToken.None)).Returns(Task.CompletedTask);
            var repo = ServiceProvider.GetService<IRepository<Domain.Entities.InfrastructureConfiguration>>();
            await repo.InsertAsync(new Domain.Entities.InfrastructureConfiguration
            {
                GatewayAddress = "Teste",
                LicensedTenantId = tenantId,
                DesktopDatabaseName = "teste"
            }, true);
            var service = ActivatorUtilities.CreateInstance<InfrastructureConfigurationService>(ServiceProvider, cacheMock.Object);

            var result = await service.UpdateAsync(new InfrastructureConfigurationUpdateInput
            {
                GatewayAddress = "NeoTeste",
                LicensedTenantId = tenantId
            });

            result.Success.Should().BeFalse();
            result.Errors.Should().ContainSingle(error => error.ErrorCode == OperationValidation.InvalidGateway);
            var newEntity = repo.First(i => i.LicensedTenantId == tenantId);
            newEntity.GatewayAddress.Should().Be("Teste");
            newEntity.DesktopDatabaseName.Should().Be("teste");
        }
    }
}