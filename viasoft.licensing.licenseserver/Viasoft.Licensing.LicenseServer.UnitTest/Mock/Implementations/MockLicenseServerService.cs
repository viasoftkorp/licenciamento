using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseServer;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.HardwareId;
using Viasoft.Licensing.LicenseServer.Shared.Enums;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.Consts;
using Viasoft.Licensing.LicenseServer.UnitTest.Mock.UtilsReturnsToMethods;

namespace Viasoft.Licensing.LicenseServer.UnitTest.Mock.Implementations
{
    public class MockLicenseServerService: ReturnLicenseTenantById, ILicenseServerService
    {
        public Task<LicenseByTenantIdOld> GetLicenseByTenantId(Guid tenantId)
        {
            LicenseByTenantIdOld licenseByTenantIdOld = null;

            if (tenantId == Tenants.SimpleLicense.Id)
                licenseByTenantIdOld = BuildLicenseForTenantWithSimpleLicense();
            else if (tenantId == Tenants.SimpleLicenseWithConnectionLicenseType.Id)
                licenseByTenantIdOld = BuildLicenseForTenantWithSimpleLicenseConnectionLicenseType();
            else if (tenantId == Tenants.SimpleLicenseWithAccessLicenseType.Id)
                licenseByTenantIdOld = BuildLicenseForTenantWithSimpleLicenseAccessLicenseType();
            else if (tenantId == Tenants.SimpleLicenseWithinBundleAccessType.Id)
                licenseByTenantIdOld = BuildLicenseForTenantWithSimpleLicenseWithinBundleAndLicenseAccessType();
            else if (tenantId == Tenants.SimpleLicenseWithinBundle.Id)
                licenseByTenantIdOld = BuildLicenseForTenantWithSimpleLicenseWithinBundle();
            else if (tenantId == Tenants.AdditionalLicense.Id)
                licenseByTenantIdOld = BuildLicenseForTenantWithAdditionalLicense();
            else if (tenantId == Tenants.AdditionalLicenseAccessType.Id)
                licenseByTenantIdOld = BuildLicenseForTenantWithAdditionalLicenseForAccessType();
            else if (tenantId == Tenants.BlockedLicense.Id)
                licenseByTenantIdOld = BuildLicenseForTenantWithBlockedLicense();
            else if (tenantId == Tenants.BlockedAppLicense.Id)
                licenseByTenantIdOld = BuildLicenseForTenantWithBlockedAppLicense();
            else if (tenantId == Tenants.BlockedAppAdditionalLicense.Id)
                licenseByTenantIdOld = BuildLicenseForTenantWithBlockedAppAdditionalLicense();
            else if (tenantId == Tenants.TwoTenantsConfigurationFromJson.Id)
                licenseByTenantIdOld = BuildLicenseForTwoTenantsConfigurationFromJson();
            else if (tenantId == Tenants.SingleTenantConfigurationFromJson.Id)
                licenseByTenantIdOld = BuildLicenseForSingleTenantConfigurationFromJson();
            else if (tenantId == Guid.Parse("bed286e0-d5e4-11eb-9d92-fc4596fac591"))
                licenseByTenantIdOld = BuildLicenseForTenantWithWrongHardwareId();

            return Task.FromResult(licenseByTenantIdOld);
        }

        public Task<UpdateHardwareIdOutput> UpdateHardwareId(UpdateHardwareIdInput input)
        {
            return Task.FromResult(new UpdateHardwareIdOutput
            {
                Code = UpdateHardwareIdOutputEnum.Success,
                IsSuccess = true
            });
        }
    }
}