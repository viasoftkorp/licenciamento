using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Viasoft.Core.ApiClient;
using Viasoft.Core.ApiClient.HttpHeaderStrategy;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Services.HttpClient;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.DataUploader
{
    public class DataUploaderService: IDataUploaderService, ITransientDependency
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        private readonly IHttpHeaderStrategy _doNothingHttpHeaderStrategy;
        private readonly IHttpClientService _httpClientService;

        public DataUploaderService(IApiClientCallBuilder apiClientCallBuilder, IHttpClientService httpClientService)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
            _httpClientService = httpClientService;
            _doNothingHttpHeaderStrategy = new DoNothingHttpHeaderStrategy();
        }

        private async Task<bool> UploadLicenseUsageInRealTimeWeb(LicenseUsageInRealTime input)
        {
            var endpoint = ExternalServiceConsts.CustomerLicensing.LicenseUsageInRealTime.Import;
            
            var call = _apiClientCallBuilder.WithEndpoint(endpoint)
                .WithServiceName(ExternalServiceConsts.CustomerLicensing.ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .WithHttpHeaderStrategy(_doNothingHttpHeaderStrategy)
                .WithTimeout(ApiServiceCallTimeout.UltraLong)
                .WithBody(input)
                .Build();
            
            return await call.ResponseCallAsync<bool>();
        }
        
        public async Task<bool> UploadLicenseUsageInRealTime(LicenseUsageInRealTime input)
        {
            if (DefaultConfigurationConsts.IsRunningAsLegacy)
            {
                return await UploadLicenseUsageInRealTimeLegacy(input);
            }

            return await UploadLicenseUsageInRealTimeWeb(input);
        }

        private async Task<bool> UploadLicenseUsageInRealTimeLegacy(LicenseUsageInRealTime input)
        {
            var endpoint = ExternalServiceConsts.CustomerLicensing.LicenseUsageInRealTime.Import;
            var call = _httpClientService.GetRequest(endpoint, HttpMethod.Post,  new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, MediaTypeNames.Application.Json), null);
            var responseMessage = await _httpClientService.SendAsync(call);
            if (responseMessage.IsSuccessStatusCode)
            {
                var content = await responseMessage.Content.ReadAsStringAsync();
                var isBool =  bool.TryParse(content, out var output);
                if (isBool)
                {
                    return output;
                }

                return false;
            }

            return false;
        }
        
    }
}