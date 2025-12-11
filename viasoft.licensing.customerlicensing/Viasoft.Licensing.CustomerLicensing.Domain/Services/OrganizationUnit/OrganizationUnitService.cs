using System;
using System.Threading.Tasks;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.OrganizationUnit;
using Viasoft.Licensing.CustomerLicensing.Domain.ExternalServices.TenantManagement;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.OrganizationUnit
{
    public class OrganizationUnitService : IOrganizationUnitService, ITransientDependency
    {
        private readonly ITenantManagementApiCaller _tenantManagementApiCaller;
        
        public OrganizationUnitService(ITenantManagementApiCaller tenantManagementApiCaller)
        {
            _tenantManagementApiCaller = tenantManagementApiCaller;
        }
        
        public async Task<PagedResultDto<OrganizationUnitOutput>> GetUnitsByOrganization(Guid orgId, PagedFilteredAndSortedRequestInput input)
        {
            var result = await _tenantManagementApiCaller.GetUnitsByOrganization(orgId, input);
            var organizationUnits = await result.GetResponse();
            return organizationUnits;
        }

        public async Task<OrganizationUnitOutput> GetUnitById(Guid id)
        {
            var result = await _tenantManagementApiCaller.GetUnitById(id);
            var organizationUnit = await result.GetResponse();
            return organizationUnit;
        }

        public async Task<CreateOrganizationUnitOutput> CreateUnit(Guid orgId, CreateOrUpdateOrganizationUnitInput input)
        {
            var result = await _tenantManagementApiCaller.CreateUnit(orgId, input);
            var createOrganizationUnitOutput = await result.GetResponse();
            return createOrganizationUnitOutput;
        }

        public async Task<UpdateOrganizationUnitOutput> UpdateUnit(Guid id, CreateOrUpdateOrganizationUnitInput input)
        {
            var result =  await _tenantManagementApiCaller.UpdateUnit(id, input);
            var updateOrganizationUnitOutput = await result.GetResponse();
            return updateOrganizationUnitOutput;
        }

        public async Task ActivateUnit(Guid id)
        {
            await _tenantManagementApiCaller.ActivateUnit(id);
        }

        public async Task DeactivateUnit(Guid id)
        {
            await _tenantManagementApiCaller.DeactivateUnit(id);
        }
    }
}