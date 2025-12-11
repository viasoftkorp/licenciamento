using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle
{
    public class RemoveNamedUserFromBundleOutput
    {
        public bool Success { get; set; }
        public RemoveNamedUserFromBundleValidationCode ValidationCode { get; set; }
    }
}