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
using Viasoft.Licensing.LicensingManagement.Domain.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle
{
    public class BundleRepository: IBundleRepository, ITransientDependency
    {
        private readonly IRepository<Entities.Bundle> _bundles;
        private readonly IRepository<Entities.BundledApp> _bundledApps;
        private readonly IRepository<LicensedBundle> _licensedBundles;
        public BundleRepository(IRepository<Entities.Bundle> bundles, IRepository<Entities.BundledApp> bundledApps, IRepository<LicensedBundle> licensedBundles)
        {
            _bundles = bundles;
            _bundledApps = bundledApps;
            _licensedBundles = licensedBundles;
        }

        public Task<bool> IsSoftwareBeingUsedByBundles(Guid softwareId)
        {
            return _bundles.AnyAsync(b => b.SoftwareId == softwareId);
        }

        public Task<bool> IsAppBeingUsedByBundles(Guid appId)
        {
            return _bundledApps.AnyAsync(b => b.AppId == appId);
        }

        public Task<Dictionary<Guid, string>> GetBundleNamesFromIdList(List<Guid> ids)
        {
            return _bundles
                .Select(bundle => new {bundle.Id, bundle.Name})
                .Where(bundle => ids.Contains(bundle.Id))
                .ToDictionaryAsync(bundle => bundle.Id, bundle => bundle.Name);
        }
        
        public async Task<bool> CheckIfBundleIsLicensedInAnyLicensing(Guid bundleId)
        {
            return await _licensedBundles.AnyAsync(l => l.BundleId == bundleId);
        }
        
        public async Task<List<Guid>> GetBundlesIdsAlreadyLicensed(List<Guid> bundleIds)
        {
            return await _licensedBundles.Where(l => bundleIds.Contains(l.BundleId)).Select(l => l.BundleId).ToListAsync();
        }

        public async Task<List<LicensedBundle>> GetLicensedBundles(Guid bundleId)
        {
            return await _licensedBundles.Where(l => l.BundleId == bundleId).ToListAsync();
        }
        
        public async Task<List<BundlesGetForBatchOperations>> GetAllBundlesForBatchOperation(string advancedFilter)
        {
            IQueryable<Entities.Bundle> query;
            if (!string.IsNullOrEmpty(advancedFilter))
            {
                query = _bundles.AsQueryable().ApplyAdvancedFilter(advancedFilter, null);
            }
            else
            {
                query = _bundles.AsQueryable();
            }
            return await query.Where(b => b.IsActive).Select(b => new BundlesGetForBatchOperations {Id = b.Id, Identifier = b.Identifier}).ToListAsync();
        }
        
        public async Task<List<BundlesGetForBatchOperations>> GetBundlesByIdsForBatchOperations(List<Guid> excludedBundles, List<Guid> includedBundles)
        {
            return await _bundles
                .Where(b => b.IsActive)
                .WhereIf(excludedBundles.Any(), s => !excludedBundles.Contains(s.Id))
                .WhereIf(includedBundles.Any(), s => includedBundles.Contains(s.Id))
                .Select(b => new BundlesGetForBatchOperations {Id = b.Id, Identifier = b.Identifier})
                .ToListAsync();
        }
        
        public Task<List<Entities.BundledApp>> GetBundledApps(List<Guid> appIds, List<Guid> bundleIds)
        {
            return _bundledApps
                .Where(app => appIds.Contains(app.AppId) && bundleIds.Contains(app.BundleId))
                .ToListAsync();
        }
        
        public async Task<List<LicensedBundleGetForBatchOperation>> GetBundlesAlreadyInLicenses(List<Guid> bundlesId, List<Guid> licenseTenantIds)
        {
            return await _licensedBundles
                .Where(l => bundlesId.Contains(l.BundleId) && licenseTenantIds.Contains(l.LicensedTenantId))
                .Select(l => new LicensedBundleGetForBatchOperation { BundleId = l.BundleId, LicensedTenantId = l.LicensedTenantId})
                .ToListAsync();
        }
    }
}