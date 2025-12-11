using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.Message
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.CreateAccount")]
    public class CreateAccountCommand: IMessage, IInternalMessage
    {
        public string Phone { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string BillingEmail { get; set; }
        public string TradingName { get; set; }
        public string CompanyName { get; set; }
        public string Cnpj { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Detail { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public Guid Id { get; set; }
    }
}