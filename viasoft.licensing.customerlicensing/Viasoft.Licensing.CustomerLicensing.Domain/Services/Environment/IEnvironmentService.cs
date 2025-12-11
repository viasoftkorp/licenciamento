using System;
using System.Threading.Tasks;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.MultiTenancy.Abstractions.Environment.Model;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.Environment
{
    public interface IEnvironmentService
    {
        Task<OrganizationUnitEnvironmentOutput> GetEnvironmentById(Guid id, GetEnvironmentInput input);
        Task<PagedResultDto<OrganizationUnitEnvironmentOutput>> GetEnvironmentByUnit(Guid unitId, GetEnvironmentByUnitInput input);
        Task<CreateEnvironmentOutput> CreateEnvironment(Guid unitId, CreateOrUpdateEnvironmentInput input);
        Task<UpdateEnvironmentOutput> UpdateEnvironment(Guid unitId, Guid id, CreateOrUpdateEnvironmentInput input);
        Task ActivateEnvironment(Guid id);
        Task DeactivateEnvironment(Guid id);
    }
}