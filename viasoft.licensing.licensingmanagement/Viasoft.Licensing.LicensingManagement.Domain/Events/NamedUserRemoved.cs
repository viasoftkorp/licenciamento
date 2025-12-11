using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.Events
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.NamedUserRemoved")]
    public class NamedUserRemoved : IMessage, IInternalMessage
    {
        public Guid LicensingIdentifier { get; set; }
        public string NamedUserEmail { get; set; }
    }
}