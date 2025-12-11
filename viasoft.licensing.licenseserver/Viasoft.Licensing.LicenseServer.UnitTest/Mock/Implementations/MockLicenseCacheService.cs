using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseCache;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.UtilsReturnsToMethods;

namespace Viasoft.Licensing.LicenseServer.UnitTest.Mock.Implementations
{
    public class MockLicenseCacheService: ReturnLicenseTenantById, ILicenseCacheService
    {
        public Task StoreLicenseByTenantId(LicenseByTenantIdOld licenseByTenantIdOld)
        {
            return Task.CompletedTask;
        }

        public Task<LicenseByTenantIdOld> GetLicenseByTenantId(Guid tenantId)
        {
            LicenseByTenantIdOld licenseByTenantIdOld = null;
            
            if (tenantId == Tenants.LicenseForDetails.Id)
                licenseByTenantIdOld = BuildLicenseForTenantDetails();

            return Task.FromResult(licenseByTenantIdOld);
        }
    }
}