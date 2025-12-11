using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.Messages
{
    public class RootUserFromTenantId
    {
        public string Email { get; set; }
        
        public Guid TenantId { get; set; }
    }
}