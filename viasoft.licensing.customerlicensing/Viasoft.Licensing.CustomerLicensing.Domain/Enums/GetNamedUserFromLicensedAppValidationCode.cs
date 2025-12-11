namespace Viasoft.Licensing.CustomerLicensing.Domain.Enums
{
    public enum GetNamedUserFromLicensedAppValidationCode
    {
        NoError = 0,
        NoLicensedTenant = 1,
        NoLicensedApp = 2
    }
}