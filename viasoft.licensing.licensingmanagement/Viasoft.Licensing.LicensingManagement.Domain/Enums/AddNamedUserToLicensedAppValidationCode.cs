namespace Viasoft.Licensing.LicensingManagement.Domain.Enums
{
    public enum AddNamedUserToLicensedAppValidationCode
    {
        NoError = 0,
        NoLicensedTenant = 1,
        NoLicensedApp = 2,
        TooManyNamedUsers = 3,
        LicensedAppIsNotNamed = 4,
        NamedUserEmailAlreadyInUse = 5
    }
}