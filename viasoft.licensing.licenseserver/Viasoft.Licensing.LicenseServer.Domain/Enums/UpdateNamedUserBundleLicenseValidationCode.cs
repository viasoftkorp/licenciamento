namespace Viasoft.Licensing.LicenseServer.Domain.Enums
{
    public enum UpdateNamedUserBundleLicenseValidationCode
    {
        NoError = 0,
        NoLicensedTenant = 1,
        NoLicensedBundle = 2,
        NoNamedUser = 3
    }
}