using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Events
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.NamedUserRemovedFromBundle")]
    public class NamedUserRemovedFromBundle : IMessage, IInternalMessage
    {
        public Guid LicensingIdentifier { get; set; }
        public string NamedUserEmail { get; set; }
    }
}