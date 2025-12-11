using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Viasoft.Core.IoC.Abstractions;

namespace Viasoft.Licensing.LicenseServer.Shared.AccessTokenProvider
{
    public class AccessTokenProvider: IAccessTokenProvider, ITransientDependency
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public AccessTokenProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }
        
        public async Task<string> ProvideAccessToken(string clientSecret, string scope, string clientId, string authority)
        {
            if (TryGetCachedAccessToken(clientId, scope, out var accessToken))
                return accessToken;
            
            using (var client = _httpClientFactory.CreateClient())
            {
                var discoveryDocumentRequest = new DiscoveryDocumentRequest
                {
                    Address = authority,
                    Policy = new DiscoveryPolicy
                    {
                        RequireHttps = _configuration["ASPNETCORE_ENVIRONMENT"] == "Production"
                    }
                };
            
                var discoveryDocument = await client.GetDiscoveryDocumentAsync(discoveryDocumentRequest);
                
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = scope
                });
                
                CacheNewToken(tokenResponse.AccessToken, clientId, scope, tokenResponse.ExpiresIn);

                return tokenResponse.AccessToken;
            }
        }

        private void CacheNewToken(string accessToken, string clientId, string scope, int expirationTimeInMinutes)
        {
            var key = string.Concat(clientId, scope);
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now + (TimeSpan.FromSeconds(expirationTimeInMinutes) - TimeSpan.FromMinutes(5))
            };

            _memoryCache.Set(key, accessToken, cacheEntryOptions);
        }

        private bool TryGetCachedAccessToken(string clientId, string scope, out string accessToken)
        {
            var key = string.Concat(clientId, scope);
            return _memoryCache.TryGetValue(key, out accessToken);
        }
    }
}