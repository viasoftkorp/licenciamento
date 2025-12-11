using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.API.Administration.Extensions;
using Viasoft.Core.API.Authentication.Extensions;
using Viasoft.Core.API.Authorization.Extensions;
using Viasoft.Core.API.Emailing.Extensions;
using Viasoft.Core.API.EmailTemplate.Extensions;
using Viasoft.Core.API.LicensingManagement.Extensions;
using Viasoft.Core.API.Reporting.Extensions;
using Viasoft.Core.API.TenantManagement.Extensions;
using Viasoft.Core.API.UserProfile.Extensions;
using Viasoft.Core.ApiClient.Extensions;
using Viasoft.Core.AspNetCore.Extensions;
using Viasoft.Core.AspNetCore.Provisioning;
using Viasoft.Core.AspNetCore.UnitOfWork;
using Viasoft.Core.Authentication.Proxy.Extensions;
using Viasoft.Core.Authorization.AspNetCore.AspNetCore.ScopeBased;
using Viasoft.Core.Authorization.AspNetCore.Extensions;
using Viasoft.Core.Authorization.AspNetCore.Options;
using Viasoft.Core.Authorization.Proxy.Extensions;
using Viasoft.Core.Dashboard.Proxy.Extensions;
using Viasoft.Core.DDD.Extensions;
using Viasoft.Core.EntityFrameworkCore.Extensions;
using Viasoft.Core.EntityFrameworkCore.SQLServer.Extensions;
using Viasoft.Core.Identity.AspNetCore.Extensions;
using Viasoft.Core.Identity.Options;
using Viasoft.Core.IoC.Extensions;
using Viasoft.Core.Mapper.Extensions;
using Viasoft.Core.MultiTenancy.AspNetCore.Extensions;
using Viasoft.Core.MultiTenancy.Options;
using Viasoft.Core.Service;
using Viasoft.Core.ServiceBus.AspNetCore.Extensions;
using Viasoft.Core.ServiceBus.SQLServer.Extensions;
using Viasoft.Core.ServiceDiscovery.Extensions;
using Viasoft.Core.ServiceDiscovery.Options;
using Viasoft.Core.Storage.Extensions;
using Viasoft.Data.Seeder.Extensions;
using Viasoft.Licensing.CustomerLicensing.Domain.Seeder;
using Viasoft.Licensing.CustomerLicensing.Host.Controllers;
using Viasoft.Licensing.CustomerLicensing.Host.Extensions;
using Viasoft.Licensing.CustomerLicensing.Infrastructure.EntityFrameworkCore;

namespace Viasoft.Licensing.CustomerLicensing.Host
{
    public class Startup
    {
        private static readonly List<MethodInfo> LicenseServerAllowedMethods = new()
        {
            typeof(LicenseUsageInRealTimeImportController).GetMethod(nameof(LicenseUsageInRealTimeImportController.Import))
        };
        
        public static IServiceConfiguration ServiceConfiguration => new ServiceConfiguration
        {
            ServiceName = "Viasoft.Licensing.CustomerLicensing",
            Domain = "Licensing",
            App = "CustomerLicensing",
            AppIdentifier = "LS02"
        };
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAdministrationApi()
                .AddAuthenticationApi()
                .AddEmailTemplateApi()
                .AddLicensingManagementApi()
                .AddReportingApi()
                .AddTenantManagementApi()
                .AddUserProfileApi()
                .AddAuthorizationApi()
                .AddEmailingApi()
                .AddPersistence(_configuration)
                .AddEfCore<ViasoftCustomerLicensingDbContext>()
                .AddEfCoreSqlServer()
                .AddServiceBus(ServiceConfiguration, _configuration)
                .AddServiceBusSqlServerProvider()
                .AddServiceMesh()
                .AddApiClient(_configuration)
                .AddAuthorizationProxy()
                .AddDashboardProxy()
                .AddAuthenticationProxy()
                .AddNullEnvironmentAmbientDataCallOptionsResolver()
                .AddMultiTenancy(MultiTenancyOptions.Default().EnvironmentNotRequired().CompanyNotRequired())
                .AddDomainDrivenDesign()
                .AddAutoMapper()
                .RegisterDependenciesByConvention()
                .AddUserIdentity(UserIdentityOptions.Default().UserNotRequired())
                .AddAuthorizations(_configuration, options =>
                {
                    options.CustomOpenIdRequirementHandler = (context, requirement) => CustomOpenIdHandler(context, requirement, options);
                })
                .AddSeeders(new []
                {
                    typeof(DashboardDataSeeder)
                })
                .AspNetCoreDefaultConfiguration(ServiceConfiguration, _configuration);
            
            services.Configure<ServiceDiscoveryOptions>(options => options.ServiceDiscoveryTagsFunc = tags =>
            {
                tags.Add("urlprefix-/licensing/customer-licensing/");
            });
        }
        
        public void Configure(IApplicationBuilder app)
        {
            app.AspNetCoreDefaultAppConfiguration()
                .UseProvisioning()
                .UseUnitOfWork()
                .UseEndpoints();
        }

        private Task<bool> CustomOpenIdHandler(AuthorizationHandlerContext context, HasScopeRequirement requirement, AspNetCoreAuthorizationOptions options)
        {
            if (context.Resource is HttpContext httpContext)
            {
                var endpoint = httpContext.GetEndpoint();
                if (endpoint is not null)
                {
                    var controllerDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                    
                    if (LicenseServerAllowedMethods.Contains(controllerDescriptor?.MethodInfo))
                    {
                        var hasLicenseServerScope = context.User.Claims.Where(c => c.Type == "scope")
                            .Any(s => s.Value is "Viasoft.CustomerLicensing.LicenseServer");
                        
                        if (hasLicenseServerScope)
                        {
                            context.Succeed(requirement);
                            return Task.FromResult(false);
                        }
                        
                        context.Fail();
                        return Task.FromResult(false);
                    }
                }
            }
            return Task.FromResult(true);
        }
    }
}