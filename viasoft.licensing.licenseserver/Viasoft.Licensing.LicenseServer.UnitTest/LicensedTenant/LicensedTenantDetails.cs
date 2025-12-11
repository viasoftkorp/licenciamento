using System;
using System.ComponentModel;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseCache;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts;
using Xunit;

namespace Viasoft.Licensing.LicenseServer.UnitTest.LicensedTenant
{
    [Collection("sequential")]
    public class LicensedTenantDetails: LicensedTenantBase, IDisposable
    {
        public LicensedTenantDetails()
        {
            Setup();
        }
        
        [Fact, Category("TenantDetails")]
        public async Task Find_Details_From_Tenant_With_License()
        {
            // prepare
            var licenseServerService = ServiceProvider.GetRequiredService<ILicenseCacheService>();
            var licenseByTenantId = await licenseServerService.GetLicenseByTenantId(Tenants.LicenseForDetails.Id);
            var expectedOutput = new TenantLicenseDetailsOutput
            {
                Cnpjs = licenseByTenantId.Cnpjs,
                Identifier = licenseByTenantId.Identifier,
                Status = licenseByTenantId.Status,
                ExpirationDateTime = licenseByTenantId.ExpirationDateTime
            };
            // execute
            var details = await GetLicensedTenantDetails(licenseByTenantId.Identifier);
            // test
            expectedOutput.Should().BeEquivalentTo(details);
        }
        
        [Fact, Category("TenantDetails")]
        public async Task Find_Details_From_Tenant_With_No_License()
        {
            // prepare
            var fakeTenantId = Guid.NewGuid();
            // execute
            var details = await GetLicensedTenantDetails(fakeTenantId);
            // test
            Assert.Null(details);
        }
        
        [Fact, Category("TenantDetails")]
        public async Task Find_Details_From_Tenant_With_License_Legacy()
        {
            // prepare
            var licenseServerService = ServiceProvider.GetRequiredService<ILicenseCacheService>();
            var licenseByTenantId = await licenseServerService.GetLicenseByTenantId(Tenants.LicenseForDetails.Id);
            var expectedOutput = new TenantLicenseDetailsOutput
            {
                Cnpjs = licenseByTenantId.Cnpjs,
                Identifier = licenseByTenantId.Identifier,
                Status = licenseByTenantId.Status,
                ExpirationDateTime = licenseByTenantId.ExpirationDateTime
            };
            // execute
            var details = await GetLicensedTenantDetailsLegacy("TESTCOMPLETE_V16_0_0");
            // test
            expectedOutput.Should().BeEquivalentTo(details);
        }
        
        [Fact, Category("TenantDetails")]
        public async Task Find_Details_From_Tenant_With_No_License_Legacy()
        {
            // execute
            var details = await GetLicensedTenantDetailsLegacy("fail");
            // test
            Assert.Null(details);
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}