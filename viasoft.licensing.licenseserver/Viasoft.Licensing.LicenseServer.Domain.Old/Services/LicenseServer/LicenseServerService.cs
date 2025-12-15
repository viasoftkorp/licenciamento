using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Viasoft.Core.ApiClient;
using Viasoft.Core.ApiClient.HttpHeaderStrategy;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Shared.AccessTokenProvider;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Consul;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.HardwareId;
using Viasoft.Licensing.LicenseServer.Shared.Services.HttpClient;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseServer
{
    public class LicenseServerService: ILicenseServerService, ISingletonDependency
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        private readonly IHttpHeaderStrategy _doNothingHttpHeaderStrategy;
        private readonly IHttpClientService _httpClientService;
        private readonly IConsulSettingsProvider _consulSettingsProvider;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly ILogger<LicenseServerService> _logger;

        public LicenseServerService(IApiClientCallBuilder apiClientCallBuilder, IHttpClientService httpClientService, IConsulSettingsProvider consulSettingsProvider, 
            IAccessTokenProvider accessTokenProvider, ILogger<LicenseServerService> logger)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
            _httpClientService = httpClientService;
            _consulSettingsProvider = consulSettingsProvider;
            _accessTokenProvider = accessTokenProvider;
            _logger = logger;
            _doNothingHttpHeaderStrategy = new DoNothingHttpHeaderStrategy();
        }

        private async Task<LicenseByTenantIdOld> GetLicenseByTenantIdWeb(Guid tenantId)
        {
            var endpoint = $"{ExternalServiceConsts.LicensingManagement.LicenseServer.GetLicenseByTenantId}{tenantId}";

            var call = _apiClientCallBuilder.WithEndpoint(endpoint)
                .WithServiceName(ExternalServiceConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .WithHttpHeaderStrategy(_doNothingHttpHeaderStrategy)
                .Build();
            
            return await call.ResponseCallAsync<LicenseByTenantIdOld>();
        }

        public async Task<LicenseByTenantIdOld> GetLicenseByTenantId(Guid tenantId)
        {
            try
            {
                if (DefaultConfigurationConsts.IsRunningAsLegacy)
                {
                    return await GetLicenseByTenantIdLegacy(tenantId);
                }

                return await GetLicenseByTenantIdWeb(tenantId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[LEGACY] - Could not get license for tenant {0}", tenantId);
                throw new Exception(ExceptionMessageConsts.CouldNotLoadLicensesFromRemoteServer);
            }
        }

        public async Task<UpdateHardwareIdOutput> UpdateHardwareId(UpdateHardwareIdInput input)
        {
            if (!DefaultConfigurationConsts.IsRunningAsLegacy && !DefaultConfigurationConsts.IsRunningAsLegacyWithBroker)
            {
                throw new NotSupportedException($"Cannot call {nameof(UpdateHardwareId)} if not in legacy mode");
            }
            
            var accessToken = await LegacyGetAccessToken();
            var endpoint = ExternalServiceConsts.LicensingManagement.Licenses.UpdateHardwareId(input.TenantId);

            var call = _apiClientCallBuilder.WithEndpoint(endpoint)
                .WithServiceName(ExternalServiceConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Put)
                .WithBody(input)
                .WithHttpHeaderStrategy(_doNothingHttpHeaderStrategy)
                .WithAccessToken(accessToken)
                .Build();

            return await call.ResponseCallAsync<UpdateHardwareIdOutput>();
        }
        
        private async Task<string> LegacyGetAccessToken()
        {
            var consulSettings = _consulSettingsProvider.GetSettingsFromConsul();
            var accessToken = await _accessTokenProvider.ProvideAccessToken(consulSettings.Authentication.LicensingManagementSecret, 
                "Viasoft.LicensingManagement.LicenseServer", "Viasoft.LicensingManagement.LicenseServer", consulSettings.Authentication.Authority);
            return accessToken;
        }

        private async Task<LicenseByTenantIdOld> GetLicenseByTenantIdLegacy(Guid tenantId)
        {
            var endpoint = $"{ExternalServiceConsts.LicensingManagement.LicenseServer.GetLicenseByTenantId}{tenantId}";
            
            var call = _httpClientService.GetRequest(endpoint, HttpMethod.Get, null, null);
            var responseMessage = await _httpClientService.SendAsync(call);
            if (responseMessage.IsSuccessStatusCode)
            {
                var content = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LicenseByTenantIdOld>(content);
            }

            throw new Exception(responseMessage.ReasonPhrase);
        }
    }
}