using System;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.UserBehaviour;
using Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.TenantLicensing;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Initializer;

namespace Viasoft.Licensing.LicenseServer.Domain.Repositories;

public class LicenseServerRepositoryOnPremise : ILicenseServerRepository
{
    public Task<TenantLicenseStatusRefreshInfo> GetLastRefreshInfo(Guid tenantId)
    {
        using var db = LiteDbInitializer.NewReadonlyRepository(tenantId);
        var refreshInfo = db.FirstOrDefault<TenantLicenseStatusRefreshInfo>(l => l.TenantId == tenantId);
        return Task.FromResult(refreshInfo);
    }

    public Task StoreLastLicenseServerRefreshInfo(TenantLicenseStatusRefreshInfo input)
    {
        using var db = LiteDbInitializer.NewDatabase(LiteDbConsts.BuildTenantConnectionString(input.TenantId));
        return DoStoreLastLicenseServerRefreshInfo(input, db);
    }

    public Task StoreLicenseByTenantId(TenantLicensesCache input)
    {
        using var db = LiteDbInitializer.NewDatabase(LiteDbConsts.BuildTenantConnectionString(input.Id));
        
        DoStoreLastLicenseServerRefreshInfo(TenantLicenseStatusRefreshInfo.RefreshOk(input.TenantLicenses.LicensedTenant.Identifier), db);
        var licenseByTenantIdCollection = db.GetCollection<TenantLicensesCache>(nameof(TenantLicensesCache));
        
        return Task.FromResult(licenseByTenantIdCollection.Upsert(input.Id, input));
    }

    public Task<TenantLicensesCache> GetLicenseByTenantId(Guid tenantId)
    {
        using var db = LiteDbInitializer.NewReadonlyRepository(tenantId);
        return Task.FromResult(db.FirstOrDefault<TenantLicensesCache>(l => l.Id == tenantId));
    }

    public Task<LicenseUsageInRealTimeOutput> GetLastUploadedLicenseUsageInRealTime(Guid tenantId)
    {
        using var db = LiteDbInitializer.NewReadonlyRepository(tenantId);
        return Task.FromResult(db.Fetch<LicenseUsageInRealTimeOutput>(x => x.TenantId == tenantId).FirstOrDefault());
    }

    public Task StoreLastUploadedLicenseUsageInRealTime(LicenseUsageInRealTimeOutput input)
    {
        using var db = LiteDbInitializer.NewDatabase(LiteDbConsts.BuildTenantConnectionString(input.TenantId));
        var licenseByTenantIdCollection = db.GetCollection<LicenseUsageInRealTimeOutput>(nameof(LicenseUsageInRealTimeOutput));
        return Task.FromResult(licenseByTenantIdCollection.Upsert(input.TenantId, input));
    }

    public Task StoreDoneUsageLog(LicenseUsageBehaviourDetails input)
    {
        using var db = LiteDbInitializer.NewDatabase(LiteDbConsts.BuildTenantConnectionString(input.TenantId));
        var licenseUsages = db.GetCollection<LicenseUsageBehaviourDetails>(nameof(LicenseUsageBehaviourDetails));
        licenseUsages.EnsureIndex(x => x.Id);
        return Task.FromResult(licenseUsages.Insert(input));
    }
    
    private static Task DoStoreLastLicenseServerRefreshInfo(TenantLicenseStatusRefreshInfo input, ILiteDatabase db)
    {
        var tenantLicenseStatusLastConnectionWithServerCollection = db.GetCollection<TenantLicenseStatusRefreshInfo>(nameof(TenantLicenseStatusRefreshInfo));
        return Task.FromResult(tenantLicenseStatusLastConnectionWithServerCollection.Upsert(input.TenantId, input));
    }
}