using System;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Extensions
{
    public static class RemoveNamedUserBundleFromProductValidationCodeExtensions
    {
         public static RemoveNamedUserFromProductValidationCode ToRemoveNamedUserBundleToProductValidationCode(this RemoveNamedUserFromBundleValidationCode validationCode)
         {
             switch (validationCode)
             {
                 case RemoveNamedUserFromBundleValidationCode.NoError:
                     return RemoveNamedUserFromProductValidationCode.NoError;
                 case RemoveNamedUserFromBundleValidationCode.NoLicensedTenant:
                     return RemoveNamedUserFromProductValidationCode.NoLicensedTenant;
                 case RemoveNamedUserFromBundleValidationCode.NoLicensedBundle:
                     return RemoveNamedUserFromProductValidationCode.NoProduct;
                 case RemoveNamedUserFromBundleValidationCode.NoNamedUser:
                     return RemoveNamedUserFromProductValidationCode.NoNamedUser;
                 default:
                     throw new ArgumentOutOfRangeException();
             }
         }
    }
}