using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Viasoft.Core.API.TenantManagement;
using Viasoft.Core.API.TenantManagement.Model;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.TenantDatabaseMapping;

public class TenantDatabaseMappingTenantManagementProvider : ITenantDatabaseMappingProvider
{
    private string SemaphoreSlimKey(Guid tenantId) => $"SemaphoreSlimKey_{tenantId}";
    private string TenantDatabasesMappingKey(Guid tenantId) => $"TenantDatabasesMappingKey_{tenantId}";
    
    private readonly IMemoryCache _memoryCache;
    private readonly ITenantManagementApi _tenantManagementApi;
    
    public TenantDatabaseMappingTenantManagementProvider(ITenantManagementApi tenantManagementApi, IMemoryCache memoryCache)
    {
        _tenantManagementApi = tenantManagementApi;
        _memoryCache = memoryCache;
    }
    
    public async Task<bool> IsTenantMapped(Guid tenantId)
    {
        EnsureTenantInSemaphores(tenantId);
        var semaphore = GetSemaphoreSlim(tenantId);
        await semaphore.WaitAsync();
        try
        {
            return GetTenantDatabaseMapping(tenantId) != null;
        }
        finally
        {
            semaphore.Release();
        }
    }
    
    public async Task<List<string>> GetTenantDatabases(Guid tenantId)
    {
        EnsureTenantInSemaphores(tenantId);
        var semaphore = GetSemaphoreSlim(tenantId);
        
        var cachedResult = GetTenantDatabaseMapping(tenantId);
        if (cachedResult != null)
            return cachedResult;
        
        await semaphore.WaitAsync();
        try
        {
            cachedResult = GetTenantDatabaseMapping(tenantId);
            if (cachedResult != null)
                return cachedResult;
            
            var result = await GetTenantDatabasesFromTenantId(tenantId);
            
            SetTenantDatabaseMapping(tenantId, result);
            
            return result;
        }
        finally
        {
            semaphore.Release();
        }
    }
    
    private SemaphoreSlim GetSemaphoreSlim(Guid tenantId)
    {
        var cacheObject = _memoryCache.Get(SemaphoreSlimKey(tenantId));
        return (SemaphoreSlim)cacheObject;
    }   
    
    private List<string> GetTenantDatabaseMapping(Guid tenantId)
    {
        var cacheObject = _memoryCache.Get(TenantDatabasesMappingKey(tenantId));
        return (List<string>)cacheObject;
    }
    
    private void SetTenantDatabaseMapping(Guid tenantId, List<string> databases)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };
        
        _memoryCache.Set(TenantDatabasesMappingKey(tenantId), databases, options);
    }
    
    private void EnsureTenantInSemaphores(Guid tenantId)
    {
        var semaphoreKey = SemaphoreSlimKey(tenantId);
        
        if (_memoryCache.Get(semaphoreKey) != null)
            return;
            
        var semaphore = new SemaphoreSlim(1, 1);
        
        var options = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove,
            Size = 1
        };
        
        _memoryCache.Set(semaphoreKey, semaphore, options);
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