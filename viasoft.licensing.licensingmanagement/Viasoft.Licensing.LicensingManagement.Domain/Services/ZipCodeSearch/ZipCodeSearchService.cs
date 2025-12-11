using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.ZipCode;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.ZipCodeSearch
{
    public class ZipCodeSearchService : IZipCodeSearchService, ITransientDependency
    {

        private readonly IHttpClientFactory _client;

        public ZipCodeSearchService(IHttpClientFactory client)
        {
            _client = client;
        }

        public async Task<ZipCodeResponseDto> GetAddress(string cep)
        {
            using var client = _client.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", "IU1fMnmYrc4JngM4vAy089gyjnLhS0oP6Pt4VcRM");

            var response = await client.GetAsync(
                "https://api.korp.com.br/v1/cep/get?cep=" + cep
            );
            var result = await response.Content.ReadAsStringAsync();

            var output = JsonConvert.DeserializeObject<ZipCodeResponseDto>(result, new JsonSerializerSettings
                
            {
                NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore
            });

            return output;
        }
    }
}