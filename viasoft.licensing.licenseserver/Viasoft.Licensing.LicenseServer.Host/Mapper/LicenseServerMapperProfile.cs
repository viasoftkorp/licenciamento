using AutoMapper;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;

namespace Viasoft.Licensing.LicenseServer.Host.Mapper
{
    public class LicenseServerMapperProfile: Profile
    {
        public LicenseServerMapperProfile()
        {
            MapLicensesInput();
        }

        private void MapLicensesInput()
        {
            CreateMap<ConsumeLicenseLegacyInput, ConsumeLicenseInput>();
        }
    }
}