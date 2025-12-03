using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Viasoft.Core.IoC.Extensions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.UserBehaviour;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Messages;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicensedTenantOrchestrator;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.TenantLicensing;
using Viasoft.Licensing.LicenseServer.Host.ControllersOld;
using Viasoft.Licensing.LicenseServer.Host.Mapper;
using Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.UnitTest.Extensions;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts;

namespace Viasoft.Licensing.LicenseServer.UnitTest.LicensedTenant
{
    public class LicensedTenantBase
    {
        protected IServiceProvider ServiceProvider;
        private ILicensedTenantOrchestratorService _licensedTenantOrchestratorService;
        private ITenantLegacyDatabaseMapping _tenantLegacyDatabaseMapping;
        private ILicenseUsageService _licenseUsageService;
        private ITenantLicensingService _tenantLicensingService;
        private LicenseServerController _licenseServerController;
        private LicenseServerLegacyController _licenseServerLegacyController;
        
        public void Setup()
        {
            var source = new MemoryConfigurationSource
            {
                InitialData = new List<KeyValuePair<string, string>>
                {
                    new("Authentication:Enabled", "false"),
                    new("Authentication:Authority", string.Empty),
                    new("LoggingLevel", "Warning"),
                    new("ASPNETCORE_ENVIRONMENT", "Development"),
                    new("LICENSE_USAGE_IN_REAL_TIME_UPLOAD_FREQUENCY_IN_MINUTES", DefaultConfigurationConsts.LicenseUsageInRealTimeUploadFrequencyInMinutes.ToString()),
                    new("LICENSE_USAGE_BEHAVIOUR_UPLOAD_FREQUENCY_IN_DAYS", DefaultConfigurationConsts.LicenseUsageBehaviourUploadFrequencyInDays.ToString()),
                    new("MINIMUM_ALLOWED_HEARTBEAT_IN_SECONDS", DefaultConfigurationConsts.MinimumAllowedHeartbeatInSeconds.ToString()),
                    new("TENANT_LEGACY_DATABASE_MAPPING_CONFIGURATION", Tenants.TwoTenantsConfigurationFromJson.TenantDatabaseConfiguration),
                }
            };

            var configuration = new ConfigurationBuilder()
                .Add(source)
                .Build();
            
            var mockLogger = new Mock<ILogger<LicenseServerLegacyController>>();
            mockLogger.Setup(logger => logger.Log(LogLevel.Warning, 0, It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            
            var mapper = new MapperConfiguration(config => config.AddProfile(typeof(LicenseServerMapperProfile)))
                .CreateMapper();
            
            ServiceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(_ => configuration)
                .RegisterDependenciesByConvention()
                .AddTransient(typeof(ILogger<>), typeof(NullLogger<>))
                .ReplaceDependenciesWithMockImplementation(typeof(LicensedTenantBase).Assembly)
                .AddSingleton(mapper)
                .AddSingleton(mockLogger.Object)
                .BuildServiceProvider();
            
            _licensedTenantOrchestratorService = ServiceProvider.GetService<ILicensedTenantOrchestratorService>();
            _tenantLegacyDatabaseMapping = ServiceProvider.GetService<ITenantLegacyDatabaseMapping>();
            _licenseUsageService = ServiceProvider.GetService<ILicenseUsageService>();
            _tenantLicensingService = ServiceProvider.GetService<ITenantLicensingService>();
            _licenseServerController = ActivatorUtilities.CreateInstance<LicenseServerController>(ServiceProvider);
            _licenseServerLegacyController = ActivatorUtilities.CreateInstance<LicenseServerLegacyController>(ServiceProvider);
        }

        public void TearDown()
        {
            if (Directory.Exists(LiteDbConsts.DefaultDirectory))
                Directory.Delete(LiteDbConsts.DefaultDirectory, true);
        }

        protected async Task<int> GetAvailableLicense(Guid tenantId, string appIdentifier)
        {
            var currentLicenseStatus = await GetTenantCurrentLicenseStatus(tenantId);
            var availableLicense = currentLicenseStatus.GetAvailableLicense(appIdentifier);
            return availableLicense;
        }
        
        protected async Task<int> GetAvailableAdditionalLicense(Guid tenantId, string appIdentifier)
        {
            var currentLicenseStatus = await GetTenantCurrentLicenseStatus(tenantId);
            var availableAdditionalLicense = currentLicenseStatus.GetAvailableAdditionalLicense(appIdentifier);
            return availableAdditionalLicense;
        }

        protected Task<TenantLicensedAppsOutput> GetTenantLicensedApps(Guid tenantId)
        {
            return _licensedTenantOrchestratorService.GetTenantLicensedApps(tenantId);
        }

        protected Task<TenantLicenseStatusOutput> GetTenantLicenseStatus(Guid tenantId)
        {
            return _licensedTenantOrchestratorService.GetTenantLicenseStatus(tenantId);
        }

        protected async Task ReleaseLicenseBasedOnHeartbeat()
        {
            await _licensedTenantOrchestratorService.ReleaseLicenseBasedOnHeartbeat();  
        }

        protected Task<IsTenantCnpjLicensedOutput> IsTenantCnpjLicensed(Guid tenantId, string cnpj)
        {
            return _licensedTenantOrchestratorService.IsTenantCnpjLicensed(tenantId, cnpj);
        }

        protected Task<ConsumeLicenseOutputOld> ConsumeLicense(Guid tenantId, string appIdentifier, string user, string cnpj, string customAppName = null, LicenseUsageAdditionalInformationOld licenseUsageAdditionalInformationOld = null)
        {
            var consumeLicenseInput = new ConsumeLicenseInput
            {
                TenantId = tenantId,
                AppIdentifier = appIdentifier,
                User = user,
                Cnpj = cnpj,
                CustomAppName = customAppName,
                LicenseUsageAdditionalInformation = licenseUsageAdditionalInformationOld
            };
           return _licensedTenantOrchestratorService.ConsumeLicense(consumeLicenseInput); 
        }
        
        protected Task<ReleaseLicenseOutputOld> ReleaseLicense(Guid tenantId, string appIdentifier, string user, string cnpj)
        {
            var releaseLicenseInput = new ReleaseLicenseInput
            {
                TenantId = tenantId,
                AppIdentifier = appIdentifier,
                User = user,
                Cnpj = cnpj
            };
            return _licensedTenantOrchestratorService.ReleaseLicense(releaseLicenseInput);    
        }

        protected Task<RefreshAppLicenseInUseByUserOutputOld> RefreshAppLicenseInUseByUser(Guid tenantId, string appIdentifier, string user, string cnpj)
        {
            var refreshAppLicenseInUseByUserInput = new RefreshAppLicenseInUseByUserInputOld
            {
                TenantId = tenantId,
                AppIdentifier = appIdentifier,
                User = user,
                Cnpj = cnpj
            };
            return _licensedTenantOrchestratorService.RefreshAppLicenseInUseByUser(refreshAppLicenseInUseByUserInput);  
        }

        protected Task<List<LicenseUsageBehaviourDetails>> GetLicensesUsage(Guid tenantId)
        {
            return _licenseUsageService.GetLicensesUsage(tenantId);
        }
        
        protected IEnumerable<LicenseUsageInRealTime> GetLicenseUsageInRealTime()
        {
            return _licensedTenantOrchestratorService.GetTenantsLicensesUsageInRealTime();
        }
        
        protected Task<ConsumeLicenseOutputOld> ConsumeLicenseLegacy(string databaseName, string appIdentifier, string user, string cnpj)
        {
            var tenantId = GetTenantIdFromLicensedDatabase(databaseName);
            return ConsumeLicense(tenantId, appIdentifier, user, cnpj); 
        }
        
        protected Task<ReleaseLicenseOutputOld> ReleaseLicenseLegacy(string databaseName, string appIdentifier, string user, string cnpj)
        {
            var tenantId = GetTenantIdFromLicensedDatabase(databaseName);
            return ReleaseLicense(tenantId, appIdentifier, user, cnpj);    
        }

        protected Task<LicenseTenantStatusCurrentOld> GetTenantCurrentLicenseStatus(Guid tenantId)
        {
           return _licensedTenantOrchestratorService.GetTenantCurrentLicenseStatus(tenantId);
        }

        private Guid GetTenantIdFromLicensedDatabase(string databaseName)
        {
            return _tenantLegacyDatabaseMapping.GetTenantIdFromLegacyLicensedDatabase(databaseName);
        }

        protected Task RefreshTenantLicensing(LicensingDetailsUpdated licensingDetailsUpdated)
        {
            return _licensedTenantOrchestratorService.RefreshTenantLicensing(licensingDetailsUpdated);  
        }

        protected Task RefreshAllTenantsLicensing()
        {
            return _licensedTenantOrchestratorService.RefreshAllTenantsLicensing();
        }

        protected async Task<TenantLicenseStatusRefreshInfo> GetLastConnectionWithServer(Guid tenantId)
        {
            return await _tenantLicensingService.GetLastRefreshInfo(tenantId);
        }

        protected async Task<TenantLicenseDetailsOutput> GetLicensedTenantDetails(Guid tenantId)
        {
            return await _licenseServerController.GetLicensedTenantDetails(tenantId);
        }
        
        protected async Task<TenantLicenseDetailsOutput> GetLicensedTenantDetailsLegacy(string databaseName)
        {
            return await _licenseServerLegacyController.GetLicensedTenantDetails(databaseName);
        }
    }
}