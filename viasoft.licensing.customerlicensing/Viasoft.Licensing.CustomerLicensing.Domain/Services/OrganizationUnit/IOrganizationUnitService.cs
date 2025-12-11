using System;
using System.Threading.Tasks;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.OrganizationUnit;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.OrganizationUnit
{
    public interface IOrganizationUnitService
    {
        Task<PagedResultDto<OrganizationUnitOutput>> GetUnitsByOrganization(Guid orgId, PagedFilteredAndSortedRequestInput input);
        Task<OrganizationUnitOutput> GetUnitById(Guid id);
        Task<CreateOrganizationUnitOutput> CreateUnit(Guid orgId, CreateOrUpdateOrganizationUnitInput input);
        Task<UpdateOrganizationUnitOutput> UpdateUnit(Guid id, CreateOrUpdateOrganizationUnitInput input);
        Task ActivateUnit(Guid id);
        Task DeactivateUnit(Guid id);
    }
}