using System;

namespace Viasoft.Licensing.LicenseServer.Shared.Consts
{
    public static class ExternalServiceConsts
    {
        public static class LicensingManagement 
        {
            public static string ServiceName => "Viasoft.Licensing.LicensingManagement";
            private static string Domain => "Licensing";
            private static string App => "LicensingManagement";
            public static string Area => $"{Domain}{App}";

            private static string ServicePath => $"{Domain}/{App}/";

            public static class LicenseServer
            {
                private static string Path => $"{ServicePath}LicenseServer/";
                public static string GetLicenseByTenantId => $"{Path}GetLicenseByTenantId?tenantId=";
            }
            
            public static class Licenses
            {
                private static string Path => "licensing/licensing-management/licenses/";
                
                public static string GetLicensingLicenses(Guid identifier) => $"{Path}{identifier}";

                public static string UpdateHardwareId(Guid identifier) => $"{Path}{identifier}/HardwareId";
            }
            
            public static class LicensedApp
            {
                private static string Path => "licensing/licensing-management/licenses/";

                public static string UpdateNamedUserAppLicense(Guid licensedTenant, Guid licensedApp,
                    Guid namedUserAppLicenseId) => $"{Path}{licensedTenant}/licensed-apps/{licensedApp}/named-user/{namedUserAppLicenseId}";
            }
            
            public static class LicensedBundle
            {
                private static string Path => "licensing/licensing-management/licenses/";

                public static string UpdateNamedUserBundleLicense(Guid licensedTenant, Guid licensedBundle,
                    Guid namedUserbundleLicenseId) => $"{Path}{licensedTenant}/licensed-bundles/{licensedBundle}/named-user/{namedUserbundleLicenseId}";
            }
        }

        public static class CustomerLicensing
        {
            public static string ServiceName => "Viasoft.Licensing.CustomerLicensing";
            private static string Domain => "Licensing";
            private static string App => "CustomerLicensing";
            public static string Area => $"{Domain}{App}";

            private static string ServicePath => $"{Domain}/{App}/";
            
            public static class LicenseUsageInRealTime
            {
                private static string Path => $"{ServicePath}LicenseUsageInRealTime/";

                public static string Import => $"{Path}import";
            }
            
            public static class LicenseUsageInRealTimeImport
            {
                private static string Path => "/licensing/customer-licensing/";
                public static string RealTime => $"{Path}real-time";
            }
        }
        
    }
}