using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense
{
    public class UpdateNamedUsersFromBundleOutput
    {
        public bool Success { get; set; }
        public UpdateNamedUsersFromBundleValidationCode ValidationCode { get; set; }
    }
}