using System;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.CustomerLicensing.Domain.Contracts.LicensingManagement;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Messages
{
    [Endpoint("Licensing.LicensingManagement.LicensingDetailsUpdated")]
    public class LicensingDetailsUpdated: IMessage, IInternalMessage
    {
        public Guid TenantId { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public LicenseByIdentifier LicenseByIdentifier { get; set; }
    }
}