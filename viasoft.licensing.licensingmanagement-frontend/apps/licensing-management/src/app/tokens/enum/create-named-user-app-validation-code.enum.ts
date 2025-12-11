export enum CreateNamedUserAppValidationCode {
    NoError = 0,
    NoLicensedTenant = 1,
    NoLicensedApp = 2,
    TooManyNamedUsers = 3,
    LicensedAppIsNotNamed = 4,
    NamedUserEmailAlreadyInUse = 5
}
