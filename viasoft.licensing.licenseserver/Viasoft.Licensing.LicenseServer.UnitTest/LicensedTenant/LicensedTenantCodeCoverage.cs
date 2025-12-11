using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Old.Messages;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseServer;
using Viasoft.Licensing.LicenseServer.Shared.Enums;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts;
using Xunit;

//using System.Globalization;

namespace Viasoft.Licensing.LicenseServer.UnitTest.LicensedTenant
{
    [Collection("sequential")]
    public class LicensedTenantCodeCoverage : LicensedTenantBase, IDisposable
    {
        public LicensedTenantCodeCoverage()
        {
            Setup();
        }
        
        [Fact, Category("CodeCoverage")]
        public async Task Should_Not_Find_App_To_Consume_License()
        {
            var consumer = await ConsumeLicense(Tenants.SimpleLicense.Id, "NotExistingApp", Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            Assert.Equal(ConsumeAppLicenseStatusOld.AppNotLicensed, consumer.ConsumeAppLicenseStatus);
        }
        
        [Fact, Category("CodeCoverage")]
        public async Task Should_Not_Find_App_To_Release_License()
        {
            var releaser = await ReleaseLicense(Tenants.SimpleLicense.Id, "NotExistingApp", Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            Assert.Equal(ReleaseAppLicenseStatusOld.AppNotLicensed, releaser.ReleaseAppLicenseStatus);
        }
        
        [Fact, Category("CodeCoverage")]
        public async Task Should_Not_Find_TenantId_From_Database_Name()
        {
            var releaser = await ReleaseLicenseLegacy("NonExistingDatabase", Apps.SingleLicense.Identifier, Users.User1, Tenants.TwoTenantsConfigurationFromJson.Cnpjs.FirstOrDefault());
            Assert.Equal(ReleaseAppLicenseStatusOld.TenantLicensingNotLoaded, releaser.ReleaseAppLicenseStatus);
        }
        
        [Fact, Category("CodeCoverage")]
        public async Task Check_Available_Licenses_For_App_That_Does_Not_Exists()
        {
            await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, "abc");
            var availableAdditionalLicense = await GetAvailableAdditionalLicense(Tenants.SimpleLicense.Id, "abc");
            Assert.Equal(0, availableLicense);
            Assert.Equal(0, availableAdditionalLicense);
        }

        [Fact, Category("CodeCoverage")]
        public async Task Fill_Memento_Backup_List()
        {
            Task<ConsumeLicenseOutputOld> consumeLicenseOutput = null;
            Task<ReleaseLicenseOutputOld> releaseLicenseOutput = null;
            for (var i = 0; i < 26; i++)
            {
                var unused1 = ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
                consumeLicenseOutput = ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
                var unused2 = ReleaseLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
                releaseLicenseOutput = ReleaseLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            }

            if (consumeLicenseOutput != null) await consumeLicenseOutput;
            if (releaseLicenseOutput != null) await releaseLicenseOutput;

            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }
        
        [Fact, Category("CodeCoverage")]
        public async Task Should_Get_Correct_Amount_Of_License_Usage_In_Real_Time()
        {
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            var licenseUsageInRealTime = GetLicenseUsageInRealTime().First();
            
            Assert.Equal(3, licenseUsageInRealTime.LicenseUsageInRealTimeDetails.Count);
            Assert.Equal(Users.User1, licenseUsageInRealTime.LicenseUsageInRealTimeDetails[0].User);
            Assert.Equal(Users.User2, licenseUsageInRealTime.LicenseUsageInRealTimeDetails[2].User);
        }
        
        [Fact, Category("CodeCoverage")]
        public async Task Should_Get_Correct_Amount_Of_License_Apps()
        {
            var validTenant = await GetTenantLicensedApps(Tenants.AdditionalLicense.Id);
            var invalidTenant = await GetTenantLicensedApps(Guid.Parse("A913446E-C23C-4DA8-ADE3-417189DFF315"));
            
            Assert.Equal(TenantLicensedAppsStatus.Successful, validTenant.Status);
            Assert.Equal(4, validTenant.AppsIdentifiers.Count);
            Assert.Equal(TenantLicensedAppsStatus.TenantNotFound, invalidTenant.Status);
        }
        
        [Fact, Category("CodeCoverage.CheckTenantStatus")]
        public async Task Should_Get_Correct_Tenant_License_Status()
        {
            var activeTenant = await GetTenantLicenseStatus(Tenants.AdditionalLicense.Id);
            var blockedTenant = await GetTenantLicenseStatus(Tenants.BlockedLicense.Id);
            var notFoundTenant = await GetTenantLicenseStatus(Guid.Parse("A913446E-C23C-4DA8-ADE3-417189DFF315"));
            
            Assert.Equal(TenantLicenseStatus.Active, activeTenant.Status);
            Assert.Equal(TenantLicenseStatus.Blocked, blockedTenant.Status);
            Assert.Equal(TenantLicenseStatus.TenantNotFound, notFoundTenant.Status);
        }
        
        [Fact, Category("CodeCoverage")]
        public async Task Should_Check_If_Tenant_Cnpj_Is_Licensed()
        {
            var validCnpj = await IsTenantCnpjLicensed(Tenants.AdditionalLicense.Id, Tenants.AdditionalLicense.Cnpjs.First());
            var invalidCnpj = await IsTenantCnpjLicensed(Tenants.AdditionalLicense.Id, "1234");
            var invalidTenant = await IsTenantCnpjLicensed(Guid.Parse("A913446E-C23C-4DA8-ADE3-417189DFF315"), "1234");
            
            Assert.True(validCnpj.IsCnpjLicensed);
            Assert.False(invalidCnpj.IsCnpjLicensed);
            Assert.False(invalidTenant.IsCnpjLicensed);
        }
        
        [Fact, Category("CodeCoverage")]
        public async Task Release_License_Based_On_Heartbeat()
        {
            var config = ServiceProvider.GetRequiredService<IConfiguration>();
            config["MINIMUM_ALLOWED_HEARTBEAT_IN_SECONDS"] = "1";
            var consumer = await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            await Task.Delay(3000);
            await ReleaseLicenseBasedOnHeartbeat();
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ConsumeAppLicenseStatusOld.LicenseConsumed,  consumer.ConsumeAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses,  availableLicense);
        }
        
        [Fact, Category("CodeCoverage")]
        public async Task Refresh_Tenant_Licensing()
        {
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());

            var licenseServerService = ServiceProvider.GetRequiredService<ILicenseServerService>();
            var licensingDetailsUpdated = new LicensingDetailsUpdated
                {
                    TenantId = Tenants.AdditionalLicense.Id,
                    UpdatedDateTime = DateTime.UtcNow,
                    LicenseByIdentifier = await licenseServerService.GetLicenseByTenantId(Tenants.AdditionalLicense.Id)
                };

            await RefreshTenantLicensing(licensingDetailsUpdated);
            
            var licenseUsageInRealTime = GetLicenseUsageInRealTime().First();
            
            Assert.Equal(3, licenseUsageInRealTime.LicenseUsageInRealTimeDetails.Count);
            Assert.Equal(Users.User1, licenseUsageInRealTime.LicenseUsageInRealTimeDetails[0].User);
            Assert.Equal(Users.User2, licenseUsageInRealTime.LicenseUsageInRealTimeDetails[2].User);
        }
        
        [Fact, Category("CodeCoverage")]
        public async Task Refresh_All_Tenants_Licensing()
        {
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());

            await RefreshAllTenantsLicensing();
            
            var licenseUsageInRealTime = GetLicenseUsageInRealTime().First();
            
            Assert.Equal(3, licenseUsageInRealTime.LicenseUsageInRealTimeDetails.Count);
            Assert.Equal(Users.User1, licenseUsageInRealTime.LicenseUsageInRealTimeDetails[0].User);
            Assert.Equal(Users.User2, licenseUsageInRealTime.LicenseUsageInRealTimeDetails[2].User);
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}