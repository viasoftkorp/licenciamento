namespace Viasoft.Licensing.LicensingManagement.Domain.Enums
{
    public enum UpdateNamedUsersFromBundleValidationCode
    {
        NoError = 0,
        NoLicensedTenant = 1,
        NoLicensedBundle = 2,
        NoNamedUser = 3,
        NamedUserEmailAlreadyInUse = 4
    }
}