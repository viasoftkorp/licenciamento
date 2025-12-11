using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant
{
    public class LicensedAppDetailsByTenant
    {
        public Guid LicensedTenantIdentifier { get; set; }
        public LicensedAppDetailsTemporaryLicenses LicensedAppDetailsTemporaryLicenses { get; set; }
    }
}