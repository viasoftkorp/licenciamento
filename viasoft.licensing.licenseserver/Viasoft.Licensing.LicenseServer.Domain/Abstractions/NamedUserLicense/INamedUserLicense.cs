using System;

namespace Viasoft.Licensing.LicenseServer.Domain.Abstractions.NamedUserLicense
{
    public interface INamedUserLicense
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid LicensedTenantId { get; set; }
        public Guid NamedUserId { get; set; }
        public string NamedUserEmail { get; set; }
        public string DeviceId { get; set; }
    }
}