using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.TenantInfo;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantInfo
{
    public interface ITenantInfoService
    {
        public Task<TenantInfoOutput> GetTenantInfoFromLicensingIdentifier(Guid identifier);
    }
}