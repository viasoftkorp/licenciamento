using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense
{
    public class AddNamedUserToLicensedAppOutput
    {
        public AddNamedUserToLicensedAppValidationCode ValidationCode { get; set; }
        public NamedUserAppLicenseOutput Output { get; set; }
    }
}