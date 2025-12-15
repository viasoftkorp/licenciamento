using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage
{
    public class RefreshAppLicenseInUseByUserOutputOld
    {
        public RefreshAppLicenseInUseByUserStatusOld Status { get; set; }
        public string StatusDescription => Status.ToString();
    }
}