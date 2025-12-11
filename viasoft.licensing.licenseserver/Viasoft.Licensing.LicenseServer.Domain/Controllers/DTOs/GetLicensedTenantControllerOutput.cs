using Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Controllers.DTOs
{
    public class GetLicensedTenantControllerOutput
    {
        public LicenseByTenantId TenantDetails { get; set; }
    }
}