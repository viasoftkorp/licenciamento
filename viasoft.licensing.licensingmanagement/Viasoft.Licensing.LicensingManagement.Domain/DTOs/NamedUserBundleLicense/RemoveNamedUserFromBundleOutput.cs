using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense
{
    public class RemoveNamedUserFromBundleOutput
    {
        public bool Success { get; set; }
        public RemoveNamedUserFromBundleValidationCode ValidationCode { get; set; }
    }
}