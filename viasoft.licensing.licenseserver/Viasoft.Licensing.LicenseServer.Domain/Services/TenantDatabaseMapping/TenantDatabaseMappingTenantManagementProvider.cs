using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Viasoft.Core.API.TenantManagement;
using Viasoft.Core.API.TenantManagement.Model;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.TenantDatabaseMapping;

public class TenantDatabaseMappingTenantManagementProvider : ITenantDatabaseMappingProvider
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _semaphoreSlims;
    private readonly ConcurrentDictionary<Guid, List<string>> _tenantsDatabasesMapping;
    private readonly ITenantManagementApi _tenantManagementApi;

    public TenantDatabaseMappingTenantManagementProvider(ITenantManagementApi tenantManagementApi)
    {
        _tenantManagementApi = tenantManagementApi;
        _semaphoreSlims = new ConcurrentDictionary<Guid, SemaphoreSlim>();
        _tenantsDatabasesMapping = new ConcurrentDictionary<Guid, List<string>>();
    }

    public async Task<bool> IsTenantMapped(Guid tenantId)
    {
        EnsureTenantInSemaphores(tenantId);
        var licensingManagerSemaphore = _semaphoreSlims[tenantId];
        await licensingManagerSemaphore.WaitAsync();
        try
        {
            return _tenantsDatabasesMapping.ContainsKey(tenantId);
        }
        finally
        {
            licensingManagerSemaphore.Release();
        }
    }

    public async Task<List<string>> GetTenantDatabases(Guid tenantId)
    {
        EnsureTenantInSemaphores(tenantId);
        var licensingManagerSemaphore = _semaphoreSlims[tenantId];

        if (_tenantsDatabasesMapping.TryGetValue(tenantId, out var result))
            return result;
        
        await licensingManagerSemaphore.WaitAsync();
        try
        {
            if (_tenantsDatabasesMapping.TryGetValue(tenantId, out result))
                return result;
            
            result = await GetTenantDatabasesFromTenantId(tenantId);
            _tenantsDatabasesMapping.TryAdd(tenantId, result);
            return result;
        }
        finally
        {
            licensingManagerSemaphore.Release();
        }
    }

    private void EnsureTenantInSemaphores(Guid tenantId)
    {
        if (_semaphoreSlims.ContainsKey(tenantId))
            return;

        var semaphore = new SemaphoreSlim(1, 1);
        _semaphoreSlims.TryAdd(tenantId, semaphore);
    }

    private async Task<List<string>> GetTenantDatabasesFromTenantId(Guid tenantId)
    {

        var apiEnvironments = await _tenantManagementApi.GetEnvironmentsAsync(new TenantManagementApiGetEnvironments
        {
            MaxResultCount = 100,
            SkipCount = 0,
            TenantIds = new List<Guid>
            {
                tenantId
            }
        });

        var apiEnvironmentsResponse = await apiEnvironments.GetResponse();

        var result = new List<string>();
        foreach (var apiEnvironment in apiEnvironmentsResponse.Items
                     .Where(apiEnvironment => apiEnvironment.IsDesktop && apiEnvironment.IsActive &&
                                              !result.Contains(apiEnvironment.DatabaseName)))
        {
            result.Add(apiEnvironment.DatabaseName);
        }

        return result;
    }
}