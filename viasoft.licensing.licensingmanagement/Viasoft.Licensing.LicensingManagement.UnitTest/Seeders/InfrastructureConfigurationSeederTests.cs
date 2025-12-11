using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.Mapper.Extensions;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Core.Testing;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.HostTenantId;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.Service;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.Seeder;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensingIdentifierToHost;
using Viasoft.Licensing.LicensingManagement.Host.Mappers;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Seeders
{
    public class InfrastructureConfigurationSeederTests: LicensingManagementTestBase
    {
        private InfrastructureConfigurationSeeder CreateSeeder(IRepository<LicensedTenant> licensedTenantsRepo, Guid identifier, IInfrastructureConfigurationService infrastructureConfiguration, ILicensingIdentifierToHostCacheService licensingIdentifierToHostCacheService)
        {
            var currentTenant = new Mock<ICurrentTenant>();
            currentTenant.Setup(m => m.Id).Returns(identifier);
            var seeder = new InfrastructureConfigurationSeeder(licensedTenantsRepo, infrastructureConfiguration, currentTenant.Object, licensingIdentifierToHostCacheService);
            return seeder;
        }
        
        [Fact]
        public async Task ShouldCreateInfrastructureConfigurations()
        {
            var identifier = Guid.NewGuid();
            var licensedTenantsRepo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var tenant = new LicensedTenant
            {
                Id = Guid.NewGuid(),
                Identifier = identifier,
                Notes = Encoding.UTF8.GetBytes("teste"),
                Status = LicensingStatus.Active,
                AccountId = Guid.NewGuid(),
                AdministratorEmail = "teste@teste.com.br",
                LicensedCnpjs = "",
                TenantId = identifier,
                LicensedCnpjList = {""},
                LicenseConsumeType = LicenseConsumeType.Connection,
                ExpirationDateTime = new DateTime(2020, 5, 1)
            };
            await licensedTenantsRepo.InsertAsync(tenant, true);
            var infrastructureServiceMock = new Mock<IInfrastructureConfigurationService>();
            var licensingIdentifierToHostMock = new Mock<ILicensingIdentifierToHostCacheService>();
            infrastructureServiceMock.Setup(m => m.GetTenantIdListAsync()).ReturnsAsync(new List<Guid>());
            Expression<Func<InfrastructureConfigurationCreateInput, bool>> mustBeIdentifier = createInput => createInput.LicensedTenantId == identifier;
            infrastructureServiceMock.Setup(m => m.CreateAsync(It.Is(mustBeIdentifier))).ReturnsAsync(new InfrastructureConfigurationCreateOutput());
            licensingIdentifierToHostMock.Setup(m => m.GetHostTenantIdFromLicensingIdentifier(identifier, TenantIdParameterKind.LicensingIdentifier)).ReturnsAsync(
                new HostTenantIdOutput
                {
                    TenantId = identifier
                });
            var seeder = CreateSeeder(licensedTenantsRepo, identifier, infrastructureServiceMock.Object, licensingIdentifierToHostMock.Object);
            
            await seeder.SeedDataAsync();

            infrastructureServiceMock.Verify(m => m.CreateAsync(It.IsAny<InfrastructureConfigurationCreateInput>()), Times.Once);
        }

        [Fact]
        public async Task ShouldNotCreateInfrastructureConfigurations()
        {
            var identifier = Guid.NewGuid();
            var infrastructureRepo = ServiceProvider.GetService<IRepository<InfrastructureConfiguration>>();
            var licensedTenantsRepo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var tenant = new LicensedTenant
            {
                Id = Guid.NewGuid(),
                Identifier = identifier,
                Notes = Encoding.UTF8.GetBytes("teste"),
                Status = LicensingStatus.Active,
                AccountId = Guid.NewGuid(),
                AdministratorEmail = "teste@teste.com.br",
                LicensedCnpjs = "",
                TenantId = identifier,
                LicensedCnpjList = {""},
                LicenseConsumeType = LicenseConsumeType.Connection,
                ExpirationDateTime = new DateTime(2020, 5, 1)
            };
            var infrastructureConfiguration = new InfrastructureConfiguration
            {
                GatewayAddress = "127.0.0.1",
                DesktopDatabaseName = "teste",
                LicensedTenantId = tenant.Id
            };
            await licensedTenantsRepo.InsertAsync(tenant, true);
            await infrastructureRepo.InsertAsync(infrastructureConfiguration, true);
            var infrastructureConfigurationService = new Mock<IInfrastructureConfigurationService>();
            infrastructureConfigurationService.Setup(m => m.GetTenantIdListAsync()).ReturnsAsync(new List<Guid>
            {
                tenant.Id
            });
            var licensingIdentifierToHostCacheService = new Mock<ILicensingIdentifierToHostCacheService>();
            licensingIdentifierToHostCacheService.Setup(m => m.GetHostTenantIdFromLicensingIdentifier(identifier, TenantIdParameterKind.LicensingIdentifier))
                .ReturnsAsync(new HostTenantIdOutput
                {
                    TenantId = identifier
                });
            var seeder = CreateSeeder(licensedTenantsRepo, identifier, infrastructureConfigurationService.Object, licensingIdentifierToHostCacheService.Object);
            
            await seeder.SeedDataAsync();
            
            infrastructureConfigurationService.Verify(m => m.CreateAsync(It.IsAny<InfrastructureConfigurationCreateInput>()), Times.Never);
        }

        [Fact]
        public async Task ShouldReachFinishCondition()
        {
            var identifier = Guid.NewGuid();
            var licensedTenantsRepo = ServiceProvider.GetService<IRepository<LicensedTenant>>();
            var infrastructureServiceMock = new Mock<IInfrastructureConfigurationService>();
            var licensingIdentifierToHostMock = new Mock<ILicensingIdentifierToHostCacheService>();
            infrastructureServiceMock.Setup(m => m.GetTenantIdListAsync()).ReturnsAsync(new List<Guid>());
            licensingIdentifierToHostMock.Setup(m => m.GetHostTenantIdFromLicensingIdentifier(identifier, TenantIdParameterKind.LicensingIdentifier)).ReturnsAsync(
                new HostTenantIdOutput
                {
                    TenantId = Guid.NewGuid()
                });
            var seeder = CreateSeeder(licensedTenantsRepo, identifier, infrastructureServiceMock.Object, licensingIdentifierToHostMock.Object);
            
            await seeder.SeedDataAsync();

            infrastructureServiceMock.Verify(m => m.CreateAsync(It.IsAny<InfrastructureConfigurationCreateInput>()), Times.Never);
        }
    }
}