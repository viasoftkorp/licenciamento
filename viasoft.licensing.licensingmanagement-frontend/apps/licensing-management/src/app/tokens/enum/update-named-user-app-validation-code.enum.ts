export enum UpdateNamedUserAppValidationCode {
    NoError = 0,
    NoLicensedTenant = 1,
    NoLicensedApp = 2,
    NoNamedUser = 3,
    NamedUserEmailAlreadyInUse = 4
}
