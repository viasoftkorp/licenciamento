namespace Viasoft.Licensing.LicenseServer.Domain.Enums
{
    public enum ConsumeAppLicenseStatus
    {
        LicenseConsumed = 0,
        NotEnoughLicenses = 2,
        AppExpired = 3,
        AppBlocked = 4,
        TenantBlocked = 5,
        AppNotLicensed = 6,
        CnpjNotLicensed = 7,
        LicenseAlreadyInUseByUser = 8,
        TenantLicensingNotLoaded = 10,
        HardwareIdDoesNotMatch = 11,
        NamedBundleLicenseNotAvailable = 12,
        NamedAppLicenseNotAvailable = 13,
        CantConsumeOnlineNamedLicenseInDifferentComputers = 14,
        NamedUserLicenseDeviceIdDoesNotMatch = 15,
        CouldNotUpdateNamedUserAppLicenseDueToNoLicensedTenant = 16,
        CouldNotUpdateNamedUserAppLicenseDueToNoLicensedApp = 17,
        CouldNotUpdateNamedUserAppLicenseDueToNoNamedUser = 18,
        CouldNotUpdateNamedUserBundleLicenseDueToNoLicensedTenant = 19,
        CouldNotUpdateNamedUserBundleLicenseDueToNoLicensedBundle = 20,
        CouldNotUpdateNamedUserBundleLicenseDueToNoNamedUser = 21,
        EmptyDeviceId = 22
    }
}