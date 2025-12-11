using System;
using System.Net.Http;
using System.Threading.Tasks;
using Viasoft.Core.ApiClient;
using Viasoft.Core.ApiClient.Extensions;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Core.MultiTenancy.Abstractions.Environment.Model;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.OrganizationUnit;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings;

namespace Viasoft.Licensing.CustomerLicensing.Domain.ExternalServices.TenantManagement
{
    public class TenantManagementApiCaller : ITenantManagementApiCaller, ITransientDependency
    {
        private const string ServiceName = "Viasoft.TenantManagement";
        private const string EnvironmentControllerEndpoint = "TenantManagement/environments/";
        private const string ServerDeployControllerEndpoint = "TenantManagement/server-deploy/";

        private readonly IApiClientCallBuilder _apiClientCallBuilder;

        public TenantManagementApiCaller(IApiClientCallBuilder apiClientCallBuilder)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
        }
        
        public async Task<IApiClientCallResponse<ServerDeployOutput>> GetServerByVersion(string version)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"{ServerDeployControllerEndpoint}{version}")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .Build();

            var result = await call.CallAsync<ServerDeployOutput>();
            return result;
        }

        public async Task<IApiClientCallResponse<PagedResultDto<OrganizationUnitOutput>>> GetUnitsByOrganization(Guid orgId, PagedFilteredAndSortedRequestInput input )
        {
            var call = _apiClientCallBuilder.WithEndpoint($"TenantManagement/organizations/{orgId}/units?{input.ToHttpGetQueryParameter()}")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .Build();

            var result = await call.CallAsync<PagedResultDto<OrganizationUnitOutput>>();
            return result;
        }

        public async Task<IApiClientCallResponse<OrganizationUnitOutput>> GetUnitById(Guid id)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"TenantManagement/units/{id}")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .Build();

            var result = await call.CallAsync<OrganizationUnitOutput>();
            return result;
        }

        public async Task<IApiClientCallResponse<CreateOrganizationUnitOutput>> CreateUnit(Guid orgId, CreateOrUpdateOrganizationUnitInput input)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"/TenantManagement/organizations/{orgId}/units")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .WithBody(input)
                .Build();

            var result = await call.CallAsync<CreateOrganizationUnitOutput>();
            return result;
        }

        public async Task<IApiClientCallResponse<UpdateOrganizationUnitOutput>> UpdateUnit(Guid id, CreateOrUpdateOrganizationUnitInput input)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"/TenantManagement/units/{id}")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Put)
                .WithBody(input)
                .Build();

            var result = await call.CallAsync<UpdateOrganizationUnitOutput>();
            return result;
        }

        public async Task<IApiClientCallResponse<object>> ActivateUnit(Guid id)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"/TenantManagement/units/{id}/activate")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .Build();

            var result = await call.CallAsync<object>();
            return result;
        }

        public async Task<IApiClientCallResponse<object>> DeactivateUnit(Guid id)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"/TenantManagement/units/{id}/deactivate")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .Build();

            var result = await call.CallAsync<object>();
            return result;
        }

        public async Task<IApiClientCallResponse<OrganizationUnitEnvironmentOutput>> GetEnvironmentById(Guid id, GetEnvironmentInput input)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"{EnvironmentControllerEndpoint}{id}?{input.ToHttpGetQueryParameter()}")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .Build();

            var result = await call.CallAsync<OrganizationUnitEnvironmentOutput>();
            return result;
        }

        public async Task<IApiClientCallResponse<PagedResultDto<OrganizationUnitEnvironmentOutput>>> GetEnvironmentByUnit(GetEnvironmentByUnitInput input)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"{EnvironmentControllerEndpoint}?{input.ToHttpGetQueryParameter()}")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .Build();

            var result = await call.CallAsync<PagedResultDto<OrganizationUnitEnvironmentOutput>>();
            return result;
        }

        public async Task<IApiClientCallResponse<CreateEnvironmentOutput>> CreateEnvironment(CreateOrUpdateEnvironmentInput input)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"{EnvironmentControllerEndpoint}")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .WithBody(input)
                .Build();

            var result = await call.CallAsync<CreateEnvironmentOutput>();
            return result;
        }

        public async Task<IApiClientCallResponse<UpdateEnvironmentOutput>> UpdateEnvironment(Guid id, CreateOrUpdateEnvironmentInput input)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"{EnvironmentControllerEndpoint}{id}")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Put)
                .WithBody(input)
                .Build();

            var result = await call.CallAsync<UpdateEnvironmentOutput>();
            return result;
        }

        public async Task<IApiClientCallResponse<object>> ActivateEnvironment(Guid id)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"{EnvironmentControllerEndpoint}{id}/activate")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .Build();

            var result = await call.CallAsync<object>();
            return result;
        }

        public async Task<IApiClientCallResponse<object>> DeactivateEnvironment(Guid id)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"{EnvironmentControllerEndpoint}{id}/deactivate")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .Build();

            var result = await call.CallAsync<object>();
            return result;
        }
    }
}