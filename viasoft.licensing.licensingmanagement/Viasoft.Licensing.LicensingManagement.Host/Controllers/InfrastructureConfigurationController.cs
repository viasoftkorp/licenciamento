using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.Service;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class InfrastructureConfigurationController: BaseReadonlyController<InfrastructureConfiguration, InfrastructureConfigurationCreateOutput, GetAllInfrastructureConfigurationInput, Guid>
    {
        private readonly IInfrastructureConfigurationService _infrastructureConfigurationService;
        
        public InfrastructureConfigurationController(IReadOnlyRepository<InfrastructureConfiguration> repository, IMapper mapper, IInfrastructureConfigurationService infrastructureConfigurationService) : base(repository, mapper)
        {
            _infrastructureConfigurationService = infrastructureConfigurationService;
        }

        protected override (Expression<Func<InfrastructureConfiguration, Guid>>, bool) DefaultGetAllSorting()
        {
            return (i => i.LicensedTenantId, true);
        }

        [HttpPut]
        [TenantIdParameterHint(nameof(InfrastructureConfigurationUpdateInput.LicensedTenantId) , ParameterLocation.Body, TenantIdParameterKind.LicensedTenantId)]
        public async Task<InfrastructureConfigurationUpdateOutput> Update([FromBody] InfrastructureConfigurationUpdateInput input)
        {
            return await _infrastructureConfigurationService.UpdateAsync(input);
        }

        [HttpGet]
        [TenantIdParameterHint("tenantId", ParameterLocation.Query)]
        public async Task<InfrastructureConfigurationCreateOutput> GetByTenantId([FromQuery] Guid tenantId)
        {
            return await _infrastructureConfigurationService.GetByTenantIdAsync(tenantId);
        }
        
    }
}