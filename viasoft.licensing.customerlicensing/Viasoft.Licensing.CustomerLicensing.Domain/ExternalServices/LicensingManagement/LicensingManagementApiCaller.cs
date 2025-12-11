using System.Net.Http;
using System.Threading.Tasks;
using Viasoft.Core.ApiClient;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant.Store;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings;

namespace Viasoft.Licensing.CustomerLicensing.Domain.ExternalServices.LicensingManagement
{
    public class LicensingManagementApiCaller : ILicensingManagementApiCaller, ITransientDependency
    {
        private const string ServiceName = "Viasoft.Licensing.LicensingManagement";
        private const string InfrastructureConfigurationEndpoint = "licensing/LicensingManagement/InfrastructureConfiguration/";
        
        private readonly IApiClientCallBuilder _apiClientCallBuilder;

        public LicensingManagementApiCaller(IApiClientCallBuilder apiClientCallBuilder, ITenancyStore tenancyStore)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
        }

        public async Task<IApiClientCallResponse<InfrastructureConfigurationUpdateOutput>> UpdateInfrastructureConfiguration(InfrastructureConfigurationUpdateInput input)
        {
            var call = _apiClientCallBuilder.WithEndpoint($"{InfrastructureConfigurationEndpoint}Update")
                .WithServiceName(ServiceName)
                .WithHttpMethod(HttpMethod.Put)
                .WithBody(input)
                .Build();

            var result = await call.CallAsync<InfrastructureConfigurationUpdateOutput>();
            return result;
        }
    }
    
}