using AutoMapper;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.UserBehaviour;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.UnitTest.Mock.Mapper
{
    public class LicenseServerMapperMockProfile: Profile
    {
        public LicenseServerMapperMockProfile()
        {
            MapLicenseStorage();
        }

        private void MapLicenseStorage()
        {
            CreateMap<StoreDoneUsageLog, LicenseUsageBehaviourDetails>();
        }
    }
}