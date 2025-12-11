using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Viasoft.Core.ApiClient;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Data.Seeder.Abstractions;
using Viasoft.Licensing.CustomerLicensing.Domain.Consts;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Dashboard;
using Viasoft.Licensing.CustomerLicensing.Domain.HttpHeaderStrategy;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Seeder
{
    public class DashboardDataSeeder : ISeedData
    {
        private readonly IApiClientCallBuilder _serviceCallBuilder;
        private readonly ICurrentTenant _currentTenant;
        private readonly IServiceProvider _serviceProvider;

        public DashboardDataSeeder(IApiClientCallBuilder serviceCallBuilder, ICurrentTenant currentTenant, IServiceProvider serviceProvider)
        {
            _serviceCallBuilder = serviceCallBuilder;
            _currentTenant = currentTenant;
            _serviceProvider = serviceProvider;
        }

        public async Task SeedDataAsync()
        {
            if(_currentTenant.Id == Guid.Empty)
                return;
            
            var headerStrategy = new HttpHeaderStrategyWithNullEnvironment(_serviceProvider);
            
            var gatewayCall = _serviceCallBuilder.WithEndpoint(ExternalServicesConsts.Dashboard.Dashboards.DashboardVerification)
                .WithServiceName(ExternalServicesConsts.Dashboard.ServiceName)
                .WithBody(new DashboardDto
                {
                    ConsumerId = Guid.Parse(DashboardConsts.ConsumerId),
                    SerializedDashboard = Encoding.UTF8.GetBytes(DashboardConsts.DashboardJson)
                })
                .WithHttpMethod(HttpMethod.Post)
                .WithHttpHeaderStrategy(headerStrategy)
                .Build();

            await gatewayCall.ResponseCallAsync<HttpResponseMessage>();

        }
    }
}