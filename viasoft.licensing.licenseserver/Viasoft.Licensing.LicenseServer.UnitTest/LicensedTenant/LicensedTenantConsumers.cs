using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicensedTenantOrchestrator;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.Implementations;
using Xunit;

namespace Viasoft.Licensing.LicenseServer.UnitTest.LicensedTenant
{
    [Collection("sequential")]
    public class LicensedTenantConsumers : LicensedTenantBase, IDisposable
    {
        public LicensedTenantConsumers()
        {
            Setup();
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_Additional_License()
        {
            var consumer1 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledWithNoAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            var consumer2 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            var availableLicense = await GetAvailableLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            var availableAdditionalLicense = await GetAvailableAdditionalLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer1.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.AdditionalLicenseConsumed,  consumer2.ConsumeAppLicenseStatus);
            
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.NumberOfLicenses-1, availableLicense);
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.AdditionalNumberOfLicenses-1, availableAdditionalLicense);
        }

        [Fact, Category("Consumer")]
        public async Task Should_Consume_License()
        {
            var consumer = await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses-1, availableLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_License_With_Connection_Type_For_Loose_App()
        {
            var consumer1 = await ConsumeLicense(Tenants.SimpleLicenseWithConnectionLicenseType.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicenseWithConnectionLicenseType.Cnpjs.FirstOrDefault());
            var consumer2 = await ConsumeLicense(Tenants.SimpleLicenseWithConnectionLicenseType.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicenseWithConnectionLicenseType.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicenseWithConnectionLicenseType.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed, consumer1.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseAlreadyInUseByUser, consumer2.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses - 1, availableLicense);
        }

        [Fact, Category("Consumer")]
        public async Task Should_Consume_License_With_Access_Type_Multiple_License_For_Loose_App()
        {
            var consumer1 = await ConsumeLicense(Tenants.SimpleLicenseWithAccessLicenseType.Id, Apps.MultipleLicense.Identifier, Users.User1, Tenants.SimpleLicenseWithAccessLicenseType.Cnpjs.FirstOrDefault());
            var consumer2 = await ConsumeLicense(Tenants.SimpleLicenseWithAccessLicenseType.Id, Apps.MultipleLicense.Identifier, Users.User1, Tenants.SimpleLicenseWithAccessLicenseType.Cnpjs.FirstOrDefault());
            var consumer3 = await ConsumeLicense(Tenants.SimpleLicenseWithAccessLicenseType.Id, Apps.MultipleLicense.Identifier, Users.User1, Tenants.SimpleLicenseWithAccessLicenseType.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicenseWithAccessLicenseType.Id, Apps.MultipleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed, consumer1.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed, consumer2.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.NotEnoughLicenses, consumer3.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.MultipleLicense.NumberOfLicenses - 2, availableLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_License_With_Connection_Type_For_Bundle()
        {
            var consumer1 = await ConsumeLicense(Tenants.SimpleLicenseWithinBundle.Id, Apps.SingleLicenseWithinBundle.Identifier, Users.User1, Tenants.SimpleLicenseWithinBundle.Cnpjs.FirstOrDefault());
            var consumer2 = await ConsumeLicense(Tenants.SimpleLicenseWithinBundle.Id, Apps.SingleLicenseWithinBundle.Identifier, Users.User1, Tenants.SimpleLicenseWithinBundle.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicenseWithAccessLicenseType.Id, Apps.SingleLicenseWithinBundle.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed, consumer1.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseAlreadyInUseByUser, consumer2.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicenseWithinBundle.NumberOfLicenses - 1, availableLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_License_With_Connection_Type_For_Bundle_Additional_License()
        {
            var consumer1 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledWithNoAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            var consumer2 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            var consumer3 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            var availableAdditionalLicense = await GetAvailableAdditionalLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer1.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.AdditionalLicenseConsumed,  consumer2.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.AdditionalLicenseAlreadyInUseByUser,  consumer3.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.NumberOfLicenses - 1, availableLicense);
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.AdditionalNumberOfLicenses - 1, availableAdditionalLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_License_With_Access_Type_Multiple_License_For_Bundle()
        {
            var consumer1 = await ConsumeLicense(Tenants.SimpleLicenseWithinBundleAccessType.Id, Apps.MultipleLicenseWithinBundle.Identifier, Users.User1, Tenants.SimpleLicenseWithinBundleAccessType.Cnpjs.FirstOrDefault());
            var consumer2 = await ConsumeLicense(Tenants.SimpleLicenseWithinBundleAccessType.Id, Apps.MultipleLicenseWithinBundle.Identifier, Users.User1, Tenants.SimpleLicenseWithinBundleAccessType.Cnpjs.FirstOrDefault());
            var consumer3 = await ConsumeLicense(Tenants.SimpleLicenseWithinBundleAccessType.Id, Apps.MultipleLicenseWithinBundle.Identifier, Users.User1, Tenants.SimpleLicenseWithinBundleAccessType.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicenseWithAccessLicenseType.Id, Apps.SingleLicenseWithinBundle.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed, consumer1.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed, consumer2.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.NotEnoughLicenses, consumer3.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.MultipleLicense.NumberOfLicenses - 2, availableLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_License_With_Access_Type_Multiple_License_For_Bundle_Additional_License()
        {
            var consumer1 = await ConsumeLicense(Tenants.AdditionalLicenseAccessType.Id, Apps.BundledWithNoAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicenseAccessType.Cnpjs.FirstOrDefault());
            var consumer2 = await ConsumeLicense(Tenants.AdditionalLicenseAccessType.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicenseAccessType.Cnpjs.FirstOrDefault());
            var consumer3 = await ConsumeLicense(Tenants.AdditionalLicenseAccessType.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicenseAccessType.Cnpjs.FirstOrDefault());
            
            var availableLicense = await GetAvailableLicense(Tenants.AdditionalLicenseAccessType.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            var availableAdditionalLicense = await GetAvailableAdditionalLicense(Tenants.AdditionalLicenseAccessType.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer1.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.AdditionalLicenseConsumed,  consumer2.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.NotEnoughLicenses,  consumer3.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.NumberOfLicenses - 1, availableLicense);
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.AdditionalNumberOfLicenses - 1, availableAdditionalLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_And_Release_Same_License_Twice()
        {
            var user1Consume1 = await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            var user1Consume2 = await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            var availableLicenseWhileConsuming = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            var user1Release1 = await ReleaseLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            var user1Release2 = await ReleaseLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            var availableLicenseAfterReleasing = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  user1Consume1.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseAlreadyInUseByUser,  user1Consume2.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses-1, availableLicenseWhileConsuming);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseStillInUseByUser,  user1Release1.ReleaseAppLicenseStatus);
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseReleased,  user1Release2.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicenseAfterReleasing);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_And_Release_Same_License_Twice_Within_A_Bundle()
        {
            var user1Consume1 = await ConsumeLicense(Tenants.SimpleLicenseWithinBundle.Id, Apps.SingleLicenseWithinBundle.Identifier, Users.User1, Tenants.SimpleLicenseWithinBundle.Cnpjs.FirstOrDefault());
            var user1Consume2 = await ConsumeLicense(Tenants.SimpleLicenseWithinBundle.Id, Apps.SingleLicenseWithinBundle.Identifier, Users.User1, Tenants.SimpleLicenseWithinBundle.Cnpjs.FirstOrDefault());
            var availableLicenseWhileConsuming = await GetAvailableLicense(Tenants.SimpleLicenseWithinBundle.Id, Apps.SingleLicense.Identifier);
            
            var user1Release1 = await ReleaseLicense(Tenants.SimpleLicenseWithinBundle.Id, Apps.SingleLicenseWithinBundle.Identifier, Users.User1, Tenants.SimpleLicenseWithinBundle.Cnpjs.FirstOrDefault());
            var user1Release2 = await ReleaseLicense(Tenants.SimpleLicenseWithinBundle.Id, Apps.SingleLicenseWithinBundle.Identifier, Users.User1, Tenants.SimpleLicenseWithinBundle.Cnpjs.FirstOrDefault());
            var availableLicenseAfterReleasing = await GetAvailableLicense(Tenants.SimpleLicenseWithinBundle.Id, Apps.SingleLicenseWithinBundle.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  user1Consume1.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseAlreadyInUseByUser,  user1Consume2.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicenseWithinBundle.NumberOfLicenses-1, availableLicenseWhileConsuming);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseStillInUseByUser,  user1Release1.ReleaseAppLicenseStatus);
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseReleased,  user1Release2.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.SingleLicenseWithinBundle.NumberOfLicenses, availableLicenseAfterReleasing);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_And_Release_Same_Additional_License()
        {
            var consumer1 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledWithNoAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            var user2Consumer1 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            var user2Consumer2 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            var availableLicense = await GetAvailableLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            var availableAdditionalLicense = await GetAvailableAdditionalLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            
            var user2Release1 = await ReleaseLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            var user2Release2 = await ReleaseLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            var availableAdditionalLicenseAfterReleasing = await GetAvailableAdditionalLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer1.ConsumeAppLicenseStatus);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.AdditionalLicenseConsumed,  user2Consumer1.ConsumeAppLicenseStatus);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.AdditionalLicenseAlreadyInUseByUser,  user2Consumer2.ConsumeAppLicenseStatus);
            
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.NumberOfLicenses-1, availableLicense);
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.AdditionalNumberOfLicenses-1, availableAdditionalLicense);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.AdditionalLicenseStillInUseByUser,  user2Release1.ReleaseAppLicenseStatus);
            Assert.Equal(ReleaseAppLicenseStatusOld.AdditionalLicenseReleased,  user2Release2.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.AdditionalNumberOfLicenses, availableAdditionalLicenseAfterReleasing);
        }
        
        [Fact, Category("Consumer")]
        public async Task Try_To_Consume_But_Return_Not_Enough_Licenses()
        {
            var consumer1Task = ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            var consumer2Task = ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User2, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());

            await consumer1Task;
            var consumer2 = await consumer2Task;
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.NotEnoughLicenses, consumer2.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses-1, availableLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Try_To_Consume_But_Return_App_Blocked()
        {
            var consumer = await ConsumeLicense(Tenants.BlockedAppLicense.Id, Apps.BlockedLicense.Identifier, Users.User1, Tenants.BlockedAppLicense.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.AppBlocked, consumer.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Try_To_Consume_Additional_License_But_Return_App_Blocked()
        {
            var consumer = await ConsumeLicense(Tenants.BlockedAppAdditionalLicense.Id, Apps.BlockedAdditionalLicense.Identifier, Users.User1, Tenants.BlockedAppAdditionalLicense.Cnpjs.FirstOrDefault());
            var availableAdditionalLicense = await GetAvailableAdditionalLicense(Tenants.BlockedAppAdditionalLicense.Id, Apps.BlockedAdditionalLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.AppBlocked, consumer.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.BlockedAdditionalLicense.AdditionalNumberOfLicenses, availableAdditionalLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Try_To_Consume_But_Return_Cnpj_Not_Licensed()
        {
            var consumer = await ConsumeLicense(Tenants.BlockedAppLicense.Id, Apps.BlockedLicense.Identifier, Users.User1, "1234");
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.CnpjNotLicensed, consumer.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Try_To_Consume_But_Return_Tenant_Blocked()
        {
            var consumer = await ConsumeLicense(Tenants.BlockedLicense.Id, Apps.BlockedLicense.Identifier, Users.User1, Tenants.BlockedLicense.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.TenantBlocked, consumer.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Try_To_Consume_But_Return_Not_Enough_Licenses_After_Additional_Licenses_Are_Consumed()
        {
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            var consumer3 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User3, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            Assert.Equal(ConsumeAppLicenseStatusOld.NotEnoughLicenses,  consumer3.ConsumeAppLicenseStatus);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_License_And_Consume_Again_Releasing_From_Heartbeat()
        {
            var config = ServiceProvider.GetRequiredService<IConfiguration>();
            config["MINIMUM_ALLOWED_HEARTBEAT_IN_SECONDS"] = "1";

            var consumer = await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            await Task.Delay(3000);
            var consumer2 = await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User2, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer2.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses-1, availableLicense);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Try_To_Consume_License_But_Not_Find_Tenant()
        {
            var consumer = await ConsumeLicense(Guid.Parse("C3A9DEAC-D90B-4DD9-836A-0409144C512D"), Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            Assert.Equal(ConsumeAppLicenseStatusOld.TenantLicensingNotLoaded,  consumer.ConsumeAppLicenseStatus);
        }

        [Fact, Category("Consumer")]
        public async Task Should_try_To_Consume_license_But_Not_Match_HardwareId()
        {
            DefaultConfigurationConsts.IsRunningAsLegacy = true;
            var tenantId = Guid.Parse("bed286e0-d5e4-11eb-9d92-fc4596fac591");
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
            
            var orchestratorService = new LicensedTenantOrchestratorService(ServiceProvider, configuration, new MockProvideHardwareIdService());

            var result = await orchestratorService.ConsumeLicense(new ConsumeLicenseInput
            {
                TenantId = tenantId,
                Cnpj = "81634257000105",
                User = "Lucas",
                AppIdentifier = "App",
                CustomAppName = "BigApp"
            });

            result.ConsumeAppLicenseStatus.Should().Be(ConsumeAppLicenseStatusOld.HardwareIdDoesNotMatch);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_And_Release_License_And_Maintain_Custom_App_Name_And_License_Usage_Additional_Information()
        {
            const string customAppName = "Custom App Here";
            var licenseUsageAdditionalInformation = new LicenseUsageAdditionalInformationOld
            {
                SoftwareVersion = "1.0.x",
                HostName = "Host_1",
                HostUser = "Host_User_1",
                LocalIpAddress = "192.168.1.1",
                Language = "en_us",
                OsInfo = "Windows 10 x64",
                BrowserInfo = "Google Chrome",
                DatabaseName = "VIASOFT_KORP"
            };
            
            var licenseConsumer = await ConsumeLicense(Tenants.SimpleLicenseWithinBundle.Id, 
                                                    Apps.SingleLicenseWithinBundle.Identifier, 
                                                    Users.User1, 
                                                    Tenants.SimpleLicenseWithinBundle.Cnpjs.FirstOrDefault(), 
                                                    customAppName, licenseUsageAdditionalInformation);
            
            var licenseUsageInRealTime = GetLicenseUsageInRealTime().First().LicenseUsageInRealTimeDetails.First();
            
            await ReleaseLicense(Tenants.SimpleLicenseWithinBundle.Id, 
                                                    Apps.SingleLicenseWithinBundle.Identifier, 
                                                    Users.User1, 
                                                    Tenants.SimpleLicenseWithinBundle.Cnpjs.FirstOrDefault());

            var licenseUsageAfterRelease = (await GetLicensesUsage(Tenants.SimpleLicenseWithinBundle.Id)).First();
            var licenseUsageAdditionalInformationFromRealTime = licenseUsageInRealTime.LicenseUsageAdditionalInformation;

            Assert.Equal(customAppName,  licenseUsageInRealTime.AppName);
            Assert.Equal(licenseUsageAdditionalInformation.SoftwareVersion,  licenseUsageAdditionalInformationFromRealTime.SoftwareVersion);
            Assert.Equal(licenseUsageAdditionalInformation.HostName,  licenseUsageAdditionalInformationFromRealTime.HostName);
            Assert.Equal(licenseUsageAdditionalInformation.HostUser,  licenseUsageAdditionalInformationFromRealTime.HostUser);
            Assert.Equal(licenseUsageAdditionalInformation.LocalIpAddress,  licenseUsageAdditionalInformationFromRealTime.LocalIpAddress);
            Assert.Equal(licenseUsageAdditionalInformation.Language,  licenseUsageAdditionalInformationFromRealTime.Language);
            Assert.Equal(licenseUsageAdditionalInformation.OsInfo,  licenseUsageAdditionalInformationFromRealTime.OsInfo);
            Assert.Equal(licenseUsageAdditionalInformation.BrowserInfo,  licenseUsageAdditionalInformationFromRealTime.BrowserInfo);
            Assert.Equal(licenseUsageAdditionalInformation.DatabaseName,  licenseUsageAdditionalInformationFromRealTime.DatabaseName);
            
            Assert.Equal(customAppName,  licenseUsageAfterRelease.AppName);
            Assert.Equal(licenseUsageAdditionalInformation.SoftwareVersion,  licenseUsageAfterRelease.SoftwareVersion);
            Assert.Equal(licenseUsageAdditionalInformation.HostName,  licenseUsageAfterRelease.HostName);
            Assert.Equal(licenseUsageAdditionalInformation.HostUser,  licenseUsageAfterRelease.HostUser);
            Assert.Equal(licenseUsageAdditionalInformation.LocalIpAddress,  licenseUsageAfterRelease.LocalIpAddress);
            Assert.Equal(licenseUsageAdditionalInformation.Language,  licenseUsageAfterRelease.Language);
            Assert.Equal(licenseUsageAdditionalInformation.OsInfo,  licenseUsageAfterRelease.OsInfo);
            Assert.Equal(licenseUsageAdditionalInformation.BrowserInfo,  licenseUsageAfterRelease.BrowserInfo);
            Assert.Equal(licenseUsageAdditionalInformation.DatabaseName,  licenseUsageAfterRelease.DatabaseName);
            
            Assert.Equal(customAppName,  licenseConsumer.AppName);
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  licenseConsumer.ConsumeAppLicenseStatus);
        }
        
        [Fact, Category("Consumer")]
        public async Task Should_Consume_Additional_License_Then_After_Another_User_Release_License_Should_Consume_License_All_Within_Bundle()
        {
            // Bundle has 1 license.
            // App BundledPlusSingleAdditionalLicense has 1 additional license.
            // App BundledWithNoAdditionalLicense has 0 additional licenses.
            
            // User1 App BundledPlusSingleAdditionalLicense must consume normal license from Bundle
            var user1Consumer1 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            // User2 App BundledPlusSingleAdditionalLicense must consume additional license from App
            var user2Consumer1 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            // User1 App BundledPlusSingleAdditionalLicense must Release normal license from Bundle
            var user1Releaser1 = await ReleaseLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            // User2 App BundledWithNoAdditionalLicense must consume normal license from Bundle, even though the user already consumes an additional license of the bundle (user2Consumer1).
            var user2Consumer2 = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledWithNoAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            // User2 App BundledWithNoAdditionalLicense must consume same normal license as user2Consumer2.
            var user2Consumer2Duplicated = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledWithNoAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            // User2 App BundledPlusSingleAdditionalLicense must consume same additional license as user2Consumer1.
            var user2Consumer1Duplicated = await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            // User2 App BundledPlusSingleAdditionalLicense must Release one use of additional license from app (user2Consumer1).
            var user2Releaser1Duplicated = await ReleaseLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());

            // User2 App BundledPlusSingleAdditionalLicense must Release additional license from app (user2Consumer1).
            var user2Releaser1 = await ReleaseLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            // User2 App BundledPlusSingleAdditionalLicense must Release one use of normal license from Bundle (user2Consumer2).
            var user2Releaser2Duplicated = await ReleaseLicense(Tenants.AdditionalLicense.Id, Apps.BundledWithNoAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            // User2 App BundledPlusSingleAdditionalLicense must Release normal license from Bundle (user2Consumer2).
            var user2Releaser2 = await ReleaseLicense(Tenants.AdditionalLicense.Id, Apps.BundledWithNoAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());

            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  user1Consumer1.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.AdditionalLicenseConsumed,  user2Consumer1.ConsumeAppLicenseStatus);
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseReleased,  user1Releaser1.ReleaseAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  user2Consumer2.ConsumeAppLicenseStatus);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.AdditionalLicenseAlreadyInUseByUser,  user2Consumer1Duplicated.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseAlreadyInUseByUser,  user2Consumer2Duplicated.ConsumeAppLicenseStatus);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.AdditionalLicenseStillInUseByUser,  user2Releaser1Duplicated.ReleaseAppLicenseStatus);
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseStillInUseByUser,  user2Releaser2Duplicated.ReleaseAppLicenseStatus);

            Assert.Equal(ReleaseAppLicenseStatusOld.AdditionalLicenseReleased,  user2Releaser1.ReleaseAppLicenseStatus);
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseReleased,  user2Releaser2.ReleaseAppLicenseStatus);
        }
        
        /*[Test, Category("Consumer"), Ignore("Expired license business rule not yet defined")]
        public async Task Try_To_Consume_But_Return_License_Expired()
        {
            var consumer = await ConsumeLicense(Tenants.ExpiredLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.ExpiredLicense.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatus.AppExpired, consumer.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }*/
        public void Dispose()
        {
            TearDown();
        }
    }
}