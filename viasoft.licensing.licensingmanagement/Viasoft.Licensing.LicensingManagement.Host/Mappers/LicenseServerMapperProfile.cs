using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Viasoft.Licensing.LicensingManagement.Domain.Account.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.AppMessages;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.App;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.AuditingLog;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Company;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSaga;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Software;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.ZipCode;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.DTO;

namespace Viasoft.Licensing.LicensingManagement.Host.Mappers
{
    public class LicenseServerMapperProfile: Profile
    {
        public LicenseServerMapperProfile()
        {
            MapSoftware();
            MapBundle();
            MapApp();
            MapLicensedTenant();
            MapAccount();
            MapZipCode();
            MapCnpjSearch();
            MapAuditing();
            MapInfrastructureConfiguration();
        }

        private void MapCnpjSearch()
        {
            CreateMap<CompanyDto, GetCompanyByCnpjOutput>();
        }

        private void MapAccount()
        {
            CreateMap<AccountCreateInput, Account>();
            CreateMap<Account, AccountCreateOutput>();
            CreateMap<AccountUpdateInput, Account>();
            CreateMap<Account, AccountUpdateOutput>();
        }

        private void MapSoftware()
        {
            CreateMap<SoftwareCreateInput, Software>();
            CreateMap<SoftwareCreateInput, SoftwareCreateOutput>();
            CreateMap<Software, SoftwareCreateOutput>();
            CreateMap<SoftwareUpdateInput, Software>();
            CreateMap<SoftwareUpdateInput, SoftwareUpdateOutput>();
            CreateMap<Software, SoftwareUpdateOutput>();
        }
        
        private void MapBundle()
        {
            CreateMap<BundleCreateInput, Bundle>();
            CreateMap<BundleCreateInput, BundleCreateOutput>();
            CreateMap<Bundle, BundleCreateOutput>();
            CreateMap<BundleUpdateInput, Bundle>();
            CreateMap<BundleUpdateInput, BundleUpdateOutput>();
            CreateMap<Bundle, BundleUpdateOutput>();
            CreateMap<LicensedBundleCreateInput, LicensedBundle>();
            CreateMap<LicensedBundle, LicensedBundleCreateOutput>();
            CreateMap<LicensedBundleCreateInput, LicensedBundleCreateOutput>();
            CreateMap<LicensedBundleUpdateInput, LicensedBundle>();
            CreateMap<LicensedBundle, LicensedBundleUpdateOutput>();
            CreateMap<LicensedBundleUpdateInput, LicensedBundleUpdateOutput>();
        }

        private void MapApp()
        {
            CreateMap<AppCreateInput, App>().ForMember(e => e.Default, 
                option => option.MapFrom(input => input.IsDefault));
            CreateMap<AppCreateInput, AppCreateOutput>();
            CreateMap<App, AppCreateOutput>().ForMember(e => e.IsDefault,
                option => option.MapFrom(input => input.Default));
            CreateMap<AppUpdateInput, App>().ForMember(e => e.Default, 
                option => option.MapFrom(input => input.IsDefault));
            CreateMap<AppUpdateInput, AppUpdateOutput>();
            CreateMap<App, AppUpdateOutput>().ForMember(e => e.IsDefault,
                option => option.MapFrom(input => input.Default));
            CreateMap<App, AppUpdatedMessage>();
            CreateMap<BundledAppCreateInput, BundledApp>();
            CreateMap<BundledApp, BundledAppCreateOutput>();
            CreateMap<BundledAppCreateInput, BundledAppCreateOutput>();
            CreateMap<LicensedAppCreateInput, LicensedApp>();
            CreateMap<LicensedApp, LicensedAppCreateOutput>();
            CreateMap<LicensedAppCreateInput, LicensedAppCreateOutput>();
            CreateMap<LicensedAppUpdateInput, LicensedAppUpdateOutput>();
            CreateMap<LicensedAppUpdateInput, LicensedApp>();
            CreateMap<LicensedApp, LicensedAppUpdateOutput>();
        }

        private void MapLicensedTenant()
        {
            CreateMap<LicenseTenantCreateInput, LicensedTenant>().ForMember(entity => entity.Notes,
            notes => notes.MapFrom(notesByte => Encoding.UTF8.GetBytes(notesByte.Notes))) ;
            CreateMap<LicenseTenantCreateInput, LicenseTenantCreateOutput>();
            
            CreateMap<LicensedTenant, LicenseTenantCreateOutput>()
                .ForMember(output => output.Notes,
            notes => notes.MapFrom(notesString => 
                Encoding.UTF8.GetString(notesString.Notes)))
                .ForMember(output => output.SagaInfo,
                    map => map.MapFrom(licensedTenant =>
                        JsonSerializer.Deserialize<LicensedTenantSagaInfo>(
                            Encoding.UTF8.GetString(licensedTenant.SagaInfo), new JsonSerializerOptions(JsonSerializerDefaults.Web))));
            
            CreateMap<LicenseTenantUpdateInput, LicensedTenant>().ForMember(entity => entity.Notes,
            notes => notes.MapFrom(notesByte => Encoding.UTF8.GetBytes(notesByte.Notes)));
            CreateMap<LicenseTenantUpdateInput, LicenseTenantUpdateOutput>();
            CreateMap<LicensedTenant, LicenseTenantUpdateOutput>().ForMember(output => output.Notes,
            notes => notes.MapFrom(notesString => Encoding.UTF8.GetString(notesString.Notes)));
            CreateMap<LicensedBundleCreateInput, LicensedApp>();

            CreateMap<LicensedTenantView, LicensedTenantViewOutput>();
            
            CreateMap<LicensedTenant, LicenseTenantUpdateInput>()
                .ForMember(output => output.Notes, notes => notes.MapFrom(notesString => Encoding.UTF8.GetString(notesString.Notes)));
        }

        private void MapZipCode()
        {
            CreateMap<ZipCodeResponseDto, ZipCodeAdressDto>();
        }
        
        private void MapAuditing()
        {
            CreateMap<AuditingLogOutput, AuditingLog>();
            CreateMap<AuditingLog, AuditingLogOutput>();
        }

        private void MapInfrastructureConfiguration()
        {
            CreateMap<InfrastructureConfigurationCreateInput, InfrastructureConfiguration>();
            CreateMap<InfrastructureConfiguration, InfrastructureConfigurationCreateOutput>();
            CreateMap<InfrastructureConfigurationUpdateInput, InfrastructureConfiguration>();
            CreateMap<InfrastructureConfiguration, InfrastructureConfigurationUpdateOutput>();
        }
    }
}