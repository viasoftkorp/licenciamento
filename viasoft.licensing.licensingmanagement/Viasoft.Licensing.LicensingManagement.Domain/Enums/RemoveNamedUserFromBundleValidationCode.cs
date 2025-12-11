namespace Viasoft.Licensing.LicensingManagement.Domain.Enums
{
    public enum RemoveNamedUserFromBundleValidationCode
    {
        NoError = 0,
        NoLicensedTenant = 1,
        NoLicensedBundle = 2,
        NoNamedUser = 3
    }
}