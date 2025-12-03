using System.Net.Http;
using System.Threading.Tasks;

namespace Viasoft.Licensing.LicenseServer.Shared.Services.HttpClient
{
    public interface IHttpClientService
    {
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
        public HttpRequestMessage GetRequest(string relativeUri, HttpMethod httpMethod, HttpContent content, string accessToken);
    }
}