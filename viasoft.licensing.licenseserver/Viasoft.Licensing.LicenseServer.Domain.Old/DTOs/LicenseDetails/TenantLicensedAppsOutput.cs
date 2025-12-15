using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails
{
    public class TenantLicensedAppsOutput
    {
        public TenantLicensedAppsStatus Status { get; set; }

        public string StatusDescription => Status.ToString();

        public List<string> AppsIdentifiers { get; set; }
    }
}