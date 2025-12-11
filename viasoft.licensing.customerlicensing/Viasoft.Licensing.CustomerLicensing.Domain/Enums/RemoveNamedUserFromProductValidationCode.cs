namespace Viasoft.Licensing.CustomerLicensing.Domain.Enums
{
    public enum RemoveNamedUserFromProductValidationCode
    {
        NoError = 0,
        NoLicensedTenant = 1,
        NoProduct = 2,
        NoNamedUser = 3
    }
}