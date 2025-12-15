using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.API.Administration.Extensions;
using Viasoft.Core.API.Authentication.Extensions;
using Viasoft.Core.API.EmailTemplate.Extensions;
using Viasoft.Core.API.LicensingManagement.Extensions;
using Viasoft.Core.API.Reporting.Extensions;
using Viasoft.Core.API.TenantManagement.Extensions;
using Viasoft.Core.API.UserProfile.Extensions;
using Viasoft.Core.ApiClient.Extensions;
using Viasoft.Core.AspNetCore.Extensions;
using Viasoft.Core.Authorization.AspNetCore.Extensions;
using Viasoft.Core.Authorization.Services;
using Viasoft.Core.Caching.DistributedCache;
using Viasoft.Core.DDD.Extensions;
using Viasoft.Core.Extensions;
using Viasoft.Core.Identity.AspNetCore.Extensions;
using Viasoft.Core.IoC.Extensions;
using Viasoft.Core.Mapper.Extensions;
using Viasoft.Core.MultiTenancy.AspNetCore.Extensions;
using Viasoft.Core.Service;
using Viasoft.Core.ServiceBus.AspNetCore.Extensions;
using Viasoft.Core.ServiceBus.SQLServer.Extensions;
using Viasoft.Core.ServiceDiscovery.Extensions;
using Viasoft.Core.ServiceDiscovery.Options;
using Viasoft.Licensing.LicenseServer.Domain.Classes;
using Viasoft.Licensing.LicenseServer.Domain.Extensions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Extensions;
using Viasoft.Licensing.LicenseServer.Domain.Old.InMemoryCache;
using Viasoft.Licensing.LicenseServer.Domain.Old.LegacyAcessTokenProvider;
using Viasoft.Licensing.LicenseServer.Domain.Repositories;
using Viasoft.Licensing.LicenseServer.Domain.Services.TenantDatabaseMapping;
using Viasoft.Licensing.LicenseServer.Host.HostedServices;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Host
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public static IServiceConfiguration ServiceConfiguration => new ServiceConfiguration
        {
            ServiceName = "Viasoft.Licensing.LicenseServer",
            Domain = "Licensing",
            App = "LicenseServer",
            AppIdentifier = "LS02"
        };
        
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private void ConfigureServicesLegacy(IServiceCollection serviceCollection)
        {
            if (_configuration.GetForceLegacyDatabaseEngine())
                serviceCollection.AddTransient<ILicenseServerRepository, LicenseServerRepositoryOnPremise>();
            else
                serviceCollection.AddTransient<ILicenseServerRepository, LicenseServerRepository>();

            serviceCollection
                .AddHostedService<ConsulProvider>()
                .AddApiClient(_configuration)
                .AddMultiTenancy(options =>
                {
                    options.MustUseCompanyPredicate = _ => false;
                    options.MustUseMultiTenancyPredicate = _ => false;
                    options.MustUseEnvironmentPredicate = _ => false;
                })
                .AddDomainDrivenDesign()
                .AddAutoMapper()
                .AddMemoryCache()
                .RegisterDependenciesByConvention()
                .AddAdministrationApi()
                .AddAuthenticationApi()
                .AddEmailTemplateApi()
                .AddLicensingManagementApi()
                .AddReportingApi()
                .AddTenantManagementApi()
                .AddUserProfileApi()
                .AddUserIdentity(options =>
                {
                    options.MustUseUserIdentityPredicate = _ => false;
                })
                .AddAuthorizations(_configuration)
                .AspNetCoreDefaultConfiguration(options =>
                {
                    options.UseNewSerializer = true;
                }, ServiceConfiguration, _configuration);
            
            serviceCollection.AddMemoryCache();
            
            var s = serviceCollection.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(IDistributedCacheService));
            if (s != null)
            {
                serviceCollection.Remove(s);
            }
            serviceCollection
                .AddTransient<IAccessTokenProvider, LegacyAcessTokenProvider>()
                .AddTransient<IDistributedCacheService, InMemoryCacheService>()
                .AddSingleton<ITenantDatabaseMappingProvider, TenantDatabaseMappingFileSettingsProvider>()
                .AddSwaggerGen(options => options.CustomSchemaIds(c => c.FullName));
        }

        private void ConfigureServicesWeb(IServiceCollection serviceCollection)
        {
            if (_configuration.GetForceLegacyDatabaseEngine())
                serviceCollection.AddTransient<ILicenseServerRepository, LicenseServerRepositoryOnPremise>();
            else
                serviceCollection.AddTransient<ILicenseServerRepository, LicenseServerRepository>();

            serviceCollection
                .AddServiceBus(options =>
                {
                    options.JsonOptions.UseNewSerializer = true;
                }, ServiceConfiguration, _configuration)
                .AddServiceBusSqlServerProvider()
                .AddServiceMesh()
                .AddApiClient(_configuration)
                .AddMultiTenancy(options =>
                {
                    options.MustUseCompanyPredicate = _ => false;
                    options.MustUseMultiTenancyPredicate = _ => false;
                    options.MustUseEnvironmentPredicate = _ => false;
                })
                .AddDomainDrivenDesign()
                .AddAutoMapper()
                .AddMemoryCache()
                .RegisterDependenciesByConvention()
                .AddAdministrationApi()
                .AddAuthenticationApi()
                .AddEmailTemplateApi()
                .AddLicensingManagementApi()
                .AddReportingApi()
                .AddTenantManagementApi()
                .AddUserProfileApi()
                .AddUserIdentity(options =>
                {
                    options.MustUseUserIdentityPredicate = _ => false;
                })
                .AddAuthorizations(_configuration)
                .AspNetCoreDefaultConfiguration(options =>
                {
                    options.UseNewSerializer = true;
                }, ServiceConfiguration, _configuration);

            serviceCollection
                .AddSingleton<ITenantDatabaseMappingProvider, TenantDatabaseMappingTenantManagementProvider>()
                .AddSwaggerGen(options => options.CustomSchemaIds(c => c.FullName));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (DefaultConfigurationConsts.IsRunningAsLegacy)
            {
                ConfigureServicesLegacy(services);
            }
            else
                ConfigureServicesWeb(services);

            services.Configure<ServiceDiscoveryOptions>(options => options.ServiceDiscoveryTagsFunc = tags =>
            {
                tags.Add("urlprefix-/licensing/license-server/");
            });
            
            services
                .AddHostedServices(typeof(Domain.Old.Classes.AppLicenseConsumer).Assembly)
                .AddHostedServices(typeof(AppLicenseConsumer).Assembly);
        }
        
        public void Configure(IApplicationBuilder app)
        {
            app.AspNetCoreDefaultAppConfiguration()
               .UseEndpoints();
        }
    }
}