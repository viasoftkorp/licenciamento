using System;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Messages
{
    [Endpoint("Licensing.LicensingManagement.LicensingDetailsUpdated")]
    public class LicensingDetailsUpdated: IMessage, IInternalMessage
    {
        public Guid TenantId { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public LicenseByTenantIdOld LicenseByIdentifier { get; set; }
    }
}