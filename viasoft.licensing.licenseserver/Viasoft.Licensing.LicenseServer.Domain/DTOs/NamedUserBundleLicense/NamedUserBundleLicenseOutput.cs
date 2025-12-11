using System;
using Viasoft.Licensing.LicenseServer.Domain.Abstractions.NamedUserLicense;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense
{
    public class NamedUserBundleLicenseOutput: INamedUserLicense
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid LicensedTenantId { get; set; }
        public Guid LicensedBundleId { get; set; }
        public Guid NamedUserId { get; set; }
        public string NamedUserEmail { get; set; }
        public string DeviceId { get; set; }
    }
}