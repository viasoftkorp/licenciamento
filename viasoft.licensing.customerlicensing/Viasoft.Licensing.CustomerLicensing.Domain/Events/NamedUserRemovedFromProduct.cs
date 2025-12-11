using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Events
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.NamedUserRemoved")]
    public class NamedUserRemovedFromProduct : IMessage, IInternalMessage
    {
        public Guid LicensingIdentifier { get; set; }
        public string NamedUserEmail { get; set; }
    }
}