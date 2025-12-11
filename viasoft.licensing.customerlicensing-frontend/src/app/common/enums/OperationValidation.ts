export enum OperationValidation {
    NoError = 0,
    NoTenantWithSuchId = 12,
    NoLicensedBundleWithSuchId = 19,
    NotANamedLicense= 20,
    TooManyNamedUserBundleLicenses = 21,
    NamedUserEmailAlreadyInUse = 22
}