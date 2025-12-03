using System;
using System.Collections.Generic;
using Viasoft.Core.ApiClient.HttpHeaderStrategy;

namespace Viasoft.Licensing.LicenseServer.Shared.Classes
{
    public class HostTenantIdHttpHeaderStrategy: IHttpHeaderStrategy
    {
        private readonly Guid _hostTenantId;

        public HostTenantIdHttpHeaderStrategy(Guid hostTenantId)
        {
            _hostTenantId = hostTenantId;
        }
        
        public Dictionary<string, string> GetHeaders()
        {
            return new Dictionary<string, string> { { "TenantId", _hostTenantId.ToString() } };
        }
    }
}