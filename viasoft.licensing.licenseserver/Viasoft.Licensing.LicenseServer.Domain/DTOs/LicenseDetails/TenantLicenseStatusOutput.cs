using Viasoft.Licensing.LicenseServer.Shared.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseDetails
{
    public class TenantLicenseStatusOutput
    {
        public TenantLicenseStatus Status { get; set; }

        public string StatusDescription => Status.ToString();
    }
}