using System;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Extensions
{
    public static class UpdateNamedUsersAppFromProductValidationCodeExtensions
    {
        public static UpdateNamedUsersFromProductValidationCode ToUpdateNamedUserAppToProductValidationCode(this UpdateNamedUserAppLicenseValidationCode validationCode)
        {
            switch (validationCode)
            {
                case UpdateNamedUserAppLicenseValidationCode.NoError:
                    return UpdateNamedUsersFromProductValidationCode.NoError;
                case UpdateNamedUserAppLicenseValidationCode.NoLicensedTenant:
                    return UpdateNamedUsersFromProductValidationCode.NoLicensedTenant;
                case UpdateNamedUserAppLicenseValidationCode.NoLicensedApp:
                    return UpdateNamedUsersFromProductValidationCode.NoProduct;
                case UpdateNamedUserAppLicenseValidationCode.NoNamedUser:
                    return UpdateNamedUsersFromProductValidationCode.NoNamedUser;
                case UpdateNamedUserAppLicenseValidationCode.NamedUserEmailAlreadyInUse:
                    return UpdateNamedUsersFromProductValidationCode.NamedUserEmailAlreadyInUse;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}