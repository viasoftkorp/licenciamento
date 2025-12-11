using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.UserBehaviour;
using Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.TenantLicensing;

namespace Viasoft.Licensing.LicenseServer.Domain.Repositories;

public interface ILicenseServerRepository
{
    public Task<TenantLicenseStatusRefreshInfo> GetLastRefreshInfo(Guid tenantId);
    public Task StoreLastLicenseServerRefreshInfo(TenantLicenseStatusRefreshInfo input);
    
    public Task StoreLicenseByTenantId(TenantLicensesCache input);
    public Task<TenantLicensesCache> GetLicenseByTenantId(Guid tenantId);

    public Task<LicenseUsageInRealTimeOutput> GetLastUploadedLicenseUsageInRealTime(Guid tenantId);
    public Task StoreLastUploadedLicenseUsageInRealTime(LicenseUsageInRealTimeOutput input);
    public Task StoreDoneUsageLog(LicenseUsageBehaviourDetails input);
}