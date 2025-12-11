using System;
using System.IO;
using LiteDB;

namespace Viasoft.Licensing.LicenseServer.Shared.Consts
{
    public static class LiteDbConsts
    {
        private static string DbPrefixOld => "LegacyLicensedTenant_";
        private static string DbPrefix => "V2_LicensedTenant_";
        public static string DefaultDirectory => "Db";
        private static string DbExtension => ".db";
        private static string Password => "B9CECDD1-05AB-448C-8D6B-BE87FB627A00";

        public static ConnectionString BuildTenantConnectionString(Guid tenantId)
        {
            var fileName = $"FileName={GetFileName(tenantId, DbPrefix)};";
            var password = $"Password={Password};";
            const string utcConfig = "Utc=true;";
            return new ConnectionString($"{fileName}{password}{utcConfig}");
        }
        
        public static ConnectionString BuildReadonlyTenantConnectionString(Guid tenantId)
        {
            var fileName = $"FileName={GetFileName(tenantId, DbPrefix)};";
            var password = $"Password={Password};";
            const string utcConfig = "Utc=true;";
            return new ConnectionString($"{fileName}{password}{utcConfig}") { ReadOnly = true };
        }
        
        public static ConnectionString BuildTenantConnectionStringOld(Guid tenantId)
        {
            var fileName = $"FileName={GetFileName(tenantId, DbPrefixOld)};";
            var password = $"Password={Password};";
            const string utcConfig = "Utc=true;";
            return new ConnectionString($"{fileName}{password}{utcConfig}");
        }
        
        public static ConnectionString BuildReadonlyTenantConnectionStringOld(Guid tenantId)
        {
            var fileName = $"FileName={GetFileName(tenantId, DbPrefixOld)};";
            var password = $"Password={Password};";
            const string utcConfig = "Utc=true;";
            return new ConnectionString($"{fileName}{password}{utcConfig}") { ReadOnly = true };
        }
        
        private static string GetFileName(Guid tenantId, string prefix)
        {
            Directory.CreateDirectory(DefaultDirectory);
            return Path.Combine(DefaultDirectory, $"{prefix}{tenantId.ToString().ToUpper()}{DbExtension}");
        }
    }
}