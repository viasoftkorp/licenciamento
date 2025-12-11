using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
using Viasoft.Core.Authorization.AspNetCore.Extensions;
using Viasoft.Core.Authorization.Proxy.Extensions;
using Viasoft.Core.BackgroundJobs.Extensions;
using Viasoft.Core.BackgroundJobs.SQLServer.Extensions;
using Viasoft.Core.DDD.Extensions;
using Viasoft.Core.EntityFrameworkCore.Extensions;
using Viasoft.Core.EntityFrameworkCore.SQLServer.Extensions;
using Viasoft.Core.Identity.AspNetCore.Extensions;
using Viasoft.Core.Identity.Options;
using Viasoft.Core.IoC.Extensions;
using Viasoft.Core.Mapper.Extensions;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant.Resolver.Contributors;
using Viasoft.Core.MultiTenancy.AspNetCore.Extensions;
using Viasoft.Core.MultiTenancy.Options;
using Viasoft.Core.Service;
using Viasoft.Core.ServiceBus.AspNetCore.Extensions;
using Viasoft.Core.ServiceBus.SQLServer.Extensions;
using Viasoft.Core.ServiceDiscovery.Extensions;
using Viasoft.Core.ServiceDiscovery.Options;
using Viasoft.Core.Storage.Extensions;
using Viasoft.Data.Seeder.Extensions;
using Viasoft.Licensing.LicensingManagement.Domain.BackgroundJobs.ExpirationDateTimeCheckerJob;
using Viasoft.Licensing.LicensingManagement.Domain.Seeder;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;
using Viasoft.Licensing.LicensingManagement.Host.Controllers;
using Viasoft.Licensing.LicensingManagement.Host.Extensions;
using Viasoft.Licensing.LicensingManagement.Infrastructure.EntityFrameworkCore;
using Viasoft.PushNotifications.AspNetCore.Extensions;

namespace Viasoft.Licensing.LicensingManagement.Host
{
    public class Startup
    {
        private static readonly List<MethodInfo> LicenseServerEndpoints = new()
        {
            typeof(LicensingLicensesController).GetMethod(nameof(LicensingLicensesController.GetLicensingLicenses)),
            typeof(LicensingLicensesController).GetMethod(nameof(LicensingLicensesController.UpdateHardwareId))
        };

        private static readonly List<MethodInfo> UpdateNamedUserEndpoints = new()
        {
            typeof(LicensedBundleController).GetMethod(nameof(LicensedBundleController.UpdateNamedUsersFromBundle)),
            typeof(LicensedAppController).GetMethod(nameof(LicensedAppController.UpdateNamedUsersFromApp))
        };

        private static readonly List<string> FrontendClientIds = new()
        {
            "application-oauth",
            "portal",
            "VendasMobileCloud",
            "com.viasoft.approval"
        };
        
        private readonly IConfiguration _configuration;
        public static IServiceConfiguration ServiceConfiguration => new ServiceConfiguration
        {
            ServiceName = "Viasoft.Licensing.LicensingManagement",
            Domain = "Licensing",
            App = "LicensingManagement",
            AppIdentifier = "LS01"
        };

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //this MUST be BEFORE AddMultiTenancy
            services.AddTransient<ITenancyResolveContributor, LicensingIdentifierToTenantIdContributor>();

            services
                .AddAdministrationApi()
                .AddAuthenticationApi()
                .AddEmailTemplateApi()
                .AddLicensingManagementApi()
                .AddReportingApi()
                .AddTenantManagementApi()
                .AddUserProfileApi()
                .AddAuthorizationApi()
                .AddEmailingApi()
                .AddPersistence(_configuration)
                .AddEfCore<ViasoftLicenseServerDbContext>()
                .AddEfCoreSqlServer()
                .AddServiceBus(options =>
                {
                    options.SagasOptions.Enabled = true;
                    options.SagasOptions.EnforceExclusiveAccess = true;
                },ServiceConfiguration, _configuration)
                .AddServiceBusSqlServerProvider()
                .AddServiceMesh()
                .AddApiClient(_configuration)
                .AddNotification()
                .AddAuthorizations(_configuration, options =>
                {
                    options.CustomOpenIdRequirementHandler = (context, requirement) =>
                    {
                        if (context.Resource is HttpContext httpContext)
                        {
                            var endpoint = httpContext.GetEndpoint();
                            if (endpoint is not null)
                            {
                                var clientIdClaim = context.User.Claims.FirstOrDefault(claim => claim.Type == "client_id");
                                var scopes = context.User.Claims.Where(c => c.Type == "scope").ToList();

                                var hasLicenseServerScope = scopes.Any(s => s.Value is "Viasoft.LicensingManagement.LicenseServer");
                                var hasFrontendScope = scopes.Any(s => s.Value is "openid" or "introspection");
                                var containsFrontendClient = FrontendClientIds.Contains(clientIdClaim?.Value);
                                var isFrontendAllowed = options.AcceptsOpenIdScope && hasFrontendScope && containsFrontendClient;
                                
                                var controllerDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

                                if (UpdateNamedUserEndpoints.Contains(controllerDescriptor?.MethodInfo))
                                {
                                     context.SucceedOrFailInCondition(hasLicenseServerScope || isFrontendAllowed, requirement);
                                     return Task.FromResult(false);
                                }

                                if (LicenseServerEndpoints.Contains(controllerDescriptor?.MethodInfo))
                                {
                                    context.SucceedOrFailInCondition(hasLicenseServerScope, requirement);
                                    return Task.FromResult(false);
                                }
                                
                                context.SucceedOrFailInCondition(isFrontendAllowed, requirement);
                                return Task.FromResult(false);
                            }
                        }
                        return Task.FromResult(true);
                    };
                })
                .AddMultiTenancy(MultiTenancyOptions.Default().CompanyNotRequired().EnvironmentNotRequired())
                .AddDomainDrivenDesign()
                .AddAutoMapper()
                .AddDataSeeder<BackgroundJobsSeeder>()
                .AddDataSeeder<InfrastructureConfigurationSeeder>()
                .AddDataSeeder<LicensingStatusSeeder>()
                .AddDataSeeder<LicensedTenantSettingsSeeder>()
                .RegisterDependenciesByConvention()
                .AddNullEnvironmentAmbientDataCallOptionsResolver()
                .AddAuthorizationProxy()
                .AddAuthenticationProxy()
                .AddUserIdentity(UserIdentityOptions.Default())
                .EnableLegacyAutomaticSaveChanges()
                .AddBackgroundJobs(ServiceConfiguration, typeof(ExpirationDateTimeCheckerJob).Assembly)
                .AddSqlServerBackgroundJobs()
                .AspNetCoreDefaultConfiguration(ServiceConfiguration, _configuration);

            services.Configure<ServiceDiscoveryOptions>(options => options.ServiceDiscoveryTagsFunc = tags =>
            {
                tags.Add("urlprefix-/licensing/licensing-management/");
            });

        }

        public void Configure(IApplicationBuilder app)
        {
            app.AspNetCoreDefaultAppConfiguration()
                .UseProvisioning()
                .UseUnitOfWork()
                .UseEndpoints();
        }
        
    }
}
