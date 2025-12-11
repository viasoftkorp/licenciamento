using System;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.DTO
{
    public class AccountOutput
    {
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
        
        public Guid TenantId { get; set; }
    }
}