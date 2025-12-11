using System;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Event
{
    [Endpoint("Licensing.LicensingManagement.LicensingDetailsUpdated")]
    public class LicensingDetailsUpdated: IEvent, IInternalMessage
    {
        public Guid TenantId { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public LicenseByIdentifier LicenseByIdentifier { get; set; }
    }
}