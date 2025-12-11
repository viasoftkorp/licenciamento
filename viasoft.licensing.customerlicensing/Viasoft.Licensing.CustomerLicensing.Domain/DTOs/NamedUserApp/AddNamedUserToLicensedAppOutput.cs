using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserApp
{
    public class AddNamedUserToLicensedAppOutput
    {
        public AddNamedUserToLicensedAppValidationCode ValidationCode { get; set; }
        public NamedUserAppLicenseOutput Output { get; set; }
    }
}