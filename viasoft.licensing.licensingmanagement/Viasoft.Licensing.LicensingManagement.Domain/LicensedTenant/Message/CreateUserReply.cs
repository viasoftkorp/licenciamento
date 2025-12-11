using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message
{
    [Endpoint("Viasoft.Authentication.CreateUserReply")]
    public class CreateUserReply: IMessage, IInternalMessage
    {
        public string Status { get; set; }
        public Guid? UserId { get; set; }
        public Guid TenantId { get; set; }
        public string ErrorDescription { get; set; }
        public bool Success { get; set; }
    }
}