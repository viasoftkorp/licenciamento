export enum AddNamedUserToProductValidationCode {
    NoError,
    NoLicensedTenant,
    NoProduct,
    TooManyNamedUsers,
    ProductIsNotNamed,
    NamedUserEmailAlreadyInUse
}