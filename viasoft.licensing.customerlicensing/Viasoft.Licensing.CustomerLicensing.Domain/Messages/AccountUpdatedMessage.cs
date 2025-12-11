using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Messages
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.AccountUpdatedMessage")]
    public class AccountUpdatedMessage : IMessage, IInternalMessage
    {
        public Guid AccountId { get; set; }
        public string CompanyName { get; set; }
    }
}