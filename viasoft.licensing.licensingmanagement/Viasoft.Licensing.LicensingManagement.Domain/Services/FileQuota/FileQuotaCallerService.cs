using System;
using System.Net.Http;
using System.Threading.Tasks;
using Viasoft.Core.ApiClient;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.FileQuota;
using Viasoft.Licensing.LicensingManagement.Domain.HttpHeaderStrategy;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.FileQuota
{
    public class FileQuotaCallerService: IFileQuotaCallerService, ITransientDependency
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        private readonly IServiceProvider _serviceProvider;

        public FileQuotaCallerService(IApiClientCallBuilder apiClientCallBuilder, IServiceProvider serviceProvider)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
            _serviceProvider = serviceProvider;
        }

        public async Task<FileTenantQuota> AddOrUpdateFileTenantQuota(Guid tenantId, long quotaLimit)
        {   
            var call = _apiClientCallBuilder.WithEndpoint($"/FileProvider/tenant/{tenantId}/quota")
                .WithServiceName("Viasoft.FileProvider")
                .WithHttpMethod(HttpMethod.Post)
                .WithHttpHeaderStrategy(GetHeaderTenantStrategy(tenantId))
                .WithBody(quotaLimit)
                .Build();

            return await call.ResponseCallAsync<FileTenantQuota>();
        }

        public async Task<FileAppQuota> AddOrUpdateFileAppQuota(Guid tenantId, string appId, long quotaLimit)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"/FileProvider/tenant/{tenantId}/app/{appId}/quota")
                .WithServiceName("Viasoft.FileProvider")
                .WithHttpMethod(HttpMethod.Post)
                .WithBody(quotaLimit)
                .WithHttpHeaderStrategy(GetHeaderTenantStrategy(tenantId))
                .Build();

            return await call.ResponseCallAsync<FileAppQuota>();
        }

        public async Task DeleteAppQuota(Guid tenantId, string appId)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"/FileProvider/tenant/{tenantId}/app/{appId}/quota")
                .WithServiceName("Viasoft.FileProvider")
                .WithHttpMethod(HttpMethod.Delete)
                .WithHttpHeaderStrategy(GetHeaderTenantStrategy(tenantId))
                .Build();

            await call.ResponseCallAsync<string>();
        }

        private TenantIdentifierHeaderStrategyRemovingEnviroment GetHeaderTenantStrategy(Guid tenantId) {
            return new TenantIdentifierHeaderStrategyRemovingEnviroment(tenantId.ToString(), _serviceProvider);
        }
    }
}