using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration
{
    public class TenantLegacyDatabaseMapping: ITenantLegacyDatabaseMapping, ISingletonDependency
    {
        private readonly Lazy<Dictionary<string, Guid>> _legacyDatabaseToTenantDictionary;
        private readonly List<Guid> _tenantIds;
        private readonly IConfiguration _configuration;

        public TenantLegacyDatabaseMapping(IConfiguration configuration)
        {
            _configuration = configuration;
            _legacyDatabaseToTenantDictionary = new Lazy<Dictionary<string, Guid>>(GetMappingConfigurationFromConfiguration, LazyThreadSafetyMode.ExecutionAndPublication);
            _tenantIds = new List<Guid>();
        }

        private Dictionary<string, Guid> GetMappingConfigurationFromConfiguration()
        {
            var databaseToTenantDictionary = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            var licenseConfigurationJson = _configuration[EnvironmentVariableConsts.TenantLegacyDatabaseMappingConfiguration];
            if (string.IsNullOrEmpty(licenseConfigurationJson)) 
                return databaseToTenantDictionary;
            
            var tenantDatabaseConfigurations = JsonConvert.DeserializeObject<List<TenantLegacyDatabaseMappingConfiguration>>(licenseConfigurationJson);
            SetTenantDatabaseConfiguration(databaseToTenantDictionary, tenantDatabaseConfigurations);
            SetTenantIds(tenantDatabaseConfigurations);

            return databaseToTenantDictionary;
        }

        private void SetTenantIds(IEnumerable<TenantLegacyDatabaseMappingConfiguration> tenantDatabaseConfigurations)
        {
            _tenantIds.AddRange(tenantDatabaseConfigurations.Select(s => s.TenantId));
        }

        private static void SetTenantDatabaseConfiguration(IDictionary<string, Guid> databaseToTenantDictionary, IEnumerable<TenantLegacyDatabaseMappingConfiguration> tenantDatabaseConfigurations)
        {
            foreach (var tenantDatabaseConfiguration in tenantDatabaseConfigurations.Where(s => s.LicensedDatabases != null))
                foreach (var licensedDatabase in tenantDatabaseConfiguration.LicensedDatabases)
                    databaseToTenantDictionary.Add(licensedDatabase, tenantDatabaseConfiguration.TenantId); 
        }

        public Guid GetTenantIdFromLegacyLicensedDatabase(string databaseName)
        {
            var databaseToTenantDictionary = _legacyDatabaseToTenantDictionary.Value;

            if (_tenantIds.Count == 1)
                return _tenantIds.First();

            return databaseToTenantDictionary.TryGetValue(databaseName, out var tenantId)
                ? tenantId
                : Guid.Empty; //throw new ArgumentException($"The database '{databaseName}' is not licensed to any Tenant.");//, nameof(databaseName));
        }
    }
}
