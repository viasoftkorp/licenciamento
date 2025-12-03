using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.UserBehaviour;
using Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.TenantLicensing;
using Viasoft.Licensing.LicenseServer.Domain.EntityFrameworkCore;

namespace Viasoft.Licensing.LicenseServer.Domain.Repositories;

public class LicenseServerRepository : ILicenseServerRepository
{
    private readonly ILicenseServerDbContextFactory _dbContextFactory;

    public LicenseServerRepository(ILicenseServerDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<TenantLicenseStatusRefreshInfo> GetLastRefreshInfo(Guid tenantId)
    {
        await using var dbContext = await _dbContextFactory.ConstructedDbContext(tenantId);
        
        return await dbContext.TenantLicenseStatusRefreshInfos
            .FirstOrDefaultAsync(i => i.TenantId == tenantId);
    }

    public async Task StoreLastLicenseServerRefreshInfo(TenantLicenseStatusRefreshInfo input)
    {
        await using var dbContext = await _dbContextFactory.ConstructedDbContext(input.TenantId);
        
        await DoStoreLastLicenseServerRefreshInfo(input, dbContext);
        
        await dbContext.SaveChangesAsync();
    }

    public async Task StoreLicenseByTenantId(TenantLicensesCache input)
    {
        await using var dbContext = await _dbContextFactory.ConstructedDbContext(input.Id);

        var tenantLicenseCache = await dbContext.TenantLicensesCaches
            .AsTracking()
            .FirstOrDefaultAsync(i => i.Id == input.Id);
        
        if (tenantLicenseCache == null)
        {
            var tenantLicensesCacheToPersist = new PersistedTenantLicensesCache(input);
            await dbContext.TenantLicensesCaches.AddAsync(tenantLicensesCacheToPersist);
        }
        else
        {
            tenantLicenseCache.Update(input);
        }
        
        await DoStoreLastLicenseServerRefreshInfo(TenantLicenseStatusRefreshInfo.RefreshOk(input.TenantLicenses.LicensedTenant.Identifier), dbContext);
        
        await dbContext.SaveChangesAsync();
    }

    public async Task<TenantLicensesCache> GetLicenseByTenantId(Guid tenantId)
    {
        await using var dbContext = await _dbContextFactory.ConstructedDbContext(tenantId);
        var persistedTenantLicensesCache =  await dbContext.TenantLicensesCaches.FirstOrDefaultAsync(l => l.Id == tenantId);
        
        return persistedTenantLicensesCache is null ? null : new TenantLicensesCache(persistedTenantLicensesCache);
    }

    public async Task<LicenseUsageInRealTimeOutput> GetLastUploadedLicenseUsageInRealTime(Guid tenantId)
    {
        await using var dbContext = await _dbContextFactory.ConstructedDbContext(tenantId);
        var persistedLicenseUsage = await dbContext.LicenseUsageInRealTimeOutputs
            .Where(x => x.TenantId == tenantId)
            .FirstOrDefaultAsync();
        
        return persistedLicenseUsage is null ? null : new LicenseUsageInRealTimeOutput(persistedLicenseUsage);
    }

    public async Task StoreLastUploadedLicenseUsageInRealTime(LicenseUsageInRealTimeOutput input)
    {
        await using var dbContext = await _dbContextFactory.ConstructedDbContext(input.TenantId);
        
        var licenseUsage = await dbContext.LicenseUsageInRealTimeOutputs
            .FirstOrDefaultAsync(i => i.TenantId == input.TenantId);
        
        if (licenseUsage == null)
        {
            var licenseUsageToPersist = new PersistedLicenseUsageInRealTimeOutput(input);
            await dbContext.LicenseUsageInRealTimeOutputs.AddAsync(licenseUsageToPersist);
        }
        else
        {
            licenseUsage.Update(input);
        }
        
        await dbContext.SaveChangesAsync();
    }

    public async Task StoreDoneUsageLog(LicenseUsageBehaviourDetails input)
    {
        await using var dbContext = await _dbContextFactory.ConstructedDbContext(input.TenantId);
        dbContext.LicenseUsageBehaviourDetails.Add(input);
        await dbContext.SaveChangesAsync();
    }
    
    private static async Task DoStoreLastLicenseServerRefreshInfo(TenantLicenseStatusRefreshInfo input, LicenseServerDbContext dbContext)
    {
        var refreshInfo = await dbContext.TenantLicenseStatusRefreshInfos
            .AsTracking()
            .FirstOrDefaultAsync(i => i.TenantId == input.TenantId);

        if (refreshInfo == null)
        {
            await dbContext.TenantLicenseStatusRefreshInfos.AddAsync(input);
        } 
        else
        {
            refreshInfo.Update(input); 
        }
    }
}