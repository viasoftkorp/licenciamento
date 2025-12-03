namespace Viasoft.Licensing.LicenseServer.Domain.Enums
{
    public enum RefreshAppLicenseInUseByUserStatus
    {
        RefreshSuccessful = 0,
        RefreshSuccessfulLicenseConsumed = 1,
        RefreshFailedLicenseNotAvailable = 2,
        TenantLicensingNotLoaded = 3
    }
}