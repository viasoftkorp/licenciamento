using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp
{
    public class AlreadyLicensedApp
    {
        public Guid AppId { get; set; }
        public Guid LicensedTenantId { get; set; }
    }
}