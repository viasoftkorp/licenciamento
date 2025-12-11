using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.Events
{
    [Endpoint("Authentication.ApplicationUserUpdated")]
    public class ApplicationUserUpdatedEvent: IMessage
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}