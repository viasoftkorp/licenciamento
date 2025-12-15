using AutoMapper;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.UserBehaviour;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.Host.Mapper
{
    public class LicenseServerMapperProfile: Profile
    {
        public LicenseServerMapperProfile()
        {
            MapLicensesInput();
            MapLicenseStorage();
            MapLicensesTenantOutput();
        }

        private void MapLicensesInput()
        {
            CreateMap<ConsumeLicenseLegacyInput, ConsumeLicenseInput>();
            CreateMap<ReleaseLicenseLegacyInput, ReleaseLicenseInput>();
            CreateMap<RefreshLegacyAppLicenseInUseByUserInput, RefreshAppLicenseInUseByUserInputOld>();
        }

        private void MapLicensesTenantOutput()
        {
            CreateMap<LicenseByTenantIdOld, TenantLicenseDetailsOutput>();
        }

        private void MapLicenseStorage()
        {
            CreateMap<StoreDoneUsageLog, LicenseUsageBehaviourDetails>();
        }
    }
}