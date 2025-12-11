namespace Viasoft.Licensing.CustomerLicensing.Domain.Enums
{
    public enum AddNamedUserToProductValidationCode
    {
        NoError = 0,
        NoLicensedTenant = 1,
        NoProduct = 2,
        TooManyNamedUsers = 3,
        ProductIsNotNamed = 4,
        NamedUserEmailAlreadyInUse = 5
    }
}