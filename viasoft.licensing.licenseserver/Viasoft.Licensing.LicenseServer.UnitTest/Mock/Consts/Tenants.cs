using System;
using System.Collections.Generic;
using System.Linq;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts
{
    public static class Tenants
    {
        public static class SimpleLicense
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("7DBEB581-B85D-4477-BBB1-D7A8BDFBD140");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
        }
        
        public static class SimpleLicenseWithConnectionLicenseType
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("88BEB581-B85D-4477-BBB1-D7A8BDFBD177");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
            public static LicenseConsumeType ConsumeType => LicenseConsumeType.Connection;
        }
        
        public static class SimpleLicenseWithAccessLicenseType
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("99BEB581-B85D-4477-BBB1-D7A8BDFBD177");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
            public static LicenseConsumeType ConsumeType => LicenseConsumeType.Access;
        }
        
        public static class SimpleLicenseWithinBundle
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("30D8BCC1-13EC-4CEE-9368-075146BA939B");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
        }

        public static class SimpleLicenseWithinBundleAccessType
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("12D8BCC1-13EC-4CEE-9368-075146BA939B");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
            
            public static LicenseConsumeType ConsumeType => LicenseConsumeType.Access;
        }

        public static class AdditionalLicense
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("DE191332-896E-41B8-B58C-53ADF3AB9887");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
        }
        
        public static class AdditionalLicenseAccessType
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("DF191332-896E-41B8-B58C-53ADF3AB9887");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
        }
        
        public static class BlockedLicense
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("856C95A0-1CA3-4199-9AB1-FF24988604AF");
            public static LicensingStatus Status => LicensingStatus.Blocked;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
        }
        
        public static class BlockedAppAdditionalLicense
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("ECFA1DD7-0066-46C4-B422-94D70D1242E0");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
        }
        
        public static class BlockedAppLicense
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("9EBD5608-0DB4-4B09-9C2D-9B137B8C0D84");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
        }
        
        public static class TwoTenantsConfigurationFromJson
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            private static List<string> _databases;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("53BD8971-1834-4C8F-827E-2B0560A18225");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
            public static IEnumerable<string> Databases => _databases ??= new List<string> { "TESTCOMPLETE_V16_0_0", "TESTE_CTE" };
            public static string TenantDatabaseConfiguration => "[{\"TenantId\":\""+Id+"\",\"LicensedDatabases\":["+Databases.Aggregate((a,b) => $"\"{a}\", \"{b}\"")+"]},{\"TenantId\":\"9EBD5608-0DB4-4B09-9C2D-9B137B8C0D84\",\"LicensedDatabases\":[\"TESTE_CTE_SEFAZ\",\"VIASOFT_CONNECT\"]}]";
        }
        
        public static class SingleTenantConfigurationFromJson
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("C5C35881-E802-42E6-A1F6-4C33B14BBFB2");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(1);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
            public static string TenantDatabaseConfiguration => "[{\"TenantId\":\""+Id+"\"}]";
        }
        
        public static class LicenseForDetails
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("53bd8971-1834-4c8f-827e-2b0560a18225");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => new DateTime(2040, 3, 5, 12, 5, 15, 5);
            public static List<string> Cnpjs => _tenantCnpjs ??= new List<string> { "94829839000180", "62134155000178" };
        }
        
        /*
        public static class ExpiredLicense
        {
            private static Guid _id;
            private static List<string> _tenantCnpjs;
            public static Guid Id => _id != Guid.Empty ? _id : _id = Guid.Parse("80165CAF-C9B9-4BDF-B370-F41EBE9DE7AD");
            public static LicensingStatus Status => LicensingStatus.Active;
            public static DateTime ExpirationDateTime => DateTime.UtcNow.AddDays(-1);
            public static List<string> Cnpjs => _tenantCnpjs ?? (_tenantCnpjs = new List<string> { "94829839000180", "62134155000178" });
        }*/
    }
}