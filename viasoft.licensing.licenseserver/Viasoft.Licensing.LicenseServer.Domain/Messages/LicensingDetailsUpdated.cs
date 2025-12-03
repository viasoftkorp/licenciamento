using System;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Messages
{
    [Endpoint("Licensing.LicensingManagement.LicensingDetailsUpdated")]
    public class LicensingDetailsUpdated: IMessage, IInternalMessage
    {
        public Guid TenantId { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public LicenseByTenantId LicenseByIdentifier { get; set; }
    }
}