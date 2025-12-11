using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle
{
    public class UpdateNamedUsersFromBundleOutput
    {
        public bool Success { get; set; }
        public UpdateNamedUsersFromBundleValidationCode ValidationCode { get; set; }
    }
}