using System;
using LiteDB;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Shared.Initializer
{
    public static class LiteDbInitializer
    {
        public static LiteDatabase NewDatabase(ConnectionString connectionString)
        {
            var db = new LiteDatabase(connectionString);
            db.UtcDate = true;
            return db;
        }
        
        public static LiteRepository OldNewReadonlyRepository(Guid tenantId)
        {
            var connectionString = LiteDbConsts.BuildReadonlyTenantConnectionStringOld(tenantId);
            var db = new LiteRepository(connectionString);
            db.Database.UtcDate = true;
            return db;
        }

        public static LiteRepository NewReadonlyRepository(Guid tenantId)
        {
            var connectionString = LiteDbConsts.BuildReadonlyTenantConnectionString(tenantId);
            var db = new LiteRepository(connectionString);
            db.Database.UtcDate = true;
            return db;
        }
    }
}