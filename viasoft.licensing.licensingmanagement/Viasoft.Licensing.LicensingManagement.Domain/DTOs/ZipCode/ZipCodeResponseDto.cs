using Newtonsoft.Json;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.ZipCode
{
    public class ZipCodeResponseDto
    {
        [JsonProperty("logradouro")]
        public string Street { get; set; }

        [JsonProperty("bairro")]
        public string Neighborhood { get; set; }

        [JsonProperty("cidade")]
        public string City { get; set; }

        [JsonProperty("uf")]
        public string State { get; set; }

        public string Status { get; set; }
        public string Message { get; set; }
    }
}