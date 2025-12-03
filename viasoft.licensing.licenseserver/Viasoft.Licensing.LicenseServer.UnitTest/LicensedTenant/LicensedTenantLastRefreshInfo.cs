using System;
using System.ComponentModel;
using System.Threading.Tasks;
using FluentAssertions;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Initializer;
using Xunit;

namespace Viasoft.Licensing.LicenseServer.UnitTest.LicensedTenant
{
    [Collection("sequential")]
    public class LicensedTenantLastRefreshInfo: LicensedTenantBase, IDisposable
    {

        public LicensedTenantLastRefreshInfo()
        {
            Setup();
        }

        [Fact, Category("ConnectionWithServer")]
        public async Task Should_Not_Found_Find_Last_Refresh_Info()
        {
            // prepare
            var tenantId = Guid.NewGuid();
            // execute
            var lastConnection = await GetLastConnectionWithServer(tenantId);
            // test
            Assert.Null(lastConnection);
        }
        
        [Fact, Category("ConnectionWithServer")]
        public async Task Should_Find_Last_Refresh_Info_One_Data()
        {
            // prepare
            var tenantId = Guid.NewGuid();
            var fakeDataInDb = await StoreFakeLicenseData(tenantId, true);
            // execute
            var lastConnection = await GetLastConnectionWithServer(tenantId);
            // test
            fakeDataInDb.Should().BeEquivalentTo(lastConnection);
        }
        
        [Fact, Category("ConnectionWithServer")]
        public async Task Should_Find_Last_Refresh_Info_Multiple_Data()
        {
            // prepare
            for (int i = 0; i < 5; i++)
            {
                var currentTenant = Guid.NewGuid();
                var currentSuccess = i < 2;
                await StoreFakeLicenseData(currentTenant, currentSuccess);
            }
            var tenantId = Guid.NewGuid();
            var fakeDataInDb = await StoreFakeLicenseData(tenantId, true);
            // execute
            var lastConnection = await GetLastConnectionWithServer(tenantId);
            // test
            fakeDataInDb.Should().BeEquivalentTo(lastConnection);
        }

        private Task<TenantLicenseStatusRefreshInfo> StoreFakeLicenseData(Guid tenantId, bool success)
        {
            var fakeData = new TenantLicenseStatusRefreshInfo
            {
                TenantId = tenantId,
                LastRefreshDateTime = new DateTime(2040, 3, 5, 12, 5, 15, 5),
                RefreshSucceed = success
            };
            using (var db = LiteDbInitializer.NewDatabase(LiteDbConsts.BuildTenantConnectionStringOld(tenantId)))
            {
                var tenantLicenseStatusLastConnectionWithServerCollection = db.GetCollection<TenantLicenseStatusRefreshInfo>(nameof(TenantLicenseStatusRefreshInfo));
                tenantLicenseStatusLastConnectionWithServerCollection.Upsert(fakeData.TenantId, fakeData);
            }
            fakeData.LastRefreshDateTime = fakeData.LastRefreshDateTime.ToUniversalTime();
            return Task.FromResult(fakeData);
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}