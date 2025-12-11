using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.FileQuota;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.FileQuota
{
    public interface IFileQuotaCallerService
    {
        Task<FileTenantQuota> AddOrUpdateFileTenantQuota(Guid tenantId, long quotaLimit);
        
        Task<FileAppQuota> AddOrUpdateFileAppQuota(Guid tenantId, string appId, long quotaLimit);
        
        Task DeleteAppQuota(Guid tenantId, string appId);
    }
}