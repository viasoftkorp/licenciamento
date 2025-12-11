﻿using System;
 using Viasoft.Core.ApiClient.Extensions;
 using Viasoft.Core.DDD.Application.Dto.Paged;
 using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle;
 using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Product;

 namespace Viasoft.Licensing.CustomerLicensing.Domain.Consts
{
    public static class ExternalServicesConsts
    {
        public static class Dashboard
        {
            private static string App => nameof(Dashboard) + "/";
            public static string ServiceName => "Viasoft.Dashboard";
            public static string Area => $"{App}";

            public static class Dashboards
            {
                public static string Path => $"{Area}{nameof(Dashboards)}/";
                public static string DashboardVerification => $"{Path}{nameof(DashboardVerification)}";
            }
        }
        
        public static class LicensingManagement
        {
            private static string Domain => "Licensing/";
            private static string App => "LicensingManagement/";
            public static string ServiceName => "Viasoft.Licensing.LicensingManagement";
            private static string Area => $"{Domain}{App}";
            public static class Account
            {
                private static string Path => $"{Area}{nameof(Account)}/";

                public static string GetAccountInfoFromLicensingIdentifier(Guid licensingIdentifier)
                {
                    return $"{Path}{nameof(GetAccountInfoFromLicensingIdentifier)}?licensingIdentifier={licensingIdentifier}";
                }
            }
            public static class HostTenant
            {
                private static string Path => $"{Area}{nameof(HostTenant)}/";

                public static string GetHostTenantIdFromLicensingIdentifier(Guid licensingIdentifier)
                {
                    return $"{Path}{nameof(GetHostTenantIdFromLicensingIdentifier)}?licensingIdentifier={licensingIdentifier}";
                }
            }
            
            public static class LicensingManagementStatistics
            {
                private static string Path => $"{Area}{nameof(LicensingManagementStatistics)}/";

                public static string GetNumberOfAppsInTotal => $"{Path}{nameof(GetNumberOfAppsInTotal)}";
            }
            
            public static class TenantInfo
            {
                private static string Path => $"{Area}{nameof(TenantInfo)}/";

                public static string GetTenantInfoFromLicensingIdentifier(Guid identifier)
                {
                    return $"{Path}{nameof(GetTenantInfoFromLicensingIdentifier)}?identifier={identifier}";
                }
            }
            
            public static class DomainController
            {
                private static string Path => $"{Area}Domain/";

                public static string GetDomainsFromAppIds => $"{Path}{nameof(GetDomainsFromAppIds)}";
            }

            public static class LicensedBundle
            {
                private static string Path => $"{Area}Bundle/";

                public static string GetAllProducts(Guid licensedTenantId, GetAllProductsInput input)
                {
                    return $"/licensing/licensing-management/licenses/{licensedTenantId}/products?{input.ToHttpGetQueryParameter()}";
                }

                public static string GetNamedUserFromBundle(Guid licensedTenant, Guid licensedBundle,
                    PagedFilteredAndSortedRequestInput filter)
                {
                    return $"/licensing/licensing-management/licenses/{licensedTenant}/licensed-bundles/{licensedBundle}/named-user?{filter.ToHttpGetQueryParameter()}";
                }
                
                public static string GetNamedUserFromApp(Guid licensedTenant, Guid licensedApp,
                    PagedFilteredAndSortedRequestInput filter)
                {
                    return $"/licensing/licensing-management/licenses/{licensedTenant}/licensed-apps/{licensedApp}/named-user?{filter.ToHttpGetQueryParameter()}";
                }

                public static string GetProductById(Guid licensedTenantId, Guid productId, GetProductByIdInput input)
                {
                    return $"/licensing/licensing-management/licenses/{licensedTenantId}/products/{productId}?{input.ToHttpGetQueryParameter()}";
                }

                public static string GetAllUsers(Guid licensingIdentifier, GetAllUsersInput input)
                {
                    return $"/licensing/licensing-management/licenses/{licensingIdentifier}/users?{input.ToHttpGetQueryParameter()}";
                }

                public static string RemoveNamedUserFromBundle(Guid licensedTenant, Guid licensedBundle, Guid namedUserBundleId)
                {
                    return $"/licensing/licensing-management/licenses/{licensedTenant}/licensed-bundles/{licensedBundle}/named-user/{namedUserBundleId}";
                }

                public static string RemoveNamedUserFromApp(Guid licensedTenant, Guid licensedApp, Guid namedUserBundleId)
                {
                    return $"/licensing/licensing-management/licenses/{licensedTenant}/licensed-apps/{licensedApp}/named-user/{namedUserBundleId}";
                }
                public static string AddNamedUserToBundle(Guid licensedTenantId, Guid licensedBundleId)
                {
                    return $"/licensing/licensing-management/licenses/{licensedTenantId}/licensed-bundles/{licensedBundleId}/named-user";
                }
                public static string AddNamedUserToApp(Guid licensedTenantId, Guid licensedAppId)
                {
                    return $"/licensing/licensing-management/licenses/{licensedTenantId}/licensed-apps/{licensedAppId}/named-user";
                }
                public static string UpdateNamedUserFromBundle(Guid licensedTenantId, Guid licensedBundleId, Guid namedUserId)
                {
                    return $"/licensing/licensing-management/licenses/{licensedTenantId}/licensed-bundles/{licensedBundleId}/named-user/{namedUserId}";
                }
                public static string UpdateNamedUserFromApp(Guid licensedTenantId, Guid licensedAppId, Guid namedUserId)
                {
                    return $"/licensing/licensing-management/licenses/{licensedTenantId}/licensed-apps/{licensedAppId}/named-user/{namedUserId}";
                }
            }
        }
    }
}