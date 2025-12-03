using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Viasoft.Core.ApiClient;
using Viasoft.Core.ApiClient.HttpHeaderStrategy;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.TenantLicensing;
using Viasoft.Licensing.LicenseServer.Shared.AccessTokenProvider;
using Viasoft.Licensing.LicenseServer.Shared.Classes;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Consul;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.HardwareId;
using Viasoft.Licensing.LicenseServer.Shared.Services.HttpClient;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.LicenseServer
{
    public class ExternalLicensingManagementService: IExternalLicensingManagementService, ITransientDependency
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        private readonly IHttpHeaderStrategy _doNothingHttpHeaderStrategy;
        private readonly IHttpClientService _httpClientService;
        private readonly IConsulSettingsProvider _consulSettingsProvider;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExternalLicensingManagementService> _logger;

        public ExternalLicensingManagementService(IApiClientCallBuilder apiClientCallBuilder, IHttpClientService httpClientService, IConsulSettingsProvider consulSettingsProvider, 
            IAccessTokenProvider accessTokenProvider, IConfiguration configuration, ILogger<ExternalLicensingManagementService> logger)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
            _httpClientService = httpClientService;
            _consulSettingsProvider = consulSettingsProvider;
            _accessTokenProvider = accessTokenProvider;
            _configuration = configuration;
            _logger = logger;
            _doNothingHttpHeaderStrategy = new DoNothingHttpHeaderStrategy();
        }

        public async Task<TenantLicenses> GetLicenseByTenantId(Guid tenantId)
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
                _logger.LogError(e, "Could not get license for tenant {TenantId}", tenantId);
                throw new Exception(ExceptionMessageConsts.CouldNotLoadLicensesFromRemoteServer);
            }
        }
        
        public async Task<UpdateHardwareIdOutput> UpdateHardwareId(UpdateHardwareIdInput input)
        {
            try
            {
                if (DefaultConfigurationConsts.IsRunningAsLegacy)
                {
                    return await UpdateHardwareIdLegacy(input);
                }

                return await UpdateHardwareIdWeb(input);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update hardware id for tenant {TenantId}", input.TenantId);
                throw new Exception(ExceptionMessageConsts.CouldNotLoadLicensesFromRemoteServer);
            }
        }
        
        public async Task<UpdateNamedUserAppLicenseOutput> UpdateNamedUserApp(UpdateNamedUserAppLicenseInput input, Guid hostTenantId, Guid licensedTenant, Guid licensedApp, Guid namedUserAppId)
        {
            try
            {
                if (DefaultConfigurationConsts.IsRunningAsLegacy)
                    return await UpdateNamedUserAppLicenseLegacy(input, hostTenantId, licensedTenant, licensedApp, namedUserAppId);
                    
                return await UpdateNamedUserAppLicenseWeb(input, hostTenantId, licensedTenant, licensedApp, namedUserAppId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update named app for tenant {TenantId}", licensedTenant);
                throw;
            }
        }
        
        public async Task<UpdateNamedUserBundleLicenseOutput> UpdateNamedUserBundle(UpdateNamedUserBundleLicenseInput input, Guid hostTenantId, Guid licensedTenant, 
            Guid licensedBundle, Guid namedUserBundleId)
        {
            try
            {
                if (DefaultConfigurationConsts.IsRunningAsLegacy)
                    return await UpdateNamedUserBundleLicenseLegacy(input, hostTenantId, licensedTenant, licensedBundle, namedUserBundleId);

                return await UpdateNamedUserBundleLicenseWeb(input, hostTenantId, licensedTenant, licensedBundle, namedUserBundleId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update named bundle for tenant {TenantId}", licensedTenant);
                throw;
            }
        }
        
        private async Task<TenantLicenses> GetLicenseByTenantIdWeb(Guid tenantId)
        {
            var accessToken = await GetAccessToken();
            var endpoint = ExternalServiceConsts.LicensingManagement.Licenses.GetLicensingLicenses(tenantId);

            var call = _apiClientCallBuilder.WithEndpoint(endpoint)
                .WithServiceName(ExternalServiceConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .WithHttpHeaderStrategy(_doNothingHttpHeaderStrategy)
                .WithAccessToken(accessToken)
                .WithTimeout(ApiServiceCallTimeout.Long)
                .Build();
            
            return await call.ResponseCallAsync<TenantLicenses>();
        }

        private async Task<TenantLicenses> GetLicenseByTenantIdLegacy(Guid tenantId)
        {
            var accessToken = await LegacyGetAccessToken();
            var endpoint = ExternalServiceConsts.LicensingManagement.Licenses.GetLicensingLicenses(tenantId);
            
            var call = _httpClientService.GetRequest(endpoint, HttpMethod.Get, null, accessToken);
            var responseMessage = await _httpClientService.SendAsync(call);
            
            if (responseMessage.IsSuccessStatusCode)
            {
                var content = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TenantLicenses>(content);
            }

            throw new Exception(responseMessage.ReasonPhrase);
        }

        private async Task<UpdateNamedUserAppLicenseOutput> UpdateNamedUserAppLicenseWeb(UpdateNamedUserAppLicenseInput input, Guid hostTenantId, Guid licensedTenant, Guid licensedApp, 
            Guid namedUserAppId)
        {
            var accessToken = await GetAccessToken();
            var endpoint = ExternalServiceConsts.LicensingManagement.LicensedApp.UpdateNamedUserAppLicense(licensedTenant, licensedApp, namedUserAppId);
            
            var call = _apiClientCallBuilder.WithEndpoint(endpoint)
                .WithServiceName(ExternalServiceConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Put)
                .WithBody(input)
                .WithHttpHeaderStrategy(new HostTenantIdHttpHeaderStrategy(hostTenantId))
                .WithAccessToken(accessToken)
                .DontThrowOnFailureCall()
                .Build();

            return await call.ResponseCallAsync<UpdateNamedUserAppLicenseOutput>();
        }

        private async Task<UpdateNamedUserBundleLicenseOutput> UpdateNamedUserBundleLicenseWeb(UpdateNamedUserBundleLicenseInput input, Guid hostTenantId, Guid licensedTenant, 
            Guid licensedBundle, Guid namedUserBundleId)
        {
            var accessToken = await GetAccessToken();
            var endpoint = ExternalServiceConsts.LicensingManagement.LicensedBundle.UpdateNamedUserBundleLicense(licensedTenant, licensedBundle, namedUserBundleId);
            
            var call = _apiClientCallBuilder.WithEndpoint(endpoint)
                .WithServiceName(ExternalServiceConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Put)
                .WithBody(input)
                .WithHttpHeaderStrategy(new HostTenantIdHttpHeaderStrategy(hostTenantId))

                .WithAccessToken(accessToken)
                .DontThrowOnFailureCall()
                .Build();

            return await call.ResponseCallAsync<UpdateNamedUserBundleLicenseOutput>();
        }

        private async Task<UpdateNamedUserAppLicenseOutput> UpdateNamedUserAppLicenseLegacy(UpdateNamedUserAppLicenseInput input, Guid hostTenantId, Guid licensedTenant, 
            Guid licensedApp, Guid namedUserAppId)
        {
            var accessToken = await LegacyGetAccessToken();
            var endpoint = ExternalServiceConsts.LicensingManagement.LicensedApp.UpdateNamedUserAppLicense(licensedTenant, licensedApp, namedUserAppId);
            
            var call = _httpClientService.GetRequest(endpoint, HttpMethod.Put, new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(input)), accessToken);
            call.Headers.Add("TenantId", hostTenantId.ToString());
            
            var responseMessage = await _httpClientService.SendAsync(call);

            var content = await responseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<UpdateNamedUserAppLicenseOutput>(content);
        }

        private async Task<UpdateNamedUserBundleLicenseOutput> UpdateNamedUserBundleLicenseLegacy(UpdateNamedUserBundleLicenseInput input, Guid hostTenantId, Guid licensedTenant, 
            Guid licensedBundle, Guid namedUserBundleId)
        {
            var accessToken = await LegacyGetAccessToken();
            var endpoint = ExternalServiceConsts.LicensingManagement.LicensedBundle.UpdateNamedUserBundleLicense(licensedTenant, licensedBundle, namedUserBundleId);
            
            var call = _httpClientService.GetRequest(endpoint, HttpMethod.Put, new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(input)), accessToken);
            call.Headers.Add("TenantId", hostTenantId.ToString());
            
            var responseMessage = await _httpClientService.SendAsync(call);

            var content = await responseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<UpdateNamedUserBundleLicenseOutput>(content);
        }

        private async Task<UpdateHardwareIdOutput> UpdateHardwareIdLegacy(UpdateHardwareIdInput input)
        {
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

        private async Task<UpdateHardwareIdOutput> UpdateHardwareIdWeb(UpdateHardwareIdInput input)
        {
            var accessToken = await GetAccessToken();
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

        private async Task<string> GetAccessToken()
        {
            var licensingManagementSecret = _configuration["Authentication:LicensingManagementSecret"];
            var authority = _configuration["Authentication:Authority"];
            var accessToken = await _accessTokenProvider.ProvideAccessToken(licensingManagementSecret, "Viasoft.LicensingManagement.LicenseServer", 
                "Viasoft.LicensingManagement.LicenseServer", authority);
            return accessToken;
        }
        
        private async Task<string> LegacyGetAccessToken()
        {
            var consulSettings = _consulSettingsProvider.GetSettingsFromConsul();
            var accessToken = await _accessTokenProvider.ProvideAccessToken(consulSettings.Authentication.LicensingManagementSecret, 
                "Viasoft.LicensingManagement.LicenseServer", "Viasoft.LicensingManagement.LicenseServer", consulSettings.Authentication.Authority);
            return accessToken;
        }
    }
}