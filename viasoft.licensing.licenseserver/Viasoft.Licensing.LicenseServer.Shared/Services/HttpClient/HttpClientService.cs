using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Shared.Services.HttpClient
{
    public class HttpClientService: IHttpClientService, ITransientDependency
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HttpClientService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.SendAsync(request);
                return response;
            }
        }

        public HttpRequestMessage GetRequest(string relativeUri, HttpMethod httpMethod, HttpContent content, string accessToken)
        {
            var uriGateway = _configuration[EnvironmentVariableConsts.UrlGateway];
            var serviceEndpoint = new Uri(new Uri(uriGateway), relativeUri);
            
            var request = new HttpRequestMessage(httpMethod, serviceEndpoint);
            request.Content = content;
            if (!string.IsNullOrEmpty(accessToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            return request;
        }
    }
}