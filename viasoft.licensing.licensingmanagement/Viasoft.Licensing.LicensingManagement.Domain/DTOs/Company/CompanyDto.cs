using System;
using Newtonsoft.Json;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Company
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        [JsonProperty("telefone")]
        public string Phone { get; set; }
        public string Email { get; set; }
        [JsonProperty("bairro")]
        public string Neighborhood { get; set; }
        public string Efr { get; set; }
        public string Uf { get; set; }
        [JsonProperty("logradouro")]
        public string Street { get; set; }
        [JsonProperty("municipio")]
        public string City { get; set; }
        public string Cnpj { get; set; }
        [JsonProperty("nome")]
        public string Name { get; set; }
        [JsonProperty("numero")]
        public string Number { get; set; }
        [JsonProperty("complemento")]
        public string Complement { get; set; }
        public string Cep { get; set; }
        [JsonProperty("fantasia")]
        public string TradingName { get; set; }
        public string Status { get; set; }
        public OperationValidation OperationValidation { get; set; }
        public string OperationValidationDescription => OperationValidation.ToString();
    }
}