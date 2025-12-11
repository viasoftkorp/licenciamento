using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message
{
    [Endpoint("Viasoft.Authentication.CreateUserIfNotExists", "Viasoft.Authentication")]
    public class CreateUserIfNotExistsCommand: ICommand, IInternalMessage
    {
        public string Email { get; set; }
        public Guid? UserId { get; set; }
        public Guid TenantId { get; set; }

        public override bool Equals(object obj)
        {
            var command = (CreateUserIfNotExistsCommand)obj;
            return command != null &&
                   command.Email.Equals(Email) && command.UserId.Equals(UserId) && command.TenantId.Equals(TenantId);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}