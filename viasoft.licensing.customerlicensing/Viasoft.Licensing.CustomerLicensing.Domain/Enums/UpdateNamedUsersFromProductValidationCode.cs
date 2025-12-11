namespace Viasoft.Licensing.CustomerLicensing.Domain.Enums
{
    public enum UpdateNamedUsersFromProductValidationCode
    {
        NoError = 0,
        NoLicensedTenant = 1,
        NoProduct = 2,
        NoNamedUser = 3,
        NamedUserEmailAlreadyInUse = 4
    }
}