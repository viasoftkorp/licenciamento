using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO
{
    public class BundledAppIds
    {
        public Guid BundleId { get; set; }
        public Guid AppId { get; set; }
    }
}