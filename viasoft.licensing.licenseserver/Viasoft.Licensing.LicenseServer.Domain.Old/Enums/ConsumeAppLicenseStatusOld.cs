namespace Viasoft.Licensing.LicenseServer.Domain.Old.Enums
{
    public enum ConsumeAppLicenseStatusOld
    {
        LicenseConsumed = 0,
        AdditionalLicenseConsumed = 1,
        NotEnoughLicenses = 2,
        AppExpired = 3,
        AppBlocked = 4,
        TenantBlocked = 5,
        AppNotLicensed = 6,
        CnpjNotLicensed = 7,
        LicenseAlreadyInUseByUser = 8,
        AdditionalLicenseAlreadyInUseByUser = 9,
        TenantLicensingNotLoaded = 10,
        HardwareIdDoesNotMatch = 11
    }
}