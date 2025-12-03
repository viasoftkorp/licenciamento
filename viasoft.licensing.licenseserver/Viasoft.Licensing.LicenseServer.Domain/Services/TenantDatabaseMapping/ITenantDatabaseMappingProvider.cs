using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.TenantDatabaseMapping;

public interface ITenantDatabaseMappingProvider
{
    Task<bool> IsTenantMapped(Guid tenantId);
    Task<List<string>> GetTenantDatabases(Guid tenantId);
}