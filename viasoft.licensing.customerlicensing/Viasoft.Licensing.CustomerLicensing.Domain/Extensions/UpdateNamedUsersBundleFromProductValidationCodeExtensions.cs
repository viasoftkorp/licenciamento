using System;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Extensions
{
    public static class UpdateNamedUsersBundleFromProductValidationCodeExtensions
    {
        public static UpdateNamedUsersFromProductValidationCode ToUpdateNamedUserBundleToProductValidationCode(this UpdateNamedUsersFromBundleValidationCode validationCode)
        {
            switch (validationCode)
            {
                case UpdateNamedUsersFromBundleValidationCode.NoError:
                    return UpdateNamedUsersFromProductValidationCode.NoError;
                case UpdateNamedUsersFromBundleValidationCode.NoLicensedTenant:
                    return UpdateNamedUsersFromProductValidationCode.NoLicensedTenant;
                case UpdateNamedUsersFromBundleValidationCode.NoLicensedBundle:
                    return UpdateNamedUsersFromProductValidationCode.NoProduct;
                case UpdateNamedUsersFromBundleValidationCode.NoNamedUser:
                    return UpdateNamedUsersFromProductValidationCode.NoNamedUser;
                case UpdateNamedUsersFromBundleValidationCode.NamedUserEmailAlreadyInUse:
                    return UpdateNamedUsersFromProductValidationCode.NamedUserEmailAlreadyInUse;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}