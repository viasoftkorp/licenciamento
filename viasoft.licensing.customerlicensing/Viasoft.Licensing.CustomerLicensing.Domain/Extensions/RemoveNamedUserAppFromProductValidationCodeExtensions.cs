using System;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Extensions
{
    public static class RemoveNamedUserAppFromProductValidationCodeExtensions
    {
        public static RemoveNamedUserFromProductValidationCode ToRemoveNamedUserAppToProductValidationCode(this RemoveNamedUsersFromAppValidationCode validationCode)
        {
            switch (validationCode)
            {
                case RemoveNamedUsersFromAppValidationCode.NoError:
                    return RemoveNamedUserFromProductValidationCode.NoError;
                case RemoveNamedUsersFromAppValidationCode.NoLicensedTenant:
                    return RemoveNamedUserFromProductValidationCode.NoLicensedTenant;
                case RemoveNamedUsersFromAppValidationCode.NoLicensedApp:
                    return RemoveNamedUserFromProductValidationCode.NoProduct;
                case RemoveNamedUsersFromAppValidationCode.NoNamedUser:
                    return RemoveNamedUserFromProductValidationCode.NoNamedUser;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}