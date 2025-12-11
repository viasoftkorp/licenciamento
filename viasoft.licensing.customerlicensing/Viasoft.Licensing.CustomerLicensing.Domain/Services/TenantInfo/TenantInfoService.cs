using System;
using System.Net.Http;
using System.Threading.Tasks;
using Viasoft.Core.ApiClient;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.CustomerLicensing.Domain.Consts;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.TenantInfo;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.TenantInfo
{
    public class TenantInfoService: ITenantInfoService, ITransientDependency
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;

        public TenantInfoService(IApiClientCallBuilder apiClientCallBuilder)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
        }

        public async Task<TenantInfoOutput> GetTenantInfoFromId(Guid id)
        {
            var gatewayCall = _apiClientCallBuilder
                .WithEndpoint(
                    ExternalServicesConsts.LicensingManagement.TenantInfo.GetTenantInfoFromLicensingIdentifier(id))
                .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .Build();

            var gatewayCallResponse = await gatewayCall.ResponseCallAsync<TenantInfoOutput>();

            return gatewayCallResponse;
        }
    }
}