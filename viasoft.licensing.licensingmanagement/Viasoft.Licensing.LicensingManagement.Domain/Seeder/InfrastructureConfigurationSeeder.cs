using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Data.Seeder.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.Service;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensingIdentifierToHost;

namespace Viasoft.Licensing.LicensingManagement.Domain.Seeder
{
    public class InfrastructureConfigurationSeeder: ISeedData
    {
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        private readonly IInfrastructureConfigurationService _infrastructureConfigurationService;
        private readonly ILicensingIdentifierToHostCacheService _licensingIdentifierToHostCacheService;
        private readonly ICurrentTenant _currentTenant;

        public InfrastructureConfigurationSeeder(IRepository<Entities.LicensedTenant> licensedTenants, IInfrastructureConfigurationService infrastructureConfigurationService, ICurrentTenant currentTenant, ILicensingIdentifierToHostCacheService licensingIdentifierToHostCacheService)
        {
            _licensedTenants = licensedTenants;
            _infrastructureConfigurationService = infrastructureConfigurationService;
            _currentTenant = currentTenant;
            _licensingIdentifierToHostCacheService = licensingIdentifierToHostCacheService;
        }

        public async Task SeedDataAsync()
        {
            var hostTenant =
                await _licensingIdentifierToHostCacheService.GetHostTenantIdFromLicensingIdentifier(_currentTenant.Id, TenantIdParameterKind.LicensingIdentifier);
            if (hostTenant?.TenantId != _currentTenant.Id)
            {
                return;
            }
            var infrastructureIds = await _infrastructureConfigurationService.GetTenantIdListAsync();
            var tenants = await _licensedTenants.Select(t => t.Id).Where(id => !infrastructureIds.Contains(id)).ToListAsync();
            foreach (var tenant in tenants)
            {
                await _infrastructureConfigurationService.CreateAsync(new InfrastructureConfigurationCreateInput
                {
                    GatewayAddress = null,
                    LicensedTenantId = tenant
                });
            }
        }
    }
}