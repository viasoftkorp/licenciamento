using System;
using Viasoft.Core.DDD.Application.Dto.Entities;
using Viasoft.Data.Attributes;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.DTO
{
    public class AccountCreateInput : IEntityDto
    {
        [StrictRequired]
        public Guid Id { get; set; }
        
        public string Phone { get; set; }

        public string WebSite { get; set; }

        public string Email { get; set; }

        public string BillingEmail { get; set; }

        [StrictRequired]
        public string TradingName { get; set; }

        [StrictRequired]
        public string CompanyName { get; set; }

        [StrictRequired]
        public string CnpjCpf { get; set; }

        [StrictRequired]
        public AccountStatus Status {get; set; }

        public string Street { get; set; }

        public string Number { get; set; }

        public string Detail { get; set; }

        public string Neighborhood { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }
}