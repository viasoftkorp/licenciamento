using System;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Extensions
{
    public static class AddNamedUserToProductValidationCodeExtensions
    {
        public static AddNamedUserToProductValidationCode ToAddNamedUserToProductValidationCode(this AddNamedUserToLicensedAppValidationCode validationCode)
        {
            switch (validationCode)
            {
                case AddNamedUserToLicensedAppValidationCode.NoError:
                    return AddNamedUserToProductValidationCode.NoError;
                case AddNamedUserToLicensedAppValidationCode.NoLicensedTenant:
                    return AddNamedUserToProductValidationCode.NoLicensedTenant;
                case AddNamedUserToLicensedAppValidationCode.NoLicensedApp:
                    return AddNamedUserToProductValidationCode.NoProduct;
                case AddNamedUserToLicensedAppValidationCode.TooManyNamedUsers:
                    return AddNamedUserToProductValidationCode.TooManyNamedUsers;
                case AddNamedUserToLicensedAppValidationCode.LicensedAppIsNotNamed:
                    return AddNamedUserToProductValidationCode.ProductIsNotNamed;
                case AddNamedUserToLicensedAppValidationCode.NamedUserEmailAlreadyInUse:
                    return AddNamedUserToProductValidationCode.NamedUserEmailAlreadyInUse;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}