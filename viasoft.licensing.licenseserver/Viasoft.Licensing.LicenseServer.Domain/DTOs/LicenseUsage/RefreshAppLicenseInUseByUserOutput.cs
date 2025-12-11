using Viasoft.Licensing.LicenseServer.Domain.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage
{
    public class RefreshAppLicenseInUseByUserOutput
    {
        public RefreshAppLicenseInUseByUserStatus Status { get; set; }
        public string StatusDescription => Status.ToString();
    }
}