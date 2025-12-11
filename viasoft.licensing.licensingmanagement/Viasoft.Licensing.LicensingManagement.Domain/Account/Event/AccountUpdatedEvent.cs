using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.Event
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.AccountUpdatedMessage")]
    public class AccountUpdatedEvent : IEvent, IInternalMessage
    {
        public Guid AccountId { get; set; }
        public string CompanyName { get; set; }
    }
}