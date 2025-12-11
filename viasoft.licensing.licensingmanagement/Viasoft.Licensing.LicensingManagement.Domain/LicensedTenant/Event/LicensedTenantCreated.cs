using System;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Event
{
    [Endpoint("Licensing.LicensingManagement.LicensedTenantCreated")]
    public class LicensedTenantCreated : IEvent, IInternalMessage
    {
        public Guid Id { get; set; }
        public string AdministratorEmail { get; set; }
        public Guid TenantId { get; set; }

        public LicensingStatus Status { get; set; }
        public string AccountName { get; set; }
    }
}