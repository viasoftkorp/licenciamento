using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts;
using Xunit;

namespace Viasoft.Licensing.LicenseServer.UnitTest.LicensedTenant
{
    [Collection("sequential")]
    public class LicensedTenantReleasers : LicensedTenantBase, IDisposable
    {

        public LicensedTenantReleasers()
        {
            Setup();
        }
        
        [Fact, Category("Releaser")]
        public async Task Should_Release_Additional_License()
        {
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            var releaser = await ReleaseLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            var availableAdditionalLicense = await GetAvailableAdditionalLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.AdditionalLicenseReleased,  releaser.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.AdditionalNumberOfLicenses, availableAdditionalLicense);
        }
        
        [Fact, Category("Releaser")]
        public async Task Should_Release_License_For_Loose_App()
        {
            await ConsumeLicense(Tenants.SimpleLicenseWithAccessLicenseType.Id, Apps.MultipleLicense.Identifier, Users.User1, Tenants.SimpleLicenseWithAccessLicenseType.Cnpjs.FirstOrDefault());
            var releaser = await ReleaseLicense(Tenants.SimpleLicenseWithAccessLicenseType.Id, Apps.MultipleLicense.Identifier, Users.User1, Tenants.SimpleLicenseWithAccessLicenseType.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicenseWithAccessLicenseType.Id, Apps.MultipleLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseReleased,  releaser.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.MultipleLicense.NumberOfLicenses, availableLicense);
        }
        
        [Fact, Category("Releaser")]
        public async Task Should_Release_Additional_License_Access_Type_For_Bundle()
        {
            await ConsumeLicense(Tenants.AdditionalLicenseAccessType.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicenseAccessType.Cnpjs.FirstOrDefault());
            await ConsumeLicense(Tenants.AdditionalLicenseAccessType.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicenseAccessType.Cnpjs.FirstOrDefault());
            var releaseLicenseForUserOne = await ReleaseLicense(Tenants.AdditionalLicenseAccessType.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicenseAccessType.Cnpjs.FirstOrDefault());
            var availableAdditionalLicense = await GetAvailableAdditionalLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.AdditionalLicenseReleased,  releaseLicenseForUserOne.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.AdditionalNumberOfLicenses, availableAdditionalLicense);
        }
        
        [Fact, Category("Releaser")]
        public async Task Should_Release_License_Access_Type_For_Bundle()
        {
            await ConsumeLicense(Tenants.SimpleLicenseWithinBundleAccessType.Id, Apps.MultipleLicenseWithinBundle.Identifier, Users.User2, Tenants.SimpleLicenseWithinBundleAccessType.Cnpjs.FirstOrDefault());
            await ConsumeLicense(Tenants.SimpleLicenseWithinBundleAccessType.Id, Apps.MultipleLicenseWithinBundle.Identifier, Users.User1, Tenants.SimpleLicenseWithinBundleAccessType.Cnpjs.FirstOrDefault());
            var releaseLicenseForUserOne = await ReleaseLicense(Tenants.SimpleLicenseWithinBundleAccessType.Id, Apps.MultipleLicenseWithinBundle.Identifier, Users.User1, Tenants.SimpleLicenseWithinBundleAccessType.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicenseWithinBundleAccessType.Id, Apps.MultipleLicenseWithinBundle.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseReleased,  releaseLicenseForUserOne.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.MultipleLicenseWithinBundle.NumberOfLicenses - 1, availableLicense);
        }
        
        [Fact, Category("Releaser")]
        public async Task Should_Release_License_After_Additional_License_Is_Consumed()
        {
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            await ConsumeLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User2, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            var releaser = await ReleaseLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier, Users.User1, Tenants.AdditionalLicense.Cnpjs.FirstOrDefault());
            
            var availableAdditionalLicense = await GetAvailableAdditionalLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            var availableLicense = await GetAvailableLicense(Tenants.AdditionalLicense.Id, Apps.BundledPlusSingleAdditionalLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseReleased,  releaser.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.AdditionalNumberOfLicenses-1, availableAdditionalLicense);
            Assert.Equal(Apps.BundledPlusSingleAdditionalLicense.NumberOfLicenses, availableLicense);
        }
        
        [Fact, Category("Releaser")]
        public async Task Should_Release_Consumed_License()
        {
            await ConsumeLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            var releaser = await ReleaseLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseReleased, releaser.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }
        
        [Fact, Category("Releaser")]
        public async Task Should_Release_Consumed_License_Discovering_Tenant_From_Database_Name()
        {
            await ConsumeLicenseLegacy(Tenants.TwoTenantsConfigurationFromJson.Databases.FirstOrDefault(), 
                Apps.SingleLicense.Identifier, Users.User1, Tenants.TwoTenantsConfigurationFromJson.Cnpjs.FirstOrDefault());
            
            var releaser = await ReleaseLicenseLegacy(Tenants.TwoTenantsConfigurationFromJson.Databases.FirstOrDefault(), 
                Apps.SingleLicense.Identifier, Users.User1, Tenants.TwoTenantsConfigurationFromJson.Cnpjs.FirstOrDefault());
            
            var availableLicense = await GetAvailableLicense(Tenants.TwoTenantsConfigurationFromJson.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.LicenseReleased,  releaser.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }
        
        [Fact, Category("Releaser")]
        public async Task Try_To_Release_But_Return_No_Consumed_Licensed_To_Release()
        {
            var releaser = await ReleaseLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.SimpleLicense.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.NoConsumedLicenseToRelease, releaser.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }
        
        [Fact, Category("Releaser")]
        public async Task Try_To_Release_But_Return_App_Blocked()
        {
            var releaser = await ReleaseLicense(Tenants.BlockedAppLicense.Id, Apps.BlockedLicense.Identifier, Users.User1, Tenants.BlockedAppLicense.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.AppBlocked, releaser.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }
        
        [Fact, Category("Releaser")]
        public async Task Try_To_Release_Additional_License_But_Return_App_Blocked()
        {
            var releaser = await ReleaseLicense(Tenants.BlockedAppAdditionalLicense.Id, Apps.BlockedAdditionalLicense.Identifier, Users.User1, Tenants.BlockedAppAdditionalLicense.Cnpjs.FirstOrDefault());
            var availableAdditionalLicense = await GetAvailableAdditionalLicense(Tenants.BlockedAppAdditionalLicense.Id, Apps.BlockedAdditionalLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.AppBlocked, releaser.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.BlockedAdditionalLicense.AdditionalNumberOfLicenses, availableAdditionalLicense);
        }
        
        [Fact, Category("Releaser")]
        public async Task Try_To_Release_But_Not_Find_Licensed_Cnpj_And_Return_NoConsumedLicenseToRelease()
        {
            var releaser = await ReleaseLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier, Users.User1, "1234");
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.NoConsumedLicenseToRelease, releaser.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }
        
        [Fact, Category("Releaser")]
        public async Task Try_To_Release_But_Tenant_Is_Blocked_And_Return_NoConsumedLicenseToRelease()
        {
            var releaser = await ReleaseLicense(Tenants.BlockedLicense.Id, Apps.SingleLicense.Identifier, Users.User1, Tenants.BlockedLicense.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatusOld.NoConsumedLicenseToRelease, releaser.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }
        
        /*
        [Test, Category("Releaser"), Ignore("Expired license business rule not yet defined")]
        public async Task Try_To_Release_But_Return_License_Expired()
        {
            var releaser = await ReleaseLicense(Tenants.BlockedAppLicense.Id, Apps.BlockedLicense.Identifier, Users.User1, Tenants.BlockedAppLicense.Cnpjs.FirstOrDefault());
            var availableLicense = await GetAvailableLicense(Tenants.SimpleLicense.Id, Apps.SingleLicense.Identifier);
            
            Assert.Equal(ReleaseAppLicenseStatus.AppBlocked, releaser.ReleaseAppLicenseStatus);
            Assert.Equal(Apps.SingleLicense.NumberOfLicenses, availableLicense);
        }
        */
        public void Dispose()
        {
            TearDown();
        }
    }
}