using System;
using System.ComponentModel;
using System.Globalization;
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
    public class LicensedTenantRefreshAppLicense : LicensedTenantBase, IDisposable
    {

        public LicensedTenantRefreshAppLicense()
        {
            Setup();
        }
        
        [Fact, Category("RefreshAppLicense")]
        public async Task Should_Consume_License_And_Refresh_License_Use_Then_Another_Consumer_Return_Not_Enough_Licenses()
        {
            const double secondsToReleaseLicenseBasedOnHeartBeat = 0.005;
            Environment.SetEnvironmentVariable(EnvironmentVariableConsts.MinimumAllowedHeartbeatInSeconds,secondsToReleaseLicenseBasedOnHeartBeat.ToString(CultureInfo.CurrentCulture));
            
            var consumer = await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            await Task.Delay(5);
            var refreshAppLicenseInUseByUserOutput = await RefreshAppLicenseInUseByUser(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            var consumer2 = await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User2, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.NotEnoughLicenses,  consumer2.ConsumeAppLicenseStatus);
            Assert.Equal(RefreshAppLicenseInUseByUserStatusOld.RefreshSuccessful,  refreshAppLicenseInUseByUserOutput.Status);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses-1, availableLicense);
        }
        
        [Fact, Category("RefreshAppLicense")]
        public async Task Should_Consume_License_Then_Fail_To_Refresh_License()
        {
            var config = ServiceProvider.GetRequiredService<IConfiguration>();
            config["MINIMUM_ALLOWED_HEARTBEAT_IN_SECONDS"] = "1";

            var consumer = await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            await Task.Delay(3000);
            var consumer2 = await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User2, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            var refreshAppLicenseInUseByUserOutput = await RefreshAppLicenseInUseByUser(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer.ConsumeAppLicenseStatus);
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer2.ConsumeAppLicenseStatus);
            Assert.Equal(RefreshAppLicenseInUseByUserStatusOld.RefreshFailedLicenseNotAvailable,  refreshAppLicenseInUseByUserOutput.Status);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses-1, availableLicense);
        }
        
        [Fact, Category("RefreshAppLicense")]
        public async Task Should_Not_Find_Tenant_To_Refresh_License()
        {
            const double secondsToReleaseLicenseBasedOnHeartBeat = 0.005;
            Environment.SetEnvironmentVariable(EnvironmentVariableConsts.MinimumAllowedHeartbeatInSeconds,secondsToReleaseLicenseBasedOnHeartBeat.ToString(CultureInfo.CurrentCulture));
            
            var refreshAppLicenseInUseByUserOutput = await RefreshAppLicenseInUseByUser(Guid.Parse("6938DBCD-65AA-4392-95FB-5B1F9AF78EBA"), 
                Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            Assert.Equal(RefreshAppLicenseInUseByUserStatusOld.TenantLicensingNotLoaded,  refreshAppLicenseInUseByUserOutput.Status);
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}