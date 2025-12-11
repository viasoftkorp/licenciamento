using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message
{
    [Endpoint("Viasoft.Authorization.RemoveRootUserFromTenant", "Viasoft.Authorization")]
    public class RemoveRootUserCommand: ICommand, IInternalMessage
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
    }
}