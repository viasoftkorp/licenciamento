using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.Message
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.DeleteLicensedTenant")]
    public class DeleteLicensedTenantMessage : IMessage, IInternalMessage
    {
        public Guid LicensedTenantId { get; set; }
    }
}