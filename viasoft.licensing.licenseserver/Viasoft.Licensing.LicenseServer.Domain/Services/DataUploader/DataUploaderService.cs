using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Viasoft.Core.ApiClient;
using Viasoft.Core.ApiClient.HttpHeaderStrategy;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models;
using Viasoft.Licensing.LicenseServer.Shared.AccessTokenProvider;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Consul;
using Viasoft.Licensing.LicenseServer.Shared.Services.HttpClient;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.DataUploader
{
    public class DataUploaderService: IDataUploaderService, ITransientDependency
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        private readonly IHttpHeaderStrategy _doNothingHttpHeaderStrategy;
        private readonly IHttpClientService _httpClientService;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly IConsulSettingsProvider _consulSettingsProvider;
        private readonly IConfiguration _configuration;

        public DataUploaderService(IApiClientCallBuilder apiClientCallBuilder, IHttpClientService httpClientService, IAccessTokenProvider accessTokenProvider, IConsulSettingsProvider consulSettingsProvider, IConfiguration configuration)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
            _httpClientService = httpClientService;
            _accessTokenProvider = accessTokenProvider;
            _consulSettingsProvider = consulSettingsProvider;
            _configuration = configuration;
            _doNothingHttpHeaderStrategy = new DoNothingHttpHeaderStrategy();
        }

        public async Task<bool> UploadLicenseUsageInRealTime(LicenseUsageInRealTimeOutput input)
        {
            if (DefaultConfigurationConsts.IsRunningAsLegacy)
            {
                return await UploadLicenseUsageInRealTimeLegacy(input);
            }

            return await UploadLicenseUsageInRealTimeWeb(input);
        }
        
        private async Task<bool> UploadLicenseUsageInRealTimeWeb(LicenseUsageInRealTimeOutput input)
        {
            var customerLicensingSecret = _configuration["Authentication:CustomerLicensingSecret"];
            var authority = _configuration["Authentication:Authority"];
            var accessToken = await _accessTokenProvider.ProvideAccessToken(customerLicensingSecret, "Viasoft.CustomerLicensing.LicenseServer", "Viasoft.CustomerLicensing.LicenseServer", authority);
            
            var endpoint = ExternalServiceConsts.CustomerLicensing.LicenseUsageInRealTimeImport.RealTime;
            
            var call = _apiClientCallBuilder.WithEndpoint(endpoint)
                .WithServiceName(ExternalServiceConsts.CustomerLicensing.ServiceName)
                .WithHttpMethod(HttpMethod.Post)
                .WithHttpHeaderStrategy(_doNothingHttpHeaderStrategy)
                .WithTimeout(ApiServiceCallTimeout.UltraLong)
                .WithBody(input)
                .WithAccessToken(accessToken)
                .Build();

            var rawCallResponse = await call.CallAsync<object>();
            return rawCallResponse.IsSuccessStatusCode;
        }

        private async Task<bool> UploadLicenseUsageInRealTimeLegacy(LicenseUsageInRealTimeOutput input)
        {
            var consulSettings = _consulSettingsProvider.GetSettingsFromConsul();
            var accessToken = await _accessTokenProvider.ProvideAccessToken(consulSettings.Authentication.CustomerLicensingSecret, "Viasoft.CustomerLicensing.LicenseServer", 
                "Viasoft.CustomerLicensing.LicenseServer", consulSettings.Authentication.Authority);
            
            var endpoint = ExternalServiceConsts.CustomerLicensing.LicenseUsageInRealTimeImport.RealTime;
            
            var call = _httpClientService.GetRequest(endpoint, HttpMethod.Post,  new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, 
                MediaTypeNames.Application.Json), accessToken);
            var responseMessage = await _httpClientService.SendAsync(call);
            
            return responseMessage.IsSuccessStatusCode;
        }
        
    }
}