using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Core.ApiClient.HttpHeaderStrategy;
using Viasoft.Core.AspNetCore.AmbientData;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.HeaderStrategies
{
    public class TenantManagementCallerHeaderStrategy : IHttpHeaderStrategy
    {
        private readonly Guid _tenantId;
        private readonly IAmbientDataCallOptionsResolver _callOptionsResolver;

        public TenantManagementCallerHeaderStrategy(Guid tenantId, IServiceProvider serviceProvider)
        {
            _tenantId = tenantId;
            // We want to forcefully inject AmbientDataCallOptionsResolver instead of NullEnvironmentAmbientDataCallOptionsResolver
            _callOptionsResolver = ActivatorUtilities.CreateInstance<AmbientDataCallOptionsResolver>(serviceProvider);
        }

        public Dictionary<string, string> GetHeaders()
        {
            var defaultStrategy = new AmbientDataCallOptionsHttpHeaderStrategy(_callOptionsResolver);
            var headers = defaultStrategy.GetHeaders();
            headers["TenantId"] = _tenantId.ToString();
            return headers;
        }
    }
}