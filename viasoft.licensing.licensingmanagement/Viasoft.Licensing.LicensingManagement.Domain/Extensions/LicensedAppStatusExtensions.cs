using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.Extensions
{
    public static class LicensedAppStatusExtensions
    {
        public static ProductStatus ToProductStatus(this LicensedAppStatus licensedAppStatus)
        {
            switch (licensedAppStatus)
            {
                case LicensedAppStatus.AppActive:
                    return ProductStatus.ProductActive;
                case LicensedAppStatus.AppBlocked:
                    return ProductStatus.ProductBlocked;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}