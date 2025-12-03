using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration
{
    public class TenantLegacyDatabaseMappingConfiguration
    {
        public Guid TenantId { get; set; }
        public List<string> LicensedDatabases { get; set; }
    }
}