using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Controllers.DTOs
{
    public class GetLicensedTenantLegacyControllerOutput
    {
        public LicenseByTenantId TenantDetails { get; set; }
        public LicensedTenantRefreshOutput Refresh { get; set; }
        public List<string> Databases { get; set; }
        public Dictionary<string, int> AppLicensesUsageCount { get; set; }
    }
}