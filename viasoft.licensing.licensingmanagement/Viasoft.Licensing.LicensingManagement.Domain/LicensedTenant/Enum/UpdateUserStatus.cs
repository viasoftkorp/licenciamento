namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum
{
    public enum UpdateUserStatus
    {
        Ok = 0,
        UserDoesNotExist = 1,
        EmailAlreadyInUse = 2,
        AdminIsNotFromSameHostTenant = 3,
        LoginAlreadyInUse = 4
    }
}