namespace Viasoft.Licensing.LicenseServer.Domain.Enums
{
    public enum ReleaseAppLicenseStatus
    {
        LicenseReleased = 0,
        NoConsumedLicenseToRelease = 2,
        AppExpired = 3,
        AppBlocked = 4,
        TenantBlocked = 5,
        AppNotLicensed = 6,
        CnpjNotLicensed = 7,
        LicenseStillInUseByUser = 8,
        TenantLicensingNotLoaded = 10
    }
}