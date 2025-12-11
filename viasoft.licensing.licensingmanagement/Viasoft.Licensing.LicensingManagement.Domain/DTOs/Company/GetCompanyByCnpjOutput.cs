using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Company
{
    public class GetCompanyByCnpjOutput
    {
        public Guid Id { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Neighborhood { get; set; }
        public string Efr { get; set; }
        public string Uf { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Cnpj { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string Cep { get; set; }
        public string TradingName { get; set; }
        public string Status { get; set; }
        public OperationValidation OperationValidation { get; set; }
        public string OperationValidationDescription => OperationValidation.ToString();
    }
}