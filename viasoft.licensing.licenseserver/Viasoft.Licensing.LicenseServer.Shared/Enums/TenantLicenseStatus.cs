namespace Viasoft.Licensing.LicenseServer.Shared.Enums
{
    public enum TenantLicenseStatus
    {
        NeedsApproval = 0,
        Blocked = 1,
        Trial = 2,
        Active = 3,
        TenantNotFound = 4,
        ReadOnly = 5
    }
}