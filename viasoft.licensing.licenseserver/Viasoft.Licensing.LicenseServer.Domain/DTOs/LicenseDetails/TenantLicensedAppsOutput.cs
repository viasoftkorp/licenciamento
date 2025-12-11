using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Domain.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseDetails
{
    public class TenantLicensedAppsOutput
    {
        public TenantLicensedAppsStatus Status { get; set; }

        public string StatusDescription => Status.ToString();

        public List<string> AppsIdentifiers { get; set; }
    }
}