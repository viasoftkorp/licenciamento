using System;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.Message
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.LicensedTenantUpdatedMessage")]
    public class LicensedTenantUpdatedMessage : IMessage, IInternalMessage
    {
        public LicensingStatus Status { get; set; }

        public Guid Identifier { get; set; }

        public Guid LicensedTenantId { get; set; }
        
        public DateTime? ExpirationDateTime { get; set; }
        
        public string LicensedCnpjs { get; set; }

        public string AdministratorEmail { get; set; }

        public Guid AccountId { get; set; }
        
        public string HardwareId { get; set; }
    }
}