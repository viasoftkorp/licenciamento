using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.DTO;

namespace Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.Service
{
    public interface IInfrastructureConfigurationService
    {
        public Task<InfrastructureConfigurationCreateOutput> CreateAsync(InfrastructureConfigurationCreateInput input);
        
        public Task<InfrastructureConfigurationDeleteOutput> DeleteAsync(Guid tenantId);

        public Task<InfrastructureConfigurationUpdateOutput> UpdateAsync(InfrastructureConfigurationUpdateInput input);

        public Task<InfrastructureConfigurationCreateOutput> GetByTenantIdAsync(Guid tenantId);

        public Task<List<Guid>> GetTenantIdListAsync();
    }
}