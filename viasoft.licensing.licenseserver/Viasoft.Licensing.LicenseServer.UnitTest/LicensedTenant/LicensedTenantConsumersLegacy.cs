using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts;
using Xunit;

namespace Viasoft.Licensing.LicenseServer.UnitTest.LicensedTenant
{
    [Collection("sequential")]
    public class LicensedTenantConsumersLegacy : LicensedTenantBase, IDisposable
    {
        public LicensedTenantConsumersLegacy()
        {
            Setup();
        }
        
        [Fact, Category("ConsumerLegacy")]
        public async Task Should_Consume_License_Discovering_Tenant_From_Database_Name_Having_Two_Tenants_Available()
        {
            var consumer = await ConsumeLicenseLegacy(Tenants.TwoTenantsConfigurationFromJson.Databases.FirstOrDefault(), 
                Apps.SingleLicense.Identifier, 
                Users.User1, 
                Tenants.TwoTenantsConfigurationFromJson.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.TwoTenantsConfigurationFromJson.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses-1, availableLicense);
        }
        
        [Fact, Category("ConsumerLegacy")]
        public async Task Should_Consume_License_Getting_The_Only_Available_Tenant_Ignoring_The_Database_Name()
        {
            var config = ServiceProvider.GetRequiredService<IConfiguration>();
            config[EnvironmentVariableConsts.TenantLegacyDatabaseMappingConfiguration] = Tenants.SingleTenantConfigurationFromJson.TenantDatabaseConfiguration;

            var consumer = await ConsumeLicenseLegacy("DatabaseNameDoesNotMatter", 
                Apps.SingleLicense.Identifier, 
                Users.User1, 
                Tenants.SingleTenantConfigurationFromJson.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SingleTenantConfigurationFromJson.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses-1, availableLicense);
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}