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
    public class LicensedTenantReleasersLegacy : LicensedTenantBase, IDisposable
    {
        public LicensedTenantReleasersLegacy()
        {
            Setup();
        }
        
        [Fact, Category("ReleaserLegacy")]
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

        public void Dispose()
        {
            TearDown();
        }
    }
}