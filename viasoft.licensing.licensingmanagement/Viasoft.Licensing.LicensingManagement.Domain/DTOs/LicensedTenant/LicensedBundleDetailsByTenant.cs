using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant
{
    public class LicensedBundleDetailsByTenant
    {
        public Guid LicensedTenantIdentifier { get; set; }
        public LicensedBundleDetailsTemporaryLicenses LicensedBundleDetailsTemporaryLicenses { get; set; }
    }
}