using System;
using System.Threading.Tasks;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Organization;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.OrganizationUnit;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement
{
    public interface ITenantManagementCaller
    {
        Task<CreateOrganizationOutput> CreateOrganization(Guid identifier, CreateOrUpdateOrganizationInput input);
        Task<Organization> GetOrganization(Guid identifier, Guid id);
        Task<UpdateOrganizationOutput> UpdateOrganization(Guid identifier, CreateOrUpdateOrganizationInput input);
        Task DeleteOrganization(Guid identifier, Guid id);
        Task<CreateOrganizationUnitOutput> CreateOrganizationUnit(Guid identifier,
            CreateOrUpdateOrganizationUnitInput input);
        Task<OrganizationUnit> GetOrganizationUnit(Guid identifier, Guid id);
        Task<PagedResultDto<OrganizationUnit>> GetUnitsByOrganization(Guid identifier,
            GetByOrganizationInput input);
        Task<UpdateOrganizationUnitOutput> UpdateOrganizationUnit(Guid identifier,
            CreateOrUpdateOrganizationUnitInput input);
        Task ActivateOrganizationUnit(Guid identifier, Guid id);
        Task DeactivateOrganizationUnit(Guid identifier, Guid id);
        Task<CreateEnvironmentOutput> CreateOrganizationEnvironment(Guid identifier,
            CreateOrUpdateEnvironmentInput input);
        Task<OrganizationUnitEnvironmentOutput> GetOrganizationEnvironment(Guid identifier, Guid id);
        Task<PagedResultDto<OrganizationUnitEnvironment>> GetEnvironmentByUnitId(Guid identifier,
            GetEnvironmentByUnitInput input);
        Task<UpdateEnvironmentOutput> UpdateOrganizationEnvironment(Guid identifier,
            CreateOrUpdateEnvironmentInput input);
        Task ActivateOrganizationEnvironment(Guid identifier, Guid id);
        Task DeactivateOrganizationEnvironment(Guid identifier, Guid id);
    }
}