using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message;

[Endpoint("Viasoft.Authorization.RemoveRootUserFromTenantReply")]
public class RemoveRootUserReply: IMessage, IInternalMessage
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public bool Success { get; set; }
    public string Status { get; set; }
    public string ErrorDescription { get; set; }
}