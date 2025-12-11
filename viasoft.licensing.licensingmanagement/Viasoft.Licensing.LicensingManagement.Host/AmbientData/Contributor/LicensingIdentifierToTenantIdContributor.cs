using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Viasoft.Core.AmbientData;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant.Resolver.Contributors;

namespace Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor
{
    public class LicensingIdentifierToTenantIdContributor : ITenancyResolveContributor
    {
        private readonly IConfiguration _configuration;

        public LicensingIdentifierToTenantIdContributor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ValueTask<AmbientDataResolveResult> TryResolveTenant()
        {
            var hostTenantIdString = _configuration["HostTenantId"];
            if (string.IsNullOrEmpty(hostTenantIdString) || !Guid.TryParse(hostTenantIdString, out var hostTenantId))
            {
                return ValueTask.FromResult(AmbientDataResolveResult.NotHandled);
            }

            return ValueTask.FromResult(AmbientDataResolveResult<Guid>.Handle(hostTenantId));
        }
    }
}