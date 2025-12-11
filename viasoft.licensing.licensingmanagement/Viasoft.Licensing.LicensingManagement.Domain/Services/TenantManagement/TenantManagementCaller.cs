using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using Viasoft.Core.ApiClient;
using Viasoft.Core.ApiClient.HttpHeaderStrategy;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Organization;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.OrganizationUnit;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.HeaderStrategies;
using callConsts = Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.TenantManagementCallerConsts;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement
{
    public class TenantManagementCaller : ITenantManagementCaller, ITransientDependency
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        private readonly IServiceProvider _serviceProvider;

        public TenantManagementCaller(IApiClientCallBuilder apiClientCallBuilder, IServiceProvider serviceProvider)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
            _serviceProvider = serviceProvider;
        }

        public async Task<CreateOrganizationOutput> CreateOrganization(Guid identifier, CreateOrUpdateOrganizationInput input)
        {
            input.TenantId = identifier;
            var call = _apiClientCallBuilder.WithEndpoint($"{callConsts.BasePath}/{callConsts.Organization}")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .WithBody(input)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            return await call.ResponseCallAsync<CreateOrganizationOutput>();
        }

        public async Task<Organization> GetOrganization(Guid identifier, Guid id)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"{callConsts.BasePath}/{callConsts.Organization}/{id}")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            return await call.ResponseCallAsync<Organization>();
        }

        public async Task<UpdateOrganizationOutput> UpdateOrganization(Guid identifier, CreateOrUpdateOrganizationInput input)
        {
            var call = _apiClientCallBuilder
                .WithEndpoint($"{callConsts.BasePath}/{callConsts.Organization}/{input.Id}")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Put)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .WithBody(input)
                .Build();

            return await call.ResponseCallAsync<UpdateOrganizationOutput>();
        }

        public async Task DeleteOrganization(Guid identifier, Guid id)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"{callConsts.BasePath}/{callConsts.Organization}/{id}")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Delete)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            await call.ResponseCallAsync<string>();
        }

        public async Task<CreateOrganizationUnitOutput> CreateOrganizationUnit(Guid identifier,
            CreateOrUpdateOrganizationUnitInput input)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"{callConsts.BasePath}/{callConsts.Organization}/{input.OrganizationId}/{callConsts.OrganizationUnit}")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .WithBody(input)
                .Build();

            return await call.ResponseCallAsync<CreateOrganizationUnitOutput>();
        }

        public async Task<OrganizationUnit> GetOrganizationUnit(Guid identifier, Guid id)
        {
            var call = _apiClientCallBuilder
                .WithEndpoint($"{callConsts.BasePath}/{callConsts.OrganizationUnit}/{id}")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            return await call.ResponseCallAsync<OrganizationUnit>();
        }

        public async Task<PagedResultDto<OrganizationUnit>> GetUnitsByOrganization(Guid identifier,
            GetByOrganizationInput input)
        {
            var url =
                $"{callConsts.BasePath}/{callConsts.Organization}/{input.OrganizationId}/{callConsts.OrganizationUnit}";
            var paramValues = new NameValueCollection
            {
                {nameof(input.AdvancedFilter), input.AdvancedFilter},
                {nameof(input.Filter), input.Filter},
                {nameof(input.SkipCount), input.SkipCount.ToString()},
                {nameof(input.MaxResultCount), input.MaxResultCount.ToString()},
                {nameof(input.OrganizationId), input.OrganizationId.ToString()}
            };
            if (!string.IsNullOrEmpty(input.Sorting))
                paramValues.Add(nameof(input.Sorting), input.Sorting);
            url += GetQueryParamsString(paramValues);
            
            var call = _apiClientCallBuilder.WithEndpoint(url)
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            return await call.ResponseCallAsync<PagedResultDto<OrganizationUnit>>();
        }

        public async Task<UpdateOrganizationUnitOutput> UpdateOrganizationUnit(Guid identifier,
            CreateOrUpdateOrganizationUnitInput input)
        {
            var call = _apiClientCallBuilder
                .WithEndpoint($"{callConsts.BasePath}/{callConsts.OrganizationUnit}/{input.Id}")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Put)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .WithBody(input)
                .Build();

            return await call.ResponseCallAsync<UpdateOrganizationUnitOutput>();
        }

        public async Task ActivateOrganizationUnit(Guid identifier, Guid id)
        {
            var call = _apiClientCallBuilder
                .WithEndpoint($"{callConsts.BasePath}/{callConsts.OrganizationUnit}/{id}/activate")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            await call.ResponseCallAsync<string>();
        }

        public async Task DeactivateOrganizationUnit(Guid identifier, Guid id)
        {
            var call = _apiClientCallBuilder
                .WithEndpoint($"{callConsts.BasePath}/{callConsts.OrganizationUnit}/{id}/deactivate")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            await call.ResponseCallAsync<string>();
        }

        public async Task<CreateEnvironmentOutput> CreateOrganizationEnvironment(Guid identifier,
            CreateOrUpdateEnvironmentInput input)
        {
            var call = _apiClientCallBuilder
                .WithEndpoint($"{callConsts.BasePath}/{callConsts.OrganizationEnvironment}")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .WithBody(input)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            return await call.ResponseCallAsync<CreateEnvironmentOutput>();
        }

        public async Task<OrganizationUnitEnvironmentOutput> GetOrganizationEnvironment(Guid identifier, Guid id)
        {
            var call = _apiClientCallBuilder
                .WithEndpoint($"{callConsts.BasePath}/{callConsts.OrganizationEnvironment}/{id}")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            return await call.ResponseCallAsync<OrganizationUnitEnvironmentOutput>();
        }

        public async Task<PagedResultDto<OrganizationUnitEnvironment>> GetEnvironmentByUnitId(
            Guid identifier, GetEnvironmentByUnitInput input)
        {
            var url = $"{callConsts.BasePath}/{callConsts.OrganizationEnvironment}";
            var paramValues = new NameValueCollection
            {
                {nameof(input.AdvancedFilter), input.AdvancedFilter},
                {nameof(input.Filter), input.Filter},
                {nameof(input.SkipCount), input.SkipCount.ToString()},
                {nameof(input.MaxResultCount), input.MaxResultCount.ToString()},
                {nameof(input.UnitId), input.UnitId.ToString()},
            };
            if (!string.IsNullOrEmpty(input.Sorting))
                paramValues.Add(nameof(input.Sorting), input.Sorting);
            if (input.ActiveOnly.HasValue && input.ActiveOnly.Value)
                paramValues.Add(nameof(input.ActiveOnly), bool.TrueString);
            if (input.DesktopOnly.HasValue && input.DesktopOnly.Value)
                paramValues.Add(nameof(input.DesktopOnly), bool.TrueString);
            if (input.WebOnly.HasValue && input.WebOnly.Value)
                paramValues.Add(nameof(input.WebOnly), bool.TrueString);
            if (input.ProductionOnly.HasValue && input.ProductionOnly.Value)
                paramValues.Add(nameof(input.ProductionOnly), bool.TrueString);
            if (input.MobileOnly.HasValue && input.MobileOnly.Value)
                paramValues.Add(nameof(input.MobileOnly), bool.TrueString);
            url += GetQueryParamsString(paramValues);

            var call = _apiClientCallBuilder.WithEndpoint(url)
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            return await call.ResponseCallAsync<PagedResultDto<OrganizationUnitEnvironment>>();
        }

        public async Task<UpdateEnvironmentOutput> UpdateOrganizationEnvironment(Guid identifier,
            CreateOrUpdateEnvironmentInput input)
        {
            var call = _apiClientCallBuilder
                .WithEndpoint($"{callConsts.BasePath}/{callConsts.OrganizationEnvironment}/{input.Id}")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Put)
                .WithBody(input)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            return await call.ResponseCallAsync<UpdateEnvironmentOutput>();
        }

        public async Task ActivateOrganizationEnvironment(Guid identifier, Guid id)
        {
            var call = _apiClientCallBuilder
                .WithEndpoint($"{callConsts.BasePath}/{callConsts.OrganizationEnvironment}/{id}/activate")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            await call.ResponseCallAsync<string>();
        }

        public async Task DeactivateOrganizationEnvironment(Guid identifier, Guid id)
        {
            var call = _apiClientCallBuilder
                .WithEndpoint($"{callConsts.BasePath}/{callConsts.OrganizationEnvironment}/{id}/deactivate")
                .WithServiceName(callConsts.ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .WithHttpHeaderStrategy(GetCustomHeaderStrategy(identifier))
                .Build();

            await call.ResponseCallAsync<string>();
        }

        private static string GetQueryParamsString(NameValueCollection paramValues)
        {
            var paramAsString = "";
            foreach (var paramName in paramValues.AllKeys)
            {
                var separator = paramAsString.Contains("?") ? "&" : "?";
                paramAsString += $"{separator}{paramName}={paramValues.Get(paramName)}";
            }

            return paramAsString;
        }

        private IHttpHeaderStrategy GetCustomHeaderStrategy(Guid tenantId)
        {
            return new TenantManagementCallerHeaderStrategy(tenantId, _serviceProvider);
        }
    }
}