using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message
{
    [Endpoint("Viasoft.Authorization.AddRootUserToTenant", "Viasoft.Authorization")]
    public class AddRootUserToTenantCommand: ICommand, IInternalMessage
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
    }
}

