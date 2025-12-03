using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.TenantDatabaseMapping;

public class TenantDatabaseMappingFileSettingsProvider : ITenantDatabaseMappingProvider
{
    private readonly LicenseServerSettings _settings;

    public TenantDatabaseMappingFileSettingsProvider()
    {
        _settings = LicenseServerSettingsExtension.LoadSettings();
    }

    public Task<bool> IsTenantMapped(Guid tenantId)
    {
        return Task.FromResult(_settings is not null &&
                               _settings.TenantLegacyDatabaseMapping
                                   .Any(configuration => configuration.TenantId == tenantId));
    }

    public Task<List<string>> GetTenantDatabases(Guid tenantId)
    {
        var tenantDatabaseMapping = _settings.TenantLegacyDatabaseMapping
            .FirstOrDefault(d => d.TenantId == tenantId);

        var result = tenantDatabaseMapping is not null ? tenantDatabaseMapping.LicensedDatabases : new List<string>();

        return Task.FromResult(result);
    }
}