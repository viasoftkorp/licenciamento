using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense
{
    public class NamedUserAppLicenseOutput
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid LicensedTenantId { get; set; }
        public Guid LicensedAppId { get; set; }
        public Guid NamedUserId { get; set; }
        public string NamedUserEmail { get; set; }
        public string DeviceId { get; set; }
    }
}