using System;
using System.Threading.Tasks;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.TenantInfo;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.TenantInfo
{
    public interface ITenantInfoService
    {
        Task<TenantInfoOutput> GetTenantInfoFromId(Guid id);
    }
}