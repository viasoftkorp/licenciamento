using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.FileQuota
{
    public interface IFileQuotaViewService
    {
        Task<FileAppQuotaView> GetAppQuota(Guid licensedTenantId, Guid appId);
        Task<FileTenantQuota> GetTenantQuota(Guid licenseTenantIdentifier);
        Task AddAppQuotaView(Guid licensedTenantId, Guid appId, string appName, long quotaLimit);
        Task UpdateAppQuotaView(Guid licensedTenantId, Guid appId, long quotaLimit);
        Task AddOrUpdateAppQuotaView(Guid licensedTenantId, Guid appId, string appName, long quotaLimit);
        Task AddOrUpdateTenantQuotaView(Guid licenseTenantId, long quotaLimit);
        Task DeleteAppQuota(Guid licensedTenantId, Guid appId);
        Task<bool> DoesAppQuotaExists(Guid licensedTenantId, Guid appId);
    }
}