using System;

namespace Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration
{
    public interface ITenantLegacyDatabaseMapping
    {
        Guid GetTenantIdFromLegacyLicensedDatabase(string databaseName);
    }
}