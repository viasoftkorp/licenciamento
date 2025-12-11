export enum UpdateNamedUsersFromProductValidationCode {
    NoError,
    NoLicensedTenant,
    NoProduct,
    NoNamedUser,
    NamedUserEmailAlreadyInUse
}