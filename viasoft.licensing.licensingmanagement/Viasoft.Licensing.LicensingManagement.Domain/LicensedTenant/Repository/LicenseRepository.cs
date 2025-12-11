using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Extensions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Account;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.TenantSettings;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository
{
    public class LicenseRepository: ILicenseRepository, ITransientDependency
    {
        private readonly IRepository<LicensedApp> _licensedApps;
        private readonly IRepository<LicensedBundle> _licensedBundles;
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        private readonly IRepository<Bundle> _bundles;
        private readonly IRepository<App> _apps;
        private readonly IRepository<Software> _softwares;
        private readonly IRepository<Entities.Account> _accounts;
        private readonly IRepository<LicensedApp> _licensedApp;
        private readonly ILicensedTenantSettingsRepository _licensedTenantSettingsRepository;

        public LicenseRepository(IRepository<LicensedApp> licensedApps, IRepository<LicensedBundle> licensedBundles, IRepository<Entities.LicensedTenant> licensedTenants,
            IRepository<Bundle> bundles, IRepository<App> apps, IRepository<Software> softwares, IRepository<LicensedApp> licensedApp, IRepository<Entities.Account> accounts, 
            ILicensedTenantSettingsRepository licensedTenantSettingsRepository)
        {
            _licensedApps = licensedApps;
            _licensedBundles = licensedBundles;
            _licensedTenants = licensedTenants;
            _bundles = bundles;
            _apps = apps;
            _softwares = softwares;
            _licensedApp = licensedApp;
            _accounts = accounts;
            _licensedTenantSettingsRepository = licensedTenantSettingsRepository;
        }

        public Task<List<string>> GetLicensedAppsIdentifiersFromTenant(Guid tenantId)
        {
            return (from licensedTenant in _licensedTenants
                join licensedApp in _licensedApps on licensedTenant.Id equals licensedApp.LicensedTenantId
                join app in _apps on licensedApp.AppId equals app.Id
                where licensedTenant.Identifier == tenantId
                select app.Identifier).ToListAsync();
        }

        public async Task<bool> IsAppBeingUsedByLicense(Guid appId)
        {
            return await _licensedApps.AnyAsync(la => la.AppId == appId);
        }

        public async Task<bool> IsBundleBeingUsedByLicense(Guid bundleId)
        {
            return await _licensedBundles.AnyAsync(lb => lb.BundleId == bundleId);
        }

        public bool IsAccountBeingUsedByLicense(Guid accountId)
        {
            return _licensedTenants.Any(lt => lt.AccountId == accountId);
        }
        
        public async Task<LicensedTenantDetails> GetLicenseDetailsByIdentifier(Entities.LicensedTenant licensedTenantIdentifier)
        {
            return (await GetLicensedTenantsDetails(new List<Guid> { licensedTenantIdentifier.Identifier }))[0];
        }

        public async Task<List<LicensedTenantDetails>> GetLicenseDetailsByIdentifiers(List<Guid> licencedTenantIdentifiers)
        {
            return await GetLicensedTenantsDetails(licencedTenantIdentifiers);
        }

        public async Task<bool> CheckLicenseTenantIdExistence(Guid tenantId)
        {
            var licensedId = await _licensedTenants.Where(licensedTenant => licensedTenant.Identifier == tenantId)
                .Select(licensedTenant => licensedTenant.Id)
                .SingleOrDefaultAsync();

            return licensedId != Guid.Empty;
        }

        public async Task<Guid> GetIdentifierFromLicenseTenantIdExistence(Guid licensedTenantId)
        {
            var identifier = await _licensedTenants.Where(licensedTenant => licensedTenant.Id == licensedTenantId)
                .Select(licensedTenant => licensedTenant.Identifier)
                .SingleOrDefaultAsync();

            return identifier;
        }

        public async Task<List<LicensedApp>> GetAllLicensedAppsFromTenantId(Guid id)
        {
            return await _licensedApp.Where(app => app.LicensedTenantId == id).ToListAsync();
        }

        public async Task<List<LicensedBundle>> GetAllLicensedBundlesFromTenantId(Guid id)
        {
            return await _licensedBundles.Where(bundle => bundle.LicensedTenantId == id).ToListAsync();
        }

        public async Task<bool> CheckIfLicensedAppIsDefault(Guid licensedAppId)
        { 
            return await _apps.AnyAsync(app => app.Default && app.Id == licensedAppId);
        }

        public async Task<bool> CheckIfLicensedTenantExists(Guid id)
        {
            return await _licensedTenants.AnyAsync(tenant => tenant.Id == id);
        }
        
        public async Task<List<Guid>> GetAllLicensesForBatchOperations(string advancedFilter)
        {
            IQueryable<Entities.LicensedTenant> query;
            if (!string.IsNullOrEmpty(advancedFilter))
            {
                query = _licensedTenants.AsQueryable().ApplyAdvancedFilter(advancedFilter, null);
            }
            else
            {
                query = _licensedTenants.AsQueryable();
            }
            return await query.Select(l => l.Id).ToListAsync();
        }

        public async Task<List<Guid>> GetLicensesByIdsForBatchOperations(List<Guid> excludedLicenses, List<Guid> includedLicenses)
        {
            return await _licensedTenants
                .WhereIf(excludedLicenses.Any(), s => !excludedLicenses.Contains(s.Id))
                .WhereIf(includedLicenses.Any(), s => includedLicenses.Contains(s.Id))
                .Select(l => l.Id)
                .ToListAsync();
        }

        public Task<QuotaAppDetails> GetQuotaAppDetailsByAppIdentifier(Guid appId, Guid licensedTenantId)
        {
            return (from licensedApp in _licensedApps
                join licensedTenant in _licensedTenants on licensedApp.LicensedTenantId equals licensedTenant.Id
                join app in _apps on licensedApp.AppId equals app.Id
                where appId == licensedApp.AppId && licensedTenantId == licensedApp.LicensedTenantId
                    select  new QuotaAppDetails
                    {
                        Identifier = app.Identifier,
                        AppId = licensedApp.Id,
                        Name = app.Name,
                        LicencedTenantIdentifier = licensedTenant.Identifier
                    }
                ).FirstOrDefaultAsync();
        }
        
        private async Task<List<LicensedTenantDetails>> GetLicensedTenantsDetails(List<Guid> licencedTenantIdentifiers)
        {
            var output = new List<LicensedTenantDetails>();
            
            var licensedApps = await GetLicensedAppsByTenantIdentifiers(licencedTenantIdentifiers);
            var licensedBundles = await GetLicensedBundlesByTenantIdentifiers(licencedTenantIdentifiers);
            var accounts = await GetAccountsByTenantIdentifiers(licencedTenantIdentifiers);
            var licensedTenantsSettings = await GetLicensedTenantsSettings(licencedTenantIdentifiers);

            foreach (var licensedTenantIdentifier in licencedTenantIdentifiers)
            {
                 var ownedAppsForCurrentTenant = licensedApps.Where(l => l.LicensedTenantIdentifier == licensedTenantIdentifier)
                    .Select(l => new LicensedAppDetails
                    {
                        NumberOfLicenses = l.LicensedAppDetailsTemporaryLicenses.NumberOfLicenses + AvailableTemporaryLicenses(l.LicensedAppDetailsTemporaryLicenses.NumberOfTemporaryLicenses, l.LicensedAppDetailsTemporaryLicenses.ExpirationDateOfTemporaryLicenses),
                        Identifier = l.LicensedAppDetailsTemporaryLicenses.Identifier,
                        Name = l.LicensedAppDetailsTemporaryLicenses.Name,
                        Status = l.LicensedAppDetailsTemporaryLicenses.Status,
                        AppId = l.LicensedAppDetailsTemporaryLicenses.AppId,
                        SoftwareIdentifier = l.LicensedAppDetailsTemporaryLicenses.SoftwareIdentifier,
                        SoftwareName = l.LicensedAppDetailsTemporaryLicenses.SoftwareName,
                        LicensedBundleId = l.LicensedAppDetailsTemporaryLicenses.LicensedBundleId,
                        AdditionalNumberOfLicenses = l.LicensedAppDetailsTemporaryLicenses.AdditionalNumberOfLicenses,
                        LicensingModel = l.LicensedAppDetailsTemporaryLicenses.LicensingModel,
                        LicensingMode = l.LicensedAppDetailsTemporaryLicenses.LicensingMode
                    }).ToList();

                var ownedBundlesForCurrentTenant = licensedBundles
                    .Where(l => l.LicensedTenantIdentifier == licensedTenantIdentifier)
                    .Select(l => new LicensedBundleDetails
                    {
                        Identifier = l.LicensedBundleDetailsTemporaryLicenses.Identifier,
                        Name = l.LicensedBundleDetailsTemporaryLicenses.Name,
                        Status = l.LicensedBundleDetailsTemporaryLicenses.Status,
                        BundleId = l.LicensedBundleDetailsTemporaryLicenses.BundleId,
                        LicensingModel = l.LicensedBundleDetailsTemporaryLicenses.LicensingModel,
                        LicensingMode = l.LicensedBundleDetailsTemporaryLicenses.LicensingMode,
                        NumberOfLicenses = l.LicensedBundleDetailsTemporaryLicenses.NumberOfLicenses + AvailableTemporaryLicenses(l.LicensedBundleDetailsTemporaryLicenses.NumberOfTemporaryLicenses, l.LicensedBundleDetailsTemporaryLicenses.ExpirationDateOfTemporaryLicenses)
                    })
                    .ToList();

                var accountByCurrentTenant = accounts.FirstOrDefault(a => a.LicensedTenantIdentifier == licensedTenantIdentifier)?
                    .LicensedAccountDetails;
                var licensedTenantSettings = licensedTenantsSettings.FirstOrDefault(settings => settings.LicensingIdentifier == licensedTenantIdentifier);
                
                output.Add(new LicensedTenantDetails(ownedBundlesForCurrentTenant, ownedAppsForCurrentTenant, licensedTenantSettings)
                {
                    LicenseIdentifier = licensedTenantIdentifier,
                    AccountDetails = accountByCurrentTenant
                });
            }

            return output;
        }

        private async Task<List<LicensedTenantSettingsOutput>> GetLicensedTenantsSettings(List<Guid> licencedTenantIdentifiers)
        {
            var licensedTenantsSettingsQuery = await _licensedTenantSettingsRepository.GetLicensedTenantSettings(licencedTenantIdentifiers);
            var licensedTenantsSettings = await licensedTenantsSettingsQuery.AsNoTracking()
                .Select(settings => new LicensedTenantSettingsOutput(settings))
                .ToListAsync();

            return licensedTenantsSettings;
        }

        private Task<List<LicensedAppDetailsByTenant>> GetLicensedAppsByTenantIdentifiers(List<Guid> licencedTenantIdentifiers)
        {
            return (from licensedTenant in _licensedTenants
                    join licensedApp in _licensedApps on licensedTenant.Id equals licensedApp.LicensedTenantId
                    join app in _apps on licensedApp.AppId equals app.Id
                    join software in _softwares on app.SoftwareId equals software.Id
                    where licencedTenantIdentifiers.Contains(licensedTenant.Identifier)        
                    select new LicensedAppDetailsByTenant
                    {
                        LicensedTenantIdentifier = licensedTenant.Identifier,
                        LicensedAppDetailsTemporaryLicenses = new LicensedAppDetailsTemporaryLicenses
                        {
                            LicensedBundleId = licensedApp.LicensedBundleId,
                            Name = app.Name,
                            Identifier = app.Identifier,
                            Status = licensedApp.Status,
                            NumberOfLicenses = licensedApp.NumberOfLicenses,
                            AdditionalNumberOfLicenses = licensedApp.AdditionalNumberOfLicenses,
                            AppId = licensedApp.Id,
                            SoftwareIdentifier = software.Identifier,
                            SoftwareName = software.Name,
                            NumberOfTemporaryLicenses = licensedApp.NumberOfTemporaryLicenses,
                            ExpirationDateOfTemporaryLicenses = licensedApp.ExpirationDateOfTemporaryLicenses,
                            LicensingModel = licensedApp.LicensingModel,
                            LicensingMode = licensedApp.LicensingMode
                        }
                    }
                ).ToListAsync();
        }
        
        private async Task<List<LicensedBundleDetailsByTenant>> GetLicensedBundlesByTenantIdentifiers(List<Guid> licencedTenantIdentifiers)
        {
            return await (from licensedTenant in _licensedTenants
                    join licensedBundle in _licensedBundles on licensedTenant.Id equals licensedBundle.LicensedTenantId
                    join bundle in _bundles on licensedBundle.BundleId equals bundle.Id
                    where licencedTenantIdentifiers.Contains(licensedTenant.Identifier)      
                    select new LicensedBundleDetailsByTenant
                    {
                        LicensedTenantIdentifier = licensedTenant.Identifier,
                        LicensedBundleDetailsTemporaryLicenses = new LicensedBundleDetailsTemporaryLicenses
                        {
                            BundleId = licensedBundle.BundleId,
                            Identifier = bundle.Identifier,
                            Name = bundle.Name,
                            Status = licensedBundle.Status,
                            NumberOfLicenses = licensedBundle.NumberOfLicenses,
                            NumberOfTemporaryLicenses = licensedBundle.NumberOfTemporaryLicenses,
                            ExpirationDateOfTemporaryLicenses = licensedBundle.ExpirationDateOfTemporaryLicenses,
                            LicensingModel = licensedBundle.LicensingModel,
                            LicensingMode = licensedBundle.LicensingMode
                        }
                    }
                ).ToListAsync();
        }
        
        //private async Task

        private async Task<List<LicensedAccountDetailsByTenant>> GetAccountsByTenantIdentifiers(List<Guid> licencedTenantIdentifiers)
        {
            return await (from licensedTenant in _licensedTenants
                join account in _accounts on licensedTenant.AccountId equals account.Id
                where licencedTenantIdentifiers.Contains(licensedTenant.Identifier)
                select new LicensedAccountDetailsByTenant
                {
                    LicensedTenantIdentifier = licensedTenant.Identifier,
                    LicensedAccountDetails = new AccountDetails
                    {
                        Email = account.Email,
                        Phone = account.Phone,
                        Status = account.Status,
                        CnpjCpf = account.CnpjCpf,
                        CompanyName = account.CompanyName,
                        TradingName = account.TradingName,
                        WebSite = account.WebSite
                    }
                }).ToListAsync();
        }

        private int AvailableTemporaryLicenses(int numberOfTemporaryLicenses, DateTime? expirationDateOfTemporaryLicenses)
        {
            if (numberOfTemporaryLicenses > 0 && expirationDateOfTemporaryLicenses.HasValue)
            {
                if (expirationDateOfTemporaryLicenses.Value.Date >= DateTime.UtcNow.Date)
                {
                    return numberOfTemporaryLicenses;
                }
            }
            return 0;
        }
    }
}