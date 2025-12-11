using System;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.DTO
{
    public class AccountUpdateOutput
    {
        public Guid Id { get; set; }
        
        public string Phone { get; set; }

        public string WebSite { get; set; }

        public string Email { get; set; }

        public string BillingEmail { get; set; }

        public string TradingName { get; set; }

        public string CompanyName { get; set; }

        public string CnpjCpf { get; set; }

        public AccountStatus Status {get; set; }

        public string Street { get; set; }

        public string Number { get; set; }

        public string Detail { get; set; }

        public string Neighborhood { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
        
        public OperationValidation OperationValidation { get; set; }

        public string OperationValidationDescription => OperationValidation.ToString();
    }
}