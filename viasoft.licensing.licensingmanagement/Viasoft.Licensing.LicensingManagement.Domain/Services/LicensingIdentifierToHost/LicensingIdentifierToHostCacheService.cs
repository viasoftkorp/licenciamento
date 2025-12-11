using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Viasoft.Core.Caching.DistributedCache;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Core.MultiTenancy.DataFilter;
using Viasoft.Data.DataFilter;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.HostTenantId;
using Viasoft.Licensing.LicensingManagement.Domain.Services.HostTenant;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.LicensingIdentifierToHost
{
    public class LicensingIdentifierToHostCacheService: ILicensingIdentifierToHostCacheService, ITransientDependency
    {
        private const string LicensingIdentifierToHostKey = "LicensingIdentifierToHostTenant";
        private readonly IDistributedCacheService _distributedCacheService;
        private readonly IDataFilterManager _dataFilterManager;
        private readonly IHostTenantService _hostTenantService;

        public LicensingIdentifierToHostCacheService(IDistributedCacheService distributedCacheService, IDataFilterManager dataFilterManager,
            IHostTenantService hostTenantService)
        {
            _distributedCacheService = distributedCacheService;
            _dataFilterManager = dataFilterManager;
            _hostTenantService = hostTenantService;
        }

        private async Task<HostTenantIdOutput> DoGetHostTenantId(Guid licensingIdentifier, TenantIdParameterKind kind)
        {
            using (_dataFilterManager.DisableDataFilter<MustHaveTenantDataFilter>())
            {
                switch (kind)
                {
                    case TenantIdParameterKind.LicensingIdentifier:
                        return await _hostTenantService.GetHostTenantIdFromLicensingIdentifier(licensingIdentifier);
                    case TenantIdParameterKind.LicensedTenantId:
                        return await _hostTenantService.GetHostTenantIdFromLicensedTenantId(licensingIdentifier);
                    default:
                        return null;
                }
            }
        }

        public async Task<HostTenantIdOutput> GetHostTenantIdFromLicensingIdentifier(Guid licensingIdentifier, TenantIdParameterKind kind)
        {
            var cache = await _distributedCacheService.GetAsync(LicensingIdentifierToHostKey, new TenantDistributedCacheKeyStrategy(licensingIdentifier));
            if (cache != null)
            {
                return JsonConvert.DeserializeObject<HostTenantIdOutput>(Encoding.UTF8.GetString(cache));
            }

            var hostTenant = await DoGetHostTenantId(licensingIdentifier, kind);
            if (hostTenant != null)
            {
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(hostTenant));
                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
            
                await _distributedCacheService.SetAsync(LicensingIdentifierToHostKey, bytes, options, new TenantDistributedCacheKeyStrategy(licensingIdentifier));
            }
            return hostTenant;
        }
    }
}