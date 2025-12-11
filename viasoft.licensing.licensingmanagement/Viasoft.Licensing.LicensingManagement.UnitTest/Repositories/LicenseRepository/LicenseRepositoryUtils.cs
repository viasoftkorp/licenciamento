using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Account;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Repositories.LicenseRepository
{
    public static class LicenseRepositoryUtils
    {
        public static async Task<List<LicensedTenantDetails>> AddFakeData(IServiceProvider serviceProvider, Guid licensedTenantId, int temporaryLicenses, 
            DateTime? expirationTemporaryLicenses)
        {
            var appId = Guid.NewGuid();
            var softwareId = Guid.NewGuid();
            var bundleId = Guid.NewGuid();
         
            var currentTenant = serviceProvider.GetRequiredService<ICurrentTenant>();
            var licensedTenantRepository = serviceProvider.GetRequiredService<IRepository<LicensedTenant>>();
            var licensedAppRepository = serviceProvider.GetRequiredService<IRepository<LicensedApp>>();
            var appRepository = serviceProvider.GetRequiredService<IRepository<Domain.Entities.App>>();
            var softwareRepository = serviceProvider.GetRequiredService<IRepository<Software>>();
            var licensedBundleRepository = serviceProvider.GetRequiredService<IRepository<LicensedBundle>>();
            var bundleRepository = serviceProvider.GetRequiredService<IRepository<Domain.Entities.Bundle>>();
            var accountRepository = serviceProvider.GetRequiredService<IRepository<Account>>();
            var tenantSettingsRepository = serviceProvider.GetRequiredService<IRepository<LicensedTenantSettings>>();
            
            var newAccount = new Account{ Id = Guid.NewGuid(), Email = "teste@teste.com"};
            var newLicensedBundle = new LicensedBundle
            {
                Id = Guid.NewGuid() , BundleId = bundleId, LicensedTenantId = licensedTenantId, 
                ExpirationDateOfTemporaryLicenses = expirationTemporaryLicenses, NumberOfTemporaryLicenses = temporaryLicenses
            };
            var newBundle = new Domain.Entities.Bundle {Id = bundleId};
            var newTenant = new LicensedTenant
            {
                Id = licensedTenantId, Identifier = licensedTenantId, AccountId = newAccount.Id
            };
            var newLicensedApp = new LicensedApp
            {
                Id = Guid.NewGuid(),AppId = appId, LicensedTenantId = licensedTenantId,
                ExpirationDateOfTemporaryLicenses = expirationTemporaryLicenses, NumberOfTemporaryLicenses = temporaryLicenses
            };
            var newApp = new Domain.Entities.App {Id = appId, SoftwareId = softwareId};
            var newSoftware = new Software {Id = softwareId};
            var tenantSettings = new LicensedTenantSettings
            {
                TenantId = currentTenant.Id,
                LicensingIdentifier = licensedTenantId,
                Key = LicensedTenantSettingsKeys.UseSimpleHardwareIdKey,
                Value = bool.FalseString
            };
            
            await licensedTenantRepository.InsertAsync(newTenant, true);
            await licensedAppRepository.InsertAsync(newLicensedApp, true);
            await appRepository.InsertAsync(newApp, true);
            await softwareRepository.InsertAsync(newSoftware, true);
            await licensedBundleRepository.InsertAsync(newLicensedBundle, true);
            await bundleRepository.InsertAsync(newBundle, true);
            await accountRepository.InsertAsync(newAccount, true);
            await tenantSettingsRepository.InsertAsync(tenantSettings, true);
            
            var bundleDetails = new LicensedBundleDetails
            {
                BundleId = newLicensedBundle.BundleId,
                Identifier = newBundle.Identifier,
                Name = newBundle.Name,
                Status = newLicensedBundle.Status,
                NumberOfLicenses = newLicensedBundle.NumberOfLicenses
            };
            var appDetails = new LicensedAppDetails
            {
                LicensedBundleId = newLicensedApp.LicensedBundleId,
                Name = newApp.Name,
                Identifier = newApp.Identifier,
                Status = newLicensedApp.Status,
                NumberOfLicenses = newLicensedApp.NumberOfLicenses,
                AdditionalNumberOfLicenses = newLicensedApp.AdditionalNumberOfLicenses,
                AppId = newLicensedApp.Id,
                SoftwareIdentifier = newSoftware.Identifier,
                SoftwareName = newSoftware.Name
            };

            var ownedBundles = new List<LicensedBundleDetails> { bundleDetails };
            var ownedApps = new List<LicensedAppDetails> { appDetails };

            var output = new List<LicensedTenantDetails>
            {
                new(ownedBundles, ownedApps, new LicensedTenantSettingsOutput(tenantSettings))
                {
                    LicenseIdentifier = licensedTenantId,
                    AccountDetails = new AccountDetails
                    {
                        Email = newAccount.Email,
                        Phone = newAccount.Phone,
                        Status = newAccount.Status,
                        CnpjCpf = newAccount.CnpjCpf,
                        CompanyName = newAccount.CompanyName,
                        TradingName = newAccount.TradingName,
                        WebSite = newAccount.WebSite
                    }
                }
            };

            return output;
        }
    }
}