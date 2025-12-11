using System;
using System.Threading.Tasks;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Core.MultiTenancy.Abstractions.Environment.Model;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment;
using Viasoft.Licensing.CustomerLicensing.Domain.ExternalServices.TenantManagement;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.Environment
{
    
    public class EnvironmentService : IEnvironmentService, ITransientDependency
    {
        private readonly ITenantManagementApiCaller _tenantManagementApiCaller;

        public EnvironmentService(ITenantManagementApiCaller tenantManagementApiCaller)
        {
            _tenantManagementApiCaller = tenantManagementApiCaller;
        }

        public async Task<OrganizationUnitEnvironmentOutput> GetEnvironmentById(Guid id, GetEnvironmentInput input)
        { 
            var result = await _tenantManagementApiCaller.GetEnvironmentById(id, input);
            var environment = await result.GetResponse();
            return environment;
        }

        public async Task<PagedResultDto<OrganizationUnitEnvironmentOutput>> GetEnvironmentByUnit(Guid unitId, GetEnvironmentByUnitInput input)
        {
            input.UnitId ??= unitId;
            var result = await _tenantManagementApiCaller.GetEnvironmentByUnit(input);
            var environment = await result.GetResponse();
            return environment;
        }

        public async Task<CreateEnvironmentOutput> CreateEnvironment(Guid unitId, CreateOrUpdateEnvironmentInput input)
        {
            input.OrganizationUnitId ??= unitId;
            var result = await _tenantManagementApiCaller.CreateEnvironment(input);
            var environment = await result.GetResponse();
            return environment;
        }

        public async Task<UpdateEnvironmentOutput> UpdateEnvironment(Guid unitId, Guid id, CreateOrUpdateEnvironmentInput input)
        {
            input.OrganizationUnitId ??= unitId;
            var result = await _tenantManagementApiCaller.UpdateEnvironment(id, input);
            var environment = await result.GetResponse();
            return environment;
        }

        public async Task ActivateEnvironment(Guid id)
        {
            await _tenantManagementApiCaller.ActivateEnvironment(id);
        }

        public async Task DeactivateEnvironment(Guid id)
        {
            await _tenantManagementApiCaller.DeactivateEnvironment(id);
        }
    }
}