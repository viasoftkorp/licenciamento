namespace Viasoft.Licensing.LicensingManagement.Domain.Enums
{
    public enum DeleteNamedUsersFromAppValidationCode
    {
        NoError = 0,
        NoLicensedTenant = 1,
        NoLicensedApp = 2,
        NoNamedUser = 3
    }
}