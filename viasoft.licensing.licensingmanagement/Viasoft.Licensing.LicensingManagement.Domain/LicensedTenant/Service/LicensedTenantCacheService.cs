using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Viasoft.Core.Caching.DistributedCache;
using Viasoft.Core.IoC.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service
{
    public class LicensedTenantCacheService: ILicensedTenantCacheService, ITransientDependency
    {
        private readonly IDistributedCacheService _cacheService;
        private const string LicensedAppsIdentifiersFromTenantConst = "LicensedAppsIdentifiersFromTenant";

        public LicensedTenantCacheService(IDistributedCacheService cacheService)
        {
            _cacheService = cacheService;
        }
        
        public async Task InvalidateCacheForTenant(Guid identifier)
        {
            await InvalidateCacheForTenants(new List<Guid> {identifier});
        }
        
        public async Task InvalidateCacheForTenants(List<Guid> identifiers)
        {
            foreach (var identifier in identifiers)
            {
                await _cacheService.RemoveAsync(LicensedAppsIdentifiersFromTenantConst, new TenantDistributedCacheKeyStrategy(identifier));
            }
        }

        public async Task<List<string>> GetOrCreateCacheForLicensedAppsIdentifiersFromTenant(Guid tenantId, Func<Task<List<string>>> func)
        {
            var cache = await _cacheService.GetAsync(LicensedAppsIdentifiersFromTenantConst, new TenantDistributedCacheKeyStrategy(tenantId));
            
            if (cache == null)
            {
                var apps = await func();
                var cacheBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(apps));
                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)).SetSlidingExpiration(TimeSpan.FromHours(1));
                await _cacheService.SetAsync(LicensedAppsIdentifiersFromTenantConst, cacheBytes, options, new TenantDistributedCacheKeyStrategy(tenantId));
                cache = await _cacheService.GetAsync(LicensedAppsIdentifiersFromTenantConst, new TenantDistributedCacheKeyStrategy(tenantId));
            }

            return JsonConvert.DeserializeObject<List<string>>(Encoding.UTF8.GetString(cache));
        }
    }
}