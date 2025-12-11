using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.Message
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.LicensedTenantDeleted")]
    public class LicensedTenantDeletedMessage : IMessage, IInternalMessage
    {
        public Guid LicensedTenantId { get; set; }
    }
}