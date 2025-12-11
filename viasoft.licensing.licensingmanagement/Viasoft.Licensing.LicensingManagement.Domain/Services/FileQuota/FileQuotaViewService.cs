using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.FileQuota
{
    public class FileQuotaViewService: IFileQuotaViewService, ITransientDependency
    {
        private readonly IRepository<FileAppQuotaView> _appQuotaViewRepository;
        private readonly IRepository<FileTenantQuota> _tenantQuotaRepository;

        public FileQuotaViewService(IRepository<FileAppQuotaView> appQuotaViewRepository, IRepository<FileTenantQuota> tenantQuotaRepository)
        {
            _appQuotaViewRepository = appQuotaViewRepository;
            _tenantQuotaRepository = tenantQuotaRepository;
        }

        public async Task<FileAppQuotaView> GetAppQuota(Guid licensedTenantId, Guid appId)
        {
            var appQuota = await _appQuotaViewRepository
                .FirstOrDefaultAsync(q => q.AppId == appId && q.LicensedTenantId == licensedTenantId);
            return appQuota;
        }

        public async Task<FileTenantQuota> GetTenantQuota(Guid licenseTenantId)
        {
            var tenantQuota = await _tenantQuotaRepository.FirstOrDefaultAsync(q => q.LicenseTenantId == licenseTenantId);
            return tenantQuota;
        }

        public async Task AddAppQuotaView(Guid licensedTenantId, Guid appId, string appName, long quotaLimit)
        {
            var quota = new FileAppQuotaView
            {
                LicensedTenantId = licensedTenantId,
                AppId = appId,
                AppName = appName,
                QuotaLimit = quotaLimit
            };
            await _appQuotaViewRepository.InsertAsync(quota, true);
        }

        public async Task UpdateAppQuotaView(Guid licensedTenantId, Guid appId, long quotaLimit)
        {
            var existingAppQuotaView = await GetAppQuota(licensedTenantId, appId);
            if (existingAppQuotaView != null)
            {
                existingAppQuotaView.QuotaLimit = quotaLimit;
                await _appQuotaViewRepository.UpdateAsync(existingAppQuotaView, true);
            }
        }

        public async Task AddOrUpdateAppQuotaView(Guid licensedTenantId, Guid appId, string appName, long quotaLimit)
        {
            var existingAppQuotaView = await GetAppQuota(licensedTenantId, appId);
            if (existingAppQuotaView != null)
                await UpdateAppQuotaView(licensedTenantId, appId, quotaLimit);
            else
                await AddAppQuotaView(licensedTenantId, appId, appName, quotaLimit);
        }

        public async Task AddOrUpdateTenantQuotaView(Guid licenseTenantId, long quotaLimit)
        {
            var existingTenantQuotaView = await GetTenantQuota(licenseTenantId);
            if (existingTenantQuotaView != null)
            {
                existingTenantQuotaView.QuotaLimit = quotaLimit;
                await _tenantQuotaRepository.UpdateAsync(existingTenantQuotaView, true);
            }
            else
            {
                var quota = new FileTenantQuota
                {
                    LicenseTenantId = licenseTenantId,
                    QuotaLimit = quotaLimit
                };
                await _tenantQuotaRepository.InsertAsync(quota, true);
            }
        }

        public async Task DeleteAppQuota(Guid licensedTenantId, Guid appId)
        {
            var appQuota = await GetAppQuota(licensedTenantId, appId);
            if (appQuota != null)
                await _appQuotaViewRepository.DeleteAsync(appQuota);
        }

        public async Task<bool> DoesAppQuotaExists(Guid licensedTenantId, Guid appId)
        {
            return await _appQuotaViewRepository
                .AnyAsync(q => q.AppId == appId && q.LicensedTenantId == licensedTenantId);
        }
    }
}