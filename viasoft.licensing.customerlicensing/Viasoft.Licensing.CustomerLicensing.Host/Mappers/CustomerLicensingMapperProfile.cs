using AutoMapper;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageInRealTime;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.UserBehaviour;
using Viasoft.Licensing.CustomerLicensing.Domain.Entities;

namespace Viasoft.Licensing.CustomerLicensing.Host.Mappers
{
    public class CustomerLicensingMapperProfile: Profile
    {
        public CustomerLicensingMapperProfile()
        {
            CreateMap<LicenseUsageInRealTimeDetails, LicenseUsageInRealTime>()
                .ForMember(d => d.LicensingIdentifier, o => o.MapFrom(src => src.TenantId))
                .ForMember(d => d.BrowserInfo, o => o.MapFrom(a => a.LicenseUsageAdditionalInformation.BrowserInfo))
                .ForMember(d => d.HostName, o => o.MapFrom(a => a.LicenseUsageAdditionalInformation.HostName))
                .ForMember(d => d.HostUser, o => o.MapFrom(a => a.LicenseUsageAdditionalInformation.HostUser))
                .ForMember(d => d.Language, o => o.MapFrom(a => a.LicenseUsageAdditionalInformation.Language))
                .ForMember(d => d.LocalIpAddress, o => o.MapFrom(a => a.LicenseUsageAdditionalInformation.LocalIpAddress))
                .ForMember(d => d.OsInfo, o => o.MapFrom(a => a.LicenseUsageAdditionalInformation.OsInfo))
                .ForMember(d => d.SoftwareVersion, o => o.MapFrom(a => a.LicenseUsageAdditionalInformation.SoftwareVersion))
                .ForMember(d => d.DatabaseName, o => o.MapFrom(a => a.LicenseUsageAdditionalInformation.DatabaseName));

            CreateMap<LicenseUsageInRealTime, LicenseUserBehaviourOutput>();
            CreateMap<LicenseUsageInRealTime, InsertedLicenseUsageInRealTime>();
        }
    }
}
