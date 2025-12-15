using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Viasoft.Core.Caching.DistributedCache;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.InMemoryCache
{
    public class InMemoryCacheService: IDistributedCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCacheKeyStrategy _defaultKeyStrategy;

        public InMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _defaultKeyStrategy = new AnonymousFuncDistributedCacheKeyStrategy(s => s);
        }

        private static MemoryCacheEntryOptions ConvertFromDistributedCacheEntryOptions(DistributedCacheEntryOptions options)
        {
            if (options == null)
            {
                return new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(45));
            }
            var memoryOptions = new MemoryCacheEntryOptions();
            if (options.AbsoluteExpiration.HasValue)
            {
                memoryOptions.SetAbsoluteExpiration(options.AbsoluteExpiration.Value);
            }
            else
            {
                memoryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(45));
            }
            return memoryOptions;
        }
        
        private void InnerSetAsync(string key, byte[] value, IDistributedCacheKeyStrategy keyStrategy, DistributedCacheEntryOptions options = null)
        {
            var memoryOptions = ConvertFromDistributedCacheEntryOptions(options);
            key = keyStrategy.GetKey(key);
            _memoryCache.Set(key, value, memoryOptions);
        }
        
        public Task SetAsync(string key, object value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
            InnerSetAsync(key, bytes, _defaultKeyStrategy, options);
            return Task.CompletedTask;
        }

        public async Task SetAsync(string key, object value, CancellationToken token = default)
        {
            await SetAsync(key, value, null, token);
        }

        public Task SetAsync(string key, byte[] value, CancellationToken token = default)
        { 
            InnerSetAsync(key, value, _defaultKeyStrategy);
            return Task.CompletedTask;
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        { 
            InnerSetAsync(key, value, _defaultKeyStrategy, options);
            return Task.CompletedTask;
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, IDistributedCacheKeyStrategy keyStrategy,
            CancellationToken token = default)
        { 
            InnerSetAsync(key, value, keyStrategy, options);
            return Task.CompletedTask;
        }

        private Task<byte[]> InnerGetAsync(string key, IDistributedCacheKeyStrategy keyStrategy, CancellationToken token = default)
        {
            key = keyStrategy.GetKey(key);
            return Task.FromResult(_memoryCache.Get<byte[]>(key));
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken token = default)
        {
            var cache = await InnerGetAsync(key, _defaultKeyStrategy, token);
            if (cache == null)
                return default;
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(cache));
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> setFunc, CancellationToken token = default)
        {
            var cache = await InnerGetAsync(key, _defaultKeyStrategy, token);
            
            if (cache == null)
            {
                var cacheOfT = await setFunc();
                cache = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cacheOfT));
                InnerSetAsync(key, cache, _defaultKeyStrategy);
                cache = await InnerGetAsync(key, _defaultKeyStrategy, token);
            }

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(cache));
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            return await InnerGetAsync(key, _defaultKeyStrategy, token);
        }

        public async Task<byte[]> GetAsync(string key, IDistributedCacheKeyStrategy keyStrategy, CancellationToken token = default)
        {
            return await InnerGetAsync(key, keyStrategy, token);
        }

        public async Task<byte[]> GetOrSetAsync<T>(string key, Func<Task<byte[]>> setFunc, CancellationToken token = default)
        {
            var cacheKeyStrategy = _defaultKeyStrategy;
            var cache = await InnerGetAsync(key, cacheKeyStrategy, token);
            
            if (cache == null) 
            {
                cache = await setFunc();
                InnerSetAsync(key, cache, cacheKeyStrategy);
                cache = await InnerGetAsync(key, cacheKeyStrategy, token);
            }

            return cache;
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            await RefreshAsync(key, _defaultKeyStrategy, token);
        }

        public Task RefreshAsync(string key, IDistributedCacheKeyStrategy keyStrategy, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public async Task ReplaceAsync(string key, object value, CancellationToken token = default)
        {
            await ReplaceAsync(key, value, _defaultKeyStrategy, token);
        }

        public async Task ReplaceAsync(string key, object value, IDistributedCacheKeyStrategy keyStrategy,
            CancellationToken token = default)
        {
            await RemoveAsync(key, keyStrategy, token);
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
            InnerSetAsync(key, bytes, keyStrategy);
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            await RemoveAsync(key, _defaultKeyStrategy, token);
        }

        public Task RemoveAsync(string key, IDistributedCacheKeyStrategy keyStrategy, CancellationToken token = default)
        {
            key = keyStrategy.GetKey(key);
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}