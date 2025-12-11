namespace Viasoft.Licensing.LicenseServer.Domain.Enums
{
    public enum UpdateNamedUserAppLicenseValidationCode
    {
        NoError = 0,
        NoLicensedTenant = 1,
        NoLicensedApp = 2,
        NoNamedUser = 3
    }
}