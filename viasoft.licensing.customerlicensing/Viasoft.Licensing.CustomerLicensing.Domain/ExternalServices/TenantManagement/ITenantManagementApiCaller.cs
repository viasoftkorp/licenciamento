using System;
using System.Threading.Tasks;
using Viasoft.Core.ApiClient;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.OrganizationUnit;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings;

namespace Viasoft.Licensing.CustomerLicensing.Domain.ExternalServices.TenantManagement
{
    public interface ITenantManagementApiCaller
    {
        Task<IApiClientCallResponse<ServerDeployOutput>> GetServerByVersion(string version);
        
        Task<IApiClientCallResponse<PagedResultDto<OrganizationUnitOutput>>> GetUnitsByOrganization(Guid orgId, PagedFilteredAndSortedRequestInput input);
        Task<IApiClientCallResponse<OrganizationUnitOutput>> GetUnitById(Guid id);
        Task<IApiClientCallResponse<CreateOrganizationUnitOutput>> CreateUnit(Guid orgId, CreateOrUpdateOrganizationUnitInput input);
        Task<IApiClientCallResponse<UpdateOrganizationUnitOutput>> UpdateUnit(Guid id, CreateOrUpdateOrganizationUnitInput input);
        Task<IApiClientCallResponse<object>> ActivateUnit(Guid id);
        Task<IApiClientCallResponse<object>> DeactivateUnit(Guid id);

        Task<IApiClientCallResponse<OrganizationUnitEnvironmentOutput>> GetEnvironmentById(Guid id, GetEnvironmentInput input);
        Task <IApiClientCallResponse<PagedResultDto<OrganizationUnitEnvironmentOutput>>> GetEnvironmentByUnit(GetEnvironmentByUnitInput input);
        Task <IApiClientCallResponse<CreateEnvironmentOutput>> CreateEnvironment(CreateOrUpdateEnvironmentInput input);
        Task <IApiClientCallResponse<UpdateEnvironmentOutput>> UpdateEnvironment(Guid id, CreateOrUpdateEnvironmentInput input);
        Task<IApiClientCallResponse<object>> ActivateEnvironment(Guid id);
        Task<IApiClientCallResponse<object>> DeactivateEnvironment(Guid id);
    }
}