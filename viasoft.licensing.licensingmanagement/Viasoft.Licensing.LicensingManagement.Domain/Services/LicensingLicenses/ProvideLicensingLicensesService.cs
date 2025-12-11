using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Account.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensingLicenses;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.OwnedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.OwnedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.LicensingLicenses
{
    public class ProvideLicensingLicensesService: IProvideLicensingLicensesService, ITransientDependency
    {
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        private readonly IRepository<Entities.Account> _accounts;
        private readonly IRepository<Entities.LicensedApp> _licensedApps;
        private readonly IRepository<Entities.LicensedBundle> _licensedBundles;
        private readonly IRepository<Bundle> _bundles;
        private readonly IRepository<Software> _softwares;
        private readonly IRepository<App> _apps;
        private readonly IRepository<Entities.NamedUserAppLicense> _namedUserAppLicenses;
        private readonly IRepository<Entities.NamedUserBundleLicense> _namedUserBundleLicenses;
        private readonly IRepository<Entities.LicensedTenantSettings> _licensedTenantSettings;
        private readonly ILicensedTenantService _licensedTenantService;
        private readonly ILogger<ProvideLicensingLicensesService> _logger;

        public ProvideLicensingLicensesService(IRepository<Entities.LicensedTenant> licensedTenants, IRepository<Entities.Account> accounts, 
            IRepository<Entities.LicensedApp> licensedApps, IRepository<Entities.LicensedBundle> licensedBundles, IRepository<Bundle> bundles, 
            IRepository<Software> softwares, IRepository<App> apps, IRepository<Entities.NamedUserAppLicense> namedUserAppLicenses, 
        IRepository<Entities.NamedUserBundleLicense> namedUserBundleLicenses, IRepository<Entities.LicensedTenantSettings> licensedTenantSettings, 
            ILicensedTenantService licensedTenantService, ILogger<ProvideLicensingLicensesService> logger)
        {
            _licensedTenants = licensedTenants;
            _accounts = accounts;
            _licensedApps = licensedApps;
            _licensedBundles = licensedBundles;
            _bundles = bundles;
            _softwares = softwares;
            _apps = apps;
            _namedUserAppLicenses = namedUserAppLicenses;
            _namedUserBundleLicenses = namedUserBundleLicenses;
            _licensedTenantSettings = licensedTenantSettings;
            _licensedTenantService = licensedTenantService;
            _logger = logger;
        }

        public async Task<LicensesOutput> GetLicensingLicenses(Guid identifier)
        {
            var licensing = await GetLicensing(identifier);
            if (licensing is null)
            {
                return null;
            }

            await SetProducts(licensing);
            await SetApps(licensing);

            return licensing;
        }
        
        public async Task<UpdateHardwareIdOutput> UpdateHardwareId(Guid identifier, UpdateHardwareIdInput input)
        {
            if (string.IsNullOrEmpty(input.HardwareId))
            {
                return new UpdateHardwareIdOutput
                {
                    IsSuccess = false,
                    Code = UpdateHardwareIdEnum.InputHardwareIdEmpty
                };
            }
            
            var entity = await _licensedTenants.AsNoTracking()
                .FirstOrDefaultAsync(lt => lt.Identifier == identifier);

            if (entity == null)
            {
                return new UpdateHardwareIdOutput
                {
                    IsSuccess = false,
                    Code = UpdateHardwareIdEnum.CouldNotFindEntity
                };
            }

            if (!string.IsNullOrEmpty(entity.HardwareId))
            {
                return new UpdateHardwareIdOutput
                {
                    IsSuccess = false,
                    Code = UpdateHardwareIdEnum.EntityHardwareIdNotEmpty
                };
            }

            var licenseTenantUpdateInput = new LicenseTenantUpdateInput
            {
                Id = entity.Id,
                AccountId = entity.AccountId,
                Status = entity.Status,
                Identifier = entity.Identifier,
                ExpirationDateTime = entity.ExpirationDateTime,
                LicenseConsumeType = entity.LicenseConsumeType,
                LicensedCnpjs = entity.LicensedCnpjs,
                AdministratorEmail = entity.AdministratorEmail,
                Notes = entity.NotesString,
                HardwareId = input.HardwareId
            };

            var updateOutput = await _licensedTenantService.UpdateTenantLicensing(licenseTenantUpdateInput);
            if (updateOutput.OperationValidation != OperationValidation.NoError)
            {
                _logger.LogWarning("update hardware id for tenant {tenant} failed with status {status}", identifier, input.HardwareId);
                
                return new UpdateHardwareIdOutput
                {
                    IsSuccess = false,
                    Code = UpdateHardwareIdEnum.CouldNotFindEntity //não é o retorno correto, mas para não quebrar os license server deixamos assim
                };
            }

            return new UpdateHardwareIdOutput
            {
                IsSuccess = true
            };
        }

        private async Task SetApps(LicensesOutput licensing)
        {
            var (licensedApps, apps, softwares, namedAppLicenses) = await GetAppRelatedEntities(licensing.LicensedTenant.Id);

            var joinedApps = licensedApps.Join(apps, licensedApp => licensedApp.AppId, app => app.Id,
                (licensedApp, app) => new
                {
                    app.Identifier,
                    app.Name,
                    licensedApp.Status,
                    licensedApp.AppId,
                    licensedApp.TenantId,
                    licensedApp.LicensedBundleId,
                    licensedApp.LicensedTenantId,
                    licensedApp.NumberOfLicenses,
                    app.SoftwareId,
                    licensedApp.AdditionalNumberOfLicenses,
                    licensedApp.NumberOfTemporaryLicenses,
                    licensedApp.ExpirationDateOfTemporaryLicenses,
                    licensedApp.LicensingModel,
                    licensedApp.LicensingMode,
                    licensedApp.Id
                });

            licensing.OwnedApps = joinedApps.Join(softwares, arg => arg.SoftwareId, software => software.Id, (ownedApp, software) => new OwnedAppOutput
            {
                Identifier = ownedApp.Identifier,
                Name = ownedApp.Name,
                Status = ownedApp.Status,
                AppId = ownedApp.AppId,
                SoftwareIdentifier = software.Identifier,
                SoftwareName = software.Name,
                TenantId = ownedApp.TenantId,
                LicensedBundleId = ownedApp.LicensedBundleId,
                LicensedTenantId = ownedApp.LicensedTenantId,
                NumberOfLicenses = ownedApp.NumberOfLicenses,
                AdditionalNumberOfLicenses = ownedApp.AdditionalNumberOfLicenses,
                NumberOfTemporaryLicenses = ownedApp.NumberOfTemporaryLicenses,
                ExpirationDateOfTemporaryLicenses = ownedApp.ExpirationDateOfTemporaryLicenses,
                LicensingMode = ownedApp.LicensingMode,
                LicensingModel = ownedApp.LicensingModel,
                LicensedAppId = ownedApp.Id
            }).ToList();

            licensing.NamedUserAppLicenses = namedAppLicenses.Select(a => new NamedUserAppLicenseOutput
            {
                Id = a.Id,
                DeviceId = a.DeviceId,
                TenantId = a.TenantId,
                LicensedAppId = a.LicensedAppId,
                LicensedTenantId = a.LicensedTenantId,
                NamedUserEmail = a.NamedUserEmail,
                NamedUserId = a.NamedUserId
            }).ToList();
        }

        private async Task SetProducts(LicensesOutput licensing)
        {
            var (bundles, licensedBundles, softwares, namedBundleLicenses) = await GetBundleRelatedEntities(licensing.LicensedTenant.Id);
            licensing.OwnedBundles = bundles.Select(bundle => new OwnedBundleOutput
            {
                BundleId = bundle.Key,
                Name = bundle.Value.Name,
                Identifier = bundle.Value.Identifier,
                IsActive = bundle.Value.IsActive,
                IsCustom = bundle.Value.IsCustom,
                SoftwareId = bundle.Value.SoftwareId,
                SoftwareName = softwares[bundle.Value.SoftwareId].Name,
                NumberOfLicenses = licensedBundles[bundle.Key].NumberOfLicenses,
                NumberOfTemporaryLicenses = licensedBundles[bundle.Key].NumberOfTemporaryLicenses,
                ExpirationDateOfTemporaryLicenses = licensedBundles[bundle.Key].ExpirationDateOfTemporaryLicenses,
                LicensedBundleId = licensedBundles[bundle.Key].Id,
                LicensingModel = licensedBundles[bundle.Key].LicensingModel,
                LicensingMode = licensedBundles[bundle.Key].LicensingMode
            }).ToList();
            
            licensing.NamedUserBundleLicenses = namedBundleLicenses.Select(b => new NamedUserBundleLicenseOutput
            {
                Id = b.Id,
                DeviceId = b.DeviceId,
                TenantId = b.TenantId,
                LicensedBundleId = b.LicensedBundleId,
                LicensedTenantId = b.LicensedTenantId,
                NamedUserEmail = b.NamedUserEmail,
                NamedUserId = b.NamedUserId
            }).ToList();
        }

        private async Task<(List<Entities.LicensedApp> LicensedApps, List<App> Apps, List<Software> Softwares, List<Entities.NamedUserAppLicense> NamedAppLicenses)> GetAppRelatedEntities(Guid licensedTenantId)
        {
            var licensedAppsQuery = from licensedApp in _licensedApps
                join app in _apps on licensedApp.AppId equals app.Id
                join appSoftware in _softwares on app.SoftwareId equals appSoftware.Id
                join namedUserAppLicense in _namedUserAppLicenses on licensedApp.Id equals 
                    namedUserAppLicense.LicensedAppId into namedUserAppLicenses
                from namedUserAppLicense in namedUserAppLicenses.DefaultIfEmpty()
                where licensedTenantId == licensedApp.LicensedTenantId
                select new
                {
                    LicensedApp = licensedApp,
                    App = app,
                    Software = appSoftware,
                    NamedUserAppLicense = namedUserAppLicense
                };
            var appRelatedEntities = await licensedAppsQuery.AsNoTracking().ToListAsync();

            var licensedApps = appRelatedEntities
                .Select(l => l.LicensedApp)
                .GroupBy(p => p.Id)
                .Select(grp => grp.First())
                .ToList();

            var apps = appRelatedEntities
                .Select(l => l.App)
                .GroupBy(p => p.Id)
                .Select(grp => grp.First())
                .ToList();

            var softwares = appRelatedEntities
                .Select(l => l.Software)
                .GroupBy(p => p.Id)
                .Select(grp => grp.First())
                .ToList();

            var namedAppLicenses = appRelatedEntities
                .Where(l => l.NamedUserAppLicense != null)
                .Select(l => l.NamedUserAppLicense)
                .GroupBy(p => p.Id)
                .Select(grp => grp.First())
                .ToList();

            return (licensedApps, apps, softwares, namedAppLicenses);
        }

        private async Task<LicensesOutput> GetLicensing(Guid identifier)
        {
            var licensingQuery = from tenant in _licensedTenants
                join account in _accounts on tenant.AccountId equals account.Id
                join licensedTenantSettings in _licensedTenantSettings
                    on new {tenant.TenantId, LicensingIdentifier = tenant.Identifier} 
                    equals new {licensedTenantSettings.TenantId, licensedTenantSettings.LicensingIdentifier}
                    into joinedLicensedTenantSettings
                from licensedTenantSettings in joinedLicensedTenantSettings
                where tenant.Identifier == identifier
                select new LicensesOutput
                {
                    LicensedTenantSettings = new LicensedTenantSettingsOutput
                    {
                        Id = licensedTenantSettings.Id,
                        TenantId = licensedTenantSettings.TenantId,
                        LicensingIdentifier = licensedTenantSettings.LicensingIdentifier,
                        Key = licensedTenantSettings.Key,
                        Value = licensedTenantSettings.Value
                    },
                    LicensedTenant = new LicensedTenantOutput
                    {
                        Id = tenant.Id,
                        Identifier = tenant.Identifier,
                        Notes = tenant.NotesString,
                        Status = tenant.Status,
                        AccountId = tenant.AccountId,
                        AccountName = account.CompanyName,
                        AdministratorEmail = tenant.AdministratorEmail,
                        HardwareId = tenant.HardwareId,
                        LicensedCnpjs = tenant.LicensedCnpjs,
                        ExpirationDateTime = tenant.ExpirationDateTime,
                        LicenseConsumeType = tenant.LicenseConsumeType
                    },
                    AccountDetails = new AccountOutput
                    {
                        City = account.City,
                        Country = account.Country,
                        Detail = account.Detail,
                        Email = account.Email,
                        Neighborhood = account.Neighborhood,
                        Number = account.Number,
                        Phone = account.Phone,
                        State = account.State,
                        Status = account.Status,
                        Street = account.Street,
                        BillingEmail = account.BillingEmail,
                        CnpjCpf = account.CnpjCpf,
                        CompanyName = account.CompanyName,
                        TenantId = account.TenantId,
                        TradingName = account.TradingName,
                        WebSite = account.WebSite,
                        ZipCode = account.ZipCode
                    }
                };

            return await licensingQuery.AsNoTracking()
                .FirstOrDefaultAsync();
        }

        private async Task<(Dictionary<Guid, Bundle> Bundles, Dictionary<Guid, Entities.LicensedBundle> LicensedBundles, Dictionary<Guid, Software> Softwares,
            List<Entities.NamedUserBundleLicense> NamedBundleLicenses)> GetBundleRelatedEntities(Guid licensedTenantId)
        {
            var licensedBundlesQuery = from licensedBundle in _licensedBundles
                join bundle in _bundles on licensedBundle.BundleId equals bundle.Id
                join bundleSoftware in _softwares on bundle.SoftwareId equals bundleSoftware.Id
                join namedUserBundleLicense in _namedUserBundleLicenses on licensedBundle.Id equals 
                    namedUserBundleLicense.LicensedBundleId into namedUserBundleLicenses
                from namedUserBundleLicense in namedUserBundleLicenses.DefaultIfEmpty()
                where licensedTenantId == licensedBundle.LicensedTenantId
                select new
                {
                    LicensedBundle = licensedBundle,
                    Bundle = bundle,
                    Software = bundleSoftware,
                    NamedUserBundle = namedUserBundleLicense
                };
            var bundleRelatedEntities = await licensedBundlesQuery.AsNoTracking().ToListAsync();

            var bundles = bundleRelatedEntities
                .Select(l => l.Bundle)
                .GroupBy(p => p.Id)
                .Select(grp => grp.First())
                .ToDictionary(bundle => bundle.Id);

            var licensedBundles = bundleRelatedEntities
                .Select(l => l.LicensedBundle)
                .GroupBy(p => p.BundleId)
                .Select(grp => grp.First())
                .ToDictionary(bundle => bundle.BundleId);

            var softwares = bundleRelatedEntities
                .Select(l => l.Software)
                .GroupBy(p => p.Id)
                .Select(grp => grp.First())
                .ToDictionary(bundle => bundle.Id);

            var resultNamedBundleLicenses = bundleRelatedEntities
                .Where(l => l.NamedUserBundle != null)
                .Select(l => l.NamedUserBundle)
                .GroupBy(p => p.Id)
                .Select(grp => grp.First())
                .ToList();

            return (bundles, licensedBundles, softwares, resultNamedBundleLicenses);
        }
    }
}