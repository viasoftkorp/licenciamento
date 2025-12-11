using System;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Extensions
{
    public static class AddNamedUserBundleToProductValidationCodeExtensions
    {
         public static AddNamedUserToProductValidationCode ToAddNamedUserBundleToProductValidationCode(this OperationValidation operationValidation)
         {
             switch (operationValidation)
             {
                 case OperationValidation.NoError:
                     return AddNamedUserToProductValidationCode.NoError;
                 case OperationValidation.NoTenantWithSuchId:
                     return AddNamedUserToProductValidationCode.NoLicensedTenant;
                 case OperationValidation.NoLicensedBundleWithSuchId:
                     return AddNamedUserToProductValidationCode.NoProduct;
                 case OperationValidation.TooManyNamedUserBundleLicenses:
                     return AddNamedUserToProductValidationCode.TooManyNamedUsers;
                 case OperationValidation.NotANamedLicense:
                     return AddNamedUserToProductValidationCode.ProductIsNotNamed;
                 case OperationValidation.NamedUserEmailAlreadyInUse:
                     return AddNamedUserToProductValidationCode.NamedUserEmailAlreadyInUse;
                 default:
                     throw new ArgumentOutOfRangeException();
             }
         }
    }
}