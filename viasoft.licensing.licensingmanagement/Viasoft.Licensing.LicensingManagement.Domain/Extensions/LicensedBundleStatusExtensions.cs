using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.Extensions
{
    public static class LicensedBundleStatusExtensions
    {
        public static ProductStatus ToProductStatus(this LicensedBundleStatus licensedBundleStatus)
        {
            switch (licensedBundleStatus)
            {
                case LicensedBundleStatus.BundleActive:
                    return ProductStatus.ProductActive;
                case LicensedBundleStatus.BundleBlocked:
                    return ProductStatus.ProductBlocked;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static LicensedAppStatus ToLicensedAppStatus(this LicensedBundleStatus licensedBundleStatus)
        {
            switch (licensedBundleStatus)
            {
                case LicensedBundleStatus.BundleActive:
                    return LicensedAppStatus.AppActive;
                case LicensedBundleStatus.BundleBlocked:
                    return LicensedAppStatus.AppBlocked;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}