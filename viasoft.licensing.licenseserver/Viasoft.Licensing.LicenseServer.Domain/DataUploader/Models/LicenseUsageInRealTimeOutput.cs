using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Viasoft.Licensing.LicenseServer.Domain.EntityFrameworkCore;

namespace Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models
{
    public class LicenseUsageInRealTimeOutput
    {

        public LicenseUsageInRealTimeOutput()
        {
            
        }

        public LicenseUsageInRealTimeOutput(LicenseUsageInRealTimeRawData rawData)
        {
            TenantId = rawData.TenantId;
            SoftwareUtilized = rawData.SoftwareUtilized;
            LicenseUsageInRealTimeDetails =
                rawData.LicenseUsageInRealTimeDetails.Select(l => new LicenseUsageInRealTimeDetailsOutput
                {
                    Cnpj = l.Cnpj,
                    User = l.User,
                    AdditionalLicense = l.AdditionalLicense,
                    AdditionalLicenses = l.AdditionalLicenses,
                    AppIdentifier = l.AppIdentifier,
                    AppLicenses = l.AppLicenses,
                    AppName = l.AppName,
                    AppStatus = l.AppStatus,
                    BundleIdentifier = l.BundleIdentifier,
                    BundleName = l.BundleName,
                    LicensingStatus = l.LicensingStatus,
                    SoftwareIdentifier = l.SoftwareIdentifier,
                    SoftwareName = l.SoftwareName,
                    StartTime = l.StartTime,
                    TenantId = l.TenantId,
                    AdditionalLicensesConsumed = l.AdditionalLicensesConsumed,
                    AppLicensesConsumed = l.AppLicensesConsumed,
                    LicensingModel = l.LicensingModel,
                    LicensingMode = l.LicensingMode,
                    LicenseUsageAdditionalInformation = new LicenseUsageAdditionalInformationOutput
                    {
                        Language = l.LicenseUsageAdditionalInformation?.Language,
                        BrowserInfo = l.LicenseUsageAdditionalInformation?.BrowserInfo,
                        DatabaseName = l.LicenseUsageAdditionalInformation?.DatabaseName,
                        HostName = l.LicenseUsageAdditionalInformation?.HostName,
                        HostUser = l.LicenseUsageAdditionalInformation?.HostUser,
                        OsInfo = l.LicenseUsageAdditionalInformation?.OsInfo,
                        SoftwareVersion = l.LicenseUsageAdditionalInformation?.SoftwareVersion,
                        LocalIpAddress = l.LicenseUsageAdditionalInformation?.LocalIpAddress
                    }
                }).ToList();
        }

        public LicenseUsageInRealTimeOutput(PersistedLicenseUsageInRealTimeOutput input)
        {
            TenantId = input.TenantId;
            SoftwareUtilized = JsonConvert.DeserializeObject<List<string>>(Encoding.UTF8.GetString(input.SoftwareUtilized));
            LicenseUsageInRealTimeDetails =
                JsonConvert.DeserializeObject<List<LicenseUsageInRealTimeDetailsOutput>>(Encoding.UTF8
                    .GetString(input.LicenseUsageInRealTimeDetails));
        }
        
        public Guid TenantId { get; set; }
        
        public List<string> SoftwareUtilized { get; set;}

        public List<LicenseUsageInRealTimeDetailsOutput> LicenseUsageInRealTimeDetails { get; set; }
    }
}