namespace Viasoft.Licensing.LicenseServer.Domain.Old.Enums
{
    public enum ReleaseAppLicenseStatusOld
    {
        LicenseReleased = 0,
        AdditionalLicenseReleased = 1,
        NoConsumedLicenseToRelease = 2,
        AppExpired = 3,
        AppBlocked = 4,
        TenantBlocked = 5,
        AppNotLicensed = 6,
        CnpjNotLicensed = 7,
        LicenseStillInUseByUser = 8,
        AdditionalLicenseStillInUseByUser = 9,
        TenantLicensingNotLoaded = 10
    }
}