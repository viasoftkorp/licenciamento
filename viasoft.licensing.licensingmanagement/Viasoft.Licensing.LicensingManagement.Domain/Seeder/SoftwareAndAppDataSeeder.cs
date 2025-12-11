using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Organization;
using Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.OrganizationUnit;

namespace Viasoft.Licensing.LicensingManagement.Domain.Seeder
{
    public class SoftwareAndAppDataSeeder : ISoftwareAndAppDataSeeder, ITransientDependency
    {
        private Guid? _administratorUserId;
        private Guid _defaultLicensedTenantId;
        private string _defaultLicencedCnpjs;
        private string _defaultAdministratorEmail;

        private readonly IRepository<Software> _softwares;
        private readonly IRepository<App> _apps;
        private readonly IRepository<Bundle> _bundles;
        private readonly IRepository<BundledApp> _bundledApps;
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILicensedTenantService _licensedTenantService;
        private readonly IRepository<Entities.Account> _accounts;
        private readonly ITenantManagementCaller _tenantManagementCaller;
        private readonly IAccountRepository _accountRepository;

        private Software _defaultSoftware;
        private Bundle _defaultBundle;

        private Software DefaultWebSoftware => _defaultSoftware ??= _softwares.First(software => software.Identifier == "WEB"); 
        private Bundle DefaultBundle => _defaultBundle ??= _bundles.First(bundle => bundle.Identifier == "Default" && bundle.SoftwareId == DefaultWebSoftware.Id);
        
        private async Task<bool> CheckIfDefaultSoftwareExists() => await _softwares.AnyAsync(software => software.Identifier == "WEB");
        private async Task<bool> CheckIfDefaultAppExists(string appIdentifier) => await _apps.AnyAsync(app => app.Identifier == appIdentifier);
        private async Task<bool> CheckIfDefaultBundleExists() => await _bundles.AnyAsync(bundle => bundle.Identifier == "Default" && bundle.SoftwareId == DefaultWebSoftware.Id);
        private async Task<bool> CheckIfAppIsBundled(Guid appId) => await _bundledApps.AnyAsync(bundledApp => bundledApp.AppId == appId && bundledApp.BundleId == DefaultBundle.Id);
        private async Task<bool> CheckIfDefaultAccountExists() => await _accounts.AnyAsync();
        private async Task<bool> CheckIfDefaultLicensingExists() => await _licensedTenants.AnyAsync(licensedTenant => licensedTenant.Identifier == _defaultLicensedTenantId);

        public SoftwareAndAppDataSeeder(IRepository<Software> softwares, IRepository<App> apps,
            IRepository<Bundle> bundles,
            IRepository<BundledApp> bundledApps, ILicensedTenantService licensedTenantService,
            IRepository<Entities.Account> accounts,
            IRepository<Entities.LicensedTenant> licensedTenants, IUnitOfWork unitOfWork,
            ITenantManagementCaller tenantManagementCaller, IAccountRepository accountRepository)
        {
            _softwares = softwares;
            _apps = apps;
            _bundles = bundles;
            _bundledApps = bundledApps;
            _licensedTenantService = licensedTenantService;
            _accounts = accounts;
            _licensedTenants = licensedTenants;
            _unitOfWork = unitOfWork;
            _tenantManagementCaller = tenantManagementCaller;
            _accountRepository = accountRepository;
        }

        private async Task CreateDefaultLicensing()
        {
            if (await CheckIfDefaultLicensingExists())
                return;

            var defaultAccountId = await _accounts.Select(account => account.Id).FirstAsync();
            
            var input = new LicenseTenantCreateInput
            {
                AdministratorEmail = _defaultAdministratorEmail,
                AccountId = defaultAccountId,
                Identifier = _defaultLicensedTenantId,
                LicensedCnpjs = _defaultLicencedCnpjs,
                Status = LicensingStatus.Active,
                Id = Guid.NewGuid(),
                AdministratorUserId =_administratorUserId 
            };
            await _licensedTenantService.CreateNewTenantLicensing(input, true);
        }
        
        private async Task CreateDefaultOrganizationEnvironment()
        {
            var licenseTenant = await _licensedTenants.FirstAsync(l => l.Identifier == _defaultLicensedTenantId);
            var organization = await TryToGetOrganization(licenseTenant);

            Guid? organizationId;
            if (organization == null || organization.Id == Guid.Empty)
            {
                var organizationOutput = await _tenantManagementCaller.CreateOrganization(licenseTenant.Identifier,
                    new CreateOrUpdateOrganizationInput
                    {
                        Id = licenseTenant.Id,
                        Name = _accountRepository.GetAccountNameFromId(licenseTenant.AccountId),
                        IsActive = true,
                        TenantId = licenseTenant.Identifier
                    });
                if (organizationOutput?.Status != CreateOrganizationOutputStatus.Ok || organizationOutput.Organization?.Id == null || organizationOutput.Organization?.Id == Guid.Empty)
                    throw new Exception(
                        $"Error trying to add default environment for License Tenant with following identifier (get organization): {licenseTenant.Identifier}");
                organizationId = organizationOutput.Organization?.Id;
            }
            else
                organizationId = organization.Id;

            if (await GetIfIsOrganizationSeeded(licenseTenant, organizationId))
                return;
            
            const string defaultName = "Default";
            var unitResult = await _tenantManagementCaller.CreateOrganizationUnit(licenseTenant.Identifier, new CreateOrUpdateOrganizationUnitInput
            {
                Id = Guid.NewGuid(),
                Name = defaultName,
                Description = defaultName,
                IsActive = true,
                OrganizationId = organizationId.Value
            });
            if (unitResult.Status != CreateOrganizationUnitOutputStatus.Ok)
                throw new Exception($"Error trying to add default environment for License Tenant with following identifier (create unit): {licenseTenant.Identifier}");

            var envResult =
                await _tenantManagementCaller.CreateOrganizationEnvironment(licenseTenant.Identifier, new CreateOrUpdateEnvironmentInput
                {
                    Id = Guid.NewGuid(),
                    Name = defaultName,
                    Description = defaultName,
                    IsActive = true,
                    IsWeb = true,
                    IsProduction = true,
                    IsMobile = true,
                    IsDesktop = false,
                    OrganizationUnitId = unitResult.Unit.Id
                });
            if (envResult.Status != CreateEnvironmentOutputStatus.Ok)
                throw new Exception($"Error trying to add default environment for License Tenant with following identifier (create environment): {licenseTenant.Identifier}");
        }

        private async Task<bool> GetIfIsOrganizationSeeded(Entities.LicensedTenant licenseTenant, Guid? organizationId)
        {
            if (!organizationId.HasValue)
                return false;
            var organizationUnits = await _tenantManagementCaller.GetUnitsByOrganization(licenseTenant.Identifier,
                new GetByOrganizationInput
                {
                    OrganizationId = organizationId.Value,
                    MaxResultCount = 1
                });
            var isAlreadySeeded = organizationUnits.TotalCount > 0;
            return isAlreadySeeded;
        }

        private async Task<Organization> TryToGetOrganization(Entities.LicensedTenant licenseTenant)
        {
            const int maxRetryCount = 3; // times to retry creating environment (Tenant is still warming up)
            Organization organization = null;
            for (var i = 0; i < maxRetryCount; i++)
            {
                try
                {
                    // Awaits before retrying
                    await Task.Delay(5000);
                    organization = await _tenantManagementCaller.GetOrganization(licenseTenant.Identifier, licenseTenant.Id);
                }
                catch (Exception)
                {
                    Console.WriteLine("Waiting 5 seconds before retrying to get Organization...");
                }
            }

            return organization;
        }

        private async Task CreateDefaultAccount()
        {
            if (!await CheckIfDefaultAccountExists())
            {
                var input = new Entities.Account
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Default Company",
                    CnpjCpf = _defaultLicencedCnpjs,
                    Status = AccountStatus.Active
                };
                await _accounts.InsertAsync(input, true);
            }
        }

        private async Task CreateDefaultSoftware()
        {
            if (!await CheckIfDefaultSoftwareExists())
            {
                var webSoftware = new Software
                {
                    Id = Guid.NewGuid(),
                    Name = "Sistema web",
                    Identifier = "WEB",
                    IsActive = true,
                    IsDeleted = false,
                    CreationTime = DateTime.UtcNow
                };
                await _softwares.InsertAsync(webSoftware, true);
            }
        }

        private async Task CreateAppIfNotExists(App app)
        {
            if (!await CheckIfDefaultAppExists(app.Identifier))
            {
                await _apps.InsertAsync(app, true);
            }
            else
            {
                var currentIdentifier = await _apps.Select(a => new { a.Identifier, a.Id })
                    .FirstAsync(firstApp => firstApp.Identifier == app.Identifier);
                
                app.Id = currentIdentifier.Id;
            }
        }

        private async Task CreateDefaultBundle()
        {
            if (!await CheckIfDefaultBundleExists())
            {
                var softwareId = DefaultWebSoftware.Id;
                var defaultBundle = new Bundle
                {
                    Id = Guid.NewGuid(),
                    Name = "Pacote Default",
                    Identifier = "Default",
                    IsActive = true,
                    IsCustom = false,
                    CreationTime = DateTime.UtcNow,
                    SoftwareId = softwareId
                };
                await _bundles.InsertAsync(defaultBundle, true);
            }
        }
        
        private async Task CreateDefaultApps(IEnumerable<App> apps)
        {
            if (await CheckIfDefaultSoftwareExists())
                foreach (var app in apps)
                    await CreateAppIfNotExists(app);
        }


        private async Task AddDefaultAppsToDefaultBundle(IEnumerable<App> apps)
        {
            foreach (var app in apps)
            {
                if (await CheckIfAppIsBundled(app.Id)) 
                    continue;
                
                var bundledApp = new BundledApp
                {
                    CreationTime = DateTime.UtcNow,
                    BundleId = DefaultBundle.Id,
                    AppId = app.Id
                };
                await _bundledApps.InsertAsync(bundledApp, true);
            }
        }

        public async Task Seed(Guid tenantIdentifier, string administratorEmail, string defaultLicensedCnpjs,
            Guid? administratorUserId)
        {
            if(tenantIdentifier == Guid.Empty)
                throw new ArgumentException(nameof(tenantIdentifier));
            if(string.IsNullOrEmpty(administratorEmail))
                throw new ArgumentException(nameof(administratorEmail));
            if(string.IsNullOrEmpty(defaultLicensedCnpjs))
                throw new ArgumentException(nameof(defaultLicensedCnpjs));

            _defaultLicensedTenantId = tenantIdentifier;
            _defaultLicencedCnpjs = defaultLicensedCnpjs;
            _defaultAdministratorEmail = administratorEmail;
            _administratorUserId = administratorUserId;

            using (_unitOfWork.Begin())
            {
                await CreateDefaultSoftware();
                await CreateDefaultBundle();
                await _unitOfWork.SaveChangesAsync();

                var apps = SoftwareAndAppDataSeederExtensions.GetDefaultApps(DefaultWebSoftware.Id);

                await CreateDefaultApps(apps);
                await AddDefaultAppsToDefaultBundle(apps);
                await CreateDefaultAccount();
                await CreateDefaultLicensing();
                await _unitOfWork.CompleteAsync();
            }

            await CreateDefaultOrganizationEnvironment();
        }
    }
}