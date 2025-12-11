using System;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.Command
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.LicensedTenantCreatedMessage", "Viasoft.Licensing.LicensingManagement")]
    public class LicensedTenantCreatedCommand : ICommand, IInternalMessage
    {
        public LicensingStatus Status { get; set; }

        public Guid Identifier { get; set; }

        public Guid LicensedTenantId { get; set; }
        
        public DateTime? ExpirationDateTime { get; set; }
        
        public string LicensedCnpjs { get; set; }

        public string AdministratorEmail { get; set; }

        public Guid AccountId { get; set; }
        
        public LicenseConsumeType LicenseConsumeType { get; set; }
        
        public string HardwareId { get; set; }
    }
}