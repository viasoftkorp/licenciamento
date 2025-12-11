using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Extensions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.App
{
    public class AppRepository: IAppRepository, ITransientDependency
    {
        private readonly IRepository<Entities.App> _apps;
        private readonly IRepository<LicensedApp> _licensedApp;

        public AppRepository(IRepository<Entities.App> apps, IRepository<LicensedApp> licensedApp)
        {
            _apps = apps;
            _licensedApp = licensedApp;
        }

        public Task<bool> IsSoftwareBeingUsedByApps(Guid appId)
        {
            return _apps.AnyAsync(a => a.SoftwareId == appId);
        }
        
        public Task<List<Entities.App>> GetAllDefaultApps()
        {
            return _apps.Where(app => app.Default).ToListAsync();
        }
        public async Task<List<AppsGetForBatchOperations>> GetAppIdentifiersByAppIds(List<Guid> appIds)
        {
            var appIdentifiers = await _apps.Where(app => appIds.Contains(app.Id)).Select(l => new AppsGetForBatchOperations { Id = l.Id, Identifier = l.Identifier}).ToListAsync();
            return appIdentifiers;
        }

        public async Task<Dictionary<string, Enums.Domain>> GetDomainsByAppIdentifiers(List<string> appIds)
        {
            return await _apps.Select(a => new
                {
                    a.Identifier,
                    a.Domain
                }).Where(a => appIds.Contains(a.Identifier)).ToDictionaryAsync(k => k.Identifier, v => v.Domain);
        }

        public async Task<List<LicensedApp>> GetLicensedAppsForLicenses(List<Guid> licensedTenantsId, Guid appId)
        {
            return await _licensedApp.Where(la =>  licensedTenantsId.Contains(la.LicensedTenantId) && la.AppId == appId).ToListAsync();
        }
        
        public Task<List<AlreadyLicensedApp>> GetAppsAlreadyLicensed(List<Guid> appId, List<Guid> licensedTenantIds)
        {
            return _licensedApp
                .Where(app => appId.Contains(app.AppId) && licensedTenantIds.Contains(app.LicensedTenantId))
                .Select(l => new AlreadyLicensedApp { AppId = l.AppId, LicensedTenantId = l.LicensedTenantId})
                .ToListAsync();
        }

        public async Task<List<AppsGetForBatchOperations>> GetAppsByIdsForBatchOperations(List<Guid> excludedApps, List<Guid> includedApps)
        {
            return await _apps
                .Where(b => b.IsActive)
                .WhereIf(excludedApps.Any(), s => !excludedApps.Contains(s.Id))
                .WhereIf(includedApps.Any(), s => includedApps.Contains(s.Id))
                .Select(b => new AppsGetForBatchOperations {Id = b.Id, Identifier = b.Identifier})
                .ToListAsync();
        }

        public async Task<List<AppsGetForBatchOperations>> GetAllAppsForBatchOperation(string advancedFilter)
        {
            IQueryable<Entities.App> query;
            if (!string.IsNullOrEmpty(advancedFilter))
            {
                query = _apps.AsQueryable().ApplyAdvancedFilter(advancedFilter, null);
            }
            else
            {
                query = _apps.AsQueryable();
            }
            return await query.Where(b => b.IsActive).Select(b => new AppsGetForBatchOperations {Id = b.Id, Identifier = b.Identifier}).ToListAsync();
        }
    }
}