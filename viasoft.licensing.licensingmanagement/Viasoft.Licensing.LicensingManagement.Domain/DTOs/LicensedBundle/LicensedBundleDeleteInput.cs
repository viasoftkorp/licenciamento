using System;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle
{
    public class LicensedBundleDeleteInput
    {
        [StrictRequired]
        public Guid LicensedTenantId { get; set; }
        
        [StrictRequired]
        public Guid BundleId { get; set; }

    }
}