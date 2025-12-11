namespace Viasoft.Licensing.CustomerLicensing.Domain.Enums
{
    public enum OperationValidation
    {
        NoError = 0,
        DuplicatedEntry = 1,
        DuplicatedIdentifier = 2,
        UsedByOtherRegister = 3,
        AppAlreadyLicensed = 4,
        LicenseDoesNotExist = 5,
        CantRemoveFromLicenseDefaultApp = 6,
        NoLicensedAppWithSuchId = 7,
        CnpjAlreadyRegistered = 8,
        AccountIdAlreadyInUse = 9,
        InvalidNumberOfLicenses = 10,
        AdministrationEmailAlreadyInUse = 11,
        NoTenantWithSuchId = 12,
        InvalidGateway = 13,
        InfrastructureConfigurationAlreadyExists = 14,
        NoAccountWithSuchId = 15,
        BundleDoesNotExist = 16,
        UnknownError = 17,
        NoLicensingMode = 18,
        NoLicensedBundleWithSuchId = 19,
        NotANamedLicense= 20,
        TooManyNamedUserBundleLicenses = 21,
        NamedUserEmailAlreadyInUse = 22
    }
}