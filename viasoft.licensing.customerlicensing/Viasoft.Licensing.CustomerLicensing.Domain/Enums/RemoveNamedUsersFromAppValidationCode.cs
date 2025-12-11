namespace Viasoft.Licensing.CustomerLicensing.Domain.Enums
{
    public enum RemoveNamedUsersFromAppValidationCode
    {
        NoError = 0,
        NoLicensedTenant = 1,
        NoLicensedApp = 2,
        NoNamedUser = 3
    }
}