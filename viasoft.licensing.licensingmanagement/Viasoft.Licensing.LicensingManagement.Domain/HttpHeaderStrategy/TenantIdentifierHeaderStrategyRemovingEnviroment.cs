using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Core.ApiClient.HttpHeaderStrategy;
using Viasoft.Core.AspNetCore.AmbientData;

namespace Viasoft.Licensing.LicensingManagement.Domain.HttpHeaderStrategy
{
    public class TenantIdentifierHeaderStrategyRemovingEnviroment: IHttpHeaderStrategy
    {
        private readonly string _tenantIdentifier;
        private readonly IAmbientDataCallOptionsResolver _callOptionsResolver;

        public TenantIdentifierHeaderStrategyRemovingEnviroment(string tenantIdentifier, IServiceProvider serviceProvider)
        {
            _tenantIdentifier = tenantIdentifier;
            // We want to forcefully inject AmbientDataCallOptionsResolver instead of NullEnvironmentAmbientDataCallOptionsResolver
            _callOptionsResolver = ActivatorUtilities.CreateInstance<AmbientDataCallOptionsResolver>(serviceProvider);
        }

        public Dictionary<string, string> GetHeaders()
        {
            var defaultHeaderStrategy = new AmbientDataCallOptionsHttpHeaderStrategy(_callOptionsResolver);
            var headers = defaultHeaderStrategy.GetHeaders();

            headers["TenantId"] = _tenantIdentifier;
            headers.Remove("EnvironmentId");

            return headers;
        }
    }
}