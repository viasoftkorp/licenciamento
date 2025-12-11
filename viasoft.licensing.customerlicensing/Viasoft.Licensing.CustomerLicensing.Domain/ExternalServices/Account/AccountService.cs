using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Core.AmbientData.Consts;
using Viasoft.Core.ApiClient;
using Viasoft.Core.ApiClient.HttpHeaderStrategy;
using Viasoft.Core.AspNetCore.AmbientData;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant.Store;
using Viasoft.Licensing.CustomerLicensing.Domain.Consts;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Account;

namespace Viasoft.Licensing.CustomerLicensing.Domain.ExternalServices.Account
{
    public class AccountService: IAccountService, ITransientDependency
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITenancyStore _tenancyStore;

        public AccountService(IApiClientCallBuilder apiClientCallBuilder, IServiceProvider serviceProvider, ITenancyStore tenancyStore)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
            _serviceProvider = serviceProvider;
            _tenancyStore = tenancyStore;
        }

        private IHttpHeaderStrategy GetCustomHttpHeaderStrategy(Guid hostTenantId)
        {
            var strategy = new HostTenantCustomHttpHeaderStrategy(hostTenantId, _serviceProvider);
            return strategy;
        }

        public async Task<AccountInfoOutput> GetAccountNameFromLicensingIdentifier(Guid licensingIdentifier)
        {
            var hostTenantIdOutput = await _tenancyStore.GetHostTenantAsync(licensingIdentifier);
            var endpoint = ExternalServicesConsts.LicensingManagement.Account.GetAccountInfoFromLicensingIdentifier(licensingIdentifier);
            
            var gatewayCall = _apiClientCallBuilder
                .WithEndpoint(endpoint)
                .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                .WithHttpHeaderStrategy(GetCustomHttpHeaderStrategy(hostTenantIdOutput.HostTenantId))
                .Build();

            return await gatewayCall.ResponseCallAsync<AccountInfoOutput>();
        }

        private class HostTenantCustomHttpHeaderStrategy : IHttpHeaderStrategy
        {
            private readonly Guid _hostTenantId;
            private readonly IHttpHeaderStrategy _headerStrategy;

            public HostTenantCustomHttpHeaderStrategy(Guid hostTenantId, IServiceProvider serviceProvider)
            {
                _hostTenantId = hostTenantId;
                _headerStrategy = new AmbientDataCallOptionsHttpHeaderStrategy(ActivatorUtilities.CreateInstance<AmbientDataCallOptionsResolver>(serviceProvider));
            }
            public Dictionary<string, string> GetHeaders()
            {
                var defaultHeaders = _headerStrategy.GetHeaders();
                if (defaultHeaders.ContainsKey(AmbientDataConsts.TenantId))
                {
                    defaultHeaders.Remove(AmbientDataConsts.TenantId);
                }
                defaultHeaders.Add(AmbientDataConsts.TenantId, _hostTenantId.ToString());
                return defaultHeaders;
            }
        }
    }
}