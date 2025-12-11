using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Extensions;
using Viasoft.Licensing.LicensingManagement.Domain.Services.NamedUserBundleLicense;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.LicensedBundle
{
    public class LicensedBundleService: ILicensedBundleService, ITransientDependency
    {
        private readonly IRepository<Entities.LicensedApp> _licensedApps;
        private readonly IRepository<Entities.NamedUserBundleLicense> _namedUserBundleLicenses;
        private readonly IRepository<Entities.LicensedBundle> _licensedBundles;
        private readonly INamedUserBundleLicenseService _namedUserBundleLicenseService;
        private readonly IRepository<Bundle> _bundles;

        public LicensedBundleService(IRepository<Entities.LicensedApp> licensedApps, IRepository<Entities.NamedUserBundleLicense> namedUserBundleLicenses, IRepository<Entities.LicensedBundle> licensedBundles, INamedUserBundleLicenseService namedUserBundleLicenseService, IRepository<Bundle> bundles)
        {
            _licensedApps = licensedApps;
            _namedUserBundleLicenses = namedUserBundleLicenses;
            _licensedBundles = licensedBundles;
            _namedUserBundleLicenseService = namedUserBundleLicenseService;
            _bundles = bundles;
        }

        public async Task RemoveLicensedBundleFromLicense(Entities.LicensedTenant licensedTenant, Entities.LicensedBundle licensedBundle)
        {
            await RemoveLicensedApps(licensedBundle, licensedTenant);

            if (licensedBundle.IsNamedLicense())
            {
                await RemoveNamedUsersFromBundle(licensedBundle, licensedTenant);
            }

            await _licensedBundles.DeleteAsync(licensedBundle);
        }

        public async Task<Entities.LicensedBundle> UpdateLicensedBundle(Entities.LicensedTenant licensedTenant, Entities.LicensedBundle licensedBundle,
            LicensedBundleUpdateInput updateInput)
        {
            var shouldRemovedNamedUserBundles = licensedBundle.IsNamedLicense() && updateInput.LicensingModel == LicensingModels.Floating;

            var output = await UpdateLicensedBundle(licensedBundle, updateInput);

            await UpdateLicensedApps(licensedBundle, licensedTenant, updateInput);

            if (shouldRemovedNamedUserBundles)
            {
                await RemoveNamedUsersFromBundle(licensedBundle, licensedTenant);
            }

            return output;
        }

        public async Task<NamedUserBundleLicenseOutput> AddNamedUserToLicensedBundle(Entities.LicensedTenant licensedTenant, Guid licensedBundleId,
            CreateNamedUserBundleLicenseInput input)
        {
            var licensedBundle = await _licensedBundles.FirstOrDefaultAsync(b =>
                b.Id == licensedBundleId && b.LicensedTenantId == licensedTenant.Id);

            if (licensedBundle == null)
                return new NamedUserBundleLicenseOutput
                {
                    OperationValidation = OperationValidation.NoLicensedBundleWithSuchId
                };

            if (licensedBundle.LicensingModel != LicensingModels.Named)
                return new NamedUserBundleLicenseOutput
                {
                    OperationValidation = OperationValidation.NotANamedLicense
                };

            return await _namedUserBundleLicenseService.AddNamedUserToLicensedBundle(licensedTenant, licensedBundle, input);
        }

        public async Task<UpdateNamedUsersFromBundleOutput> UpdateNamedUsersFromBundle(
            Entities.LicensedTenant licensedTenant, Guid licensedBundleId,
            UpdateNamedUserBundleLicenseInput input, Guid namedUserId)
        {
            var licensedBundle = await _licensedBundles.FirstOrDefaultAsync(b =>
                b.Id == licensedBundleId && b.LicensedTenantId == licensedTenant.Id);

            if (licensedBundle == null)
                return new UpdateNamedUsersFromBundleOutput
                {
                    Success = false,
                    ValidationCode = UpdateNamedUsersFromBundleValidationCode.NoLicensedBundle
                };

            return await _namedUserBundleLicenseService.UpdateNamedUsersFromBundle(licensedTenant, licensedBundle, input, namedUserId);
        }

        public async Task<RemoveNamedUserFromBundleOutput> RemoveNamedUserFromBundle(
            Entities.LicensedTenant licensedTenant, Guid licensedBundleId, Guid namedUserId)
        {
            var licensedBundle = await _licensedBundles.FirstOrDefaultAsync(b =>
                b.Id == licensedBundleId && b.LicensedTenantId == licensedTenant.Id);

            if (licensedBundle == null)
                return new RemoveNamedUserFromBundleOutput
                {
                    Success = false,
                    ValidationCode = RemoveNamedUserFromBundleValidationCode.NoLicensedBundle
                };

            return await _namedUserBundleLicenseService.RemoveNamedUserFromBundle(licensedTenant, licensedBundle, namedUserId);
        }

        public async Task<GetNamedUserFromBundleOutput> GetNamedUserFromBundle(Entities.LicensedTenant licensedTenant,
            Guid licensedBundleId, GetAllNamedUserBundleInput input)
        {
            var licensedBundle = await _licensedBundles.FirstOrDefaultAsync(b =>
                b.Id == licensedBundleId && b.LicensedTenantId == licensedTenant.Id);

            if (licensedBundle == null)
                return new GetNamedUserFromBundleOutput
                {
                    NamedUserFromBundleValidationCode = GetNamedUserFromBundleValidationCode.NoLicensedBundle
                };

            return await _namedUserBundleLicenseService.GetNamedUserFromBundle(licensedTenant, licensedBundle, input);
        }

        private async Task RemoveLicensedApps(Entities.LicensedBundle licensedBundle, Entities.LicensedTenant licensedTenant)
        {
            var licensedAppsToDelete = await _licensedApps.Where(a => a.LicensedBundleId == licensedBundle.BundleId && a.LicensedTenantId == licensedTenant.Id).ToListAsync();
            
            foreach (var licensedApp in licensedAppsToDelete)
            {
                await _licensedApps.DeleteAsync(licensedApp);
            }
        }

        private async Task RemoveNamedUsersFromBundle(Entities.LicensedBundle licensedBundle, Entities.LicensedTenant licensedTenant)
        {
            var namedUserBundleLicenseToDelete = await _namedUserBundleLicenses.Where(n => n.LicensedBundleId == licensedBundle.Id && n.LicensedTenantId == licensedTenant.Id).ToListAsync();

            foreach (var namedUserBundleLicense in namedUserBundleLicenseToDelete)
            {
                await _namedUserBundleLicenses.DeleteAsync(namedUserBundleLicense);
            }
        }

        private async Task<Entities.LicensedBundle> UpdateLicensedBundle(Entities.LicensedBundle licensedBundle, LicensedBundleUpdateInput updateInput)
        {
            licensedBundle.NumberOfLicenses = updateInput.NumberOfLicenses;
            licensedBundle.NumberOfTemporaryLicenses = updateInput.NumberOfTemporaryLicenses;
            licensedBundle.ExpirationDateOfTemporaryLicenses = updateInput.ExpirationDateOfTemporaryLicenses;
            licensedBundle.LicensingMode = updateInput.LicensingMode;
            licensedBundle.LicensingModel = updateInput.LicensingModel;
            licensedBundle.ExpirationDateTime = updateInput.ExpirationDateTime;
            licensedBundle.Status = updateInput.Status;

            var output = await _licensedBundles.UpdateAsync(licensedBundle);
            return output;
        }

        private async Task UpdateLicensedApps(Entities.LicensedBundle licensedBundle, Entities.LicensedTenant licensedTenant, LicensedBundleUpdateInput updateInput)
        {
            var licensedAppsToUpdate = await _licensedApps.Where(a => a.LicensedBundleId == licensedBundle.BundleId && a.LicensedTenantId == licensedTenant.Id).ToListAsync();
            
            foreach (var licensedApp in licensedAppsToUpdate)
            {
                licensedApp.NumberOfLicenses = updateInput.NumberOfLicenses;
                licensedApp.NumberOfTemporaryLicenses = updateInput.NumberOfTemporaryLicenses;
                licensedApp.ExpirationDateOfTemporaryLicenses = updateInput.ExpirationDateOfTemporaryLicenses;
                licensedApp.LicensingModel = updateInput.LicensingModel;
                licensedApp.LicensingMode = updateInput.LicensingMode;
                licensedApp.ExpirationDateTime = updateInput.ExpirationDateTime;
                licensedApp.Status = updateInput.Status.ToLicensedAppStatus();
                
                await _licensedApps.UpdateAsync(licensedApp);
            }
        }
        
        public async Task<LicensedBundleOutput> GetLicensedBundleById(Guid licensedBundleId)
        {
            return await _licensedBundles
                .Where(e =>  e.Id == licensedBundleId)
                .Join(_bundles, licensedBundle => licensedBundle.BundleId, bundle => bundle.Id, 
                    (licensedBundle, bundle) => new LicensedBundleOutput()
                    {
                        Id = bundle.Id,
                        Identifier = bundle.Identifier,
                        Name = bundle.Name,
                        IsActive = bundle.IsActive,
                        IsCustom = bundle.IsCustom,
                        LicensingMode = licensedBundle.LicensingMode,
                        LicensingModel = licensedBundle.LicensingModel,
                        NumberOfLicenses = licensedBundle.NumberOfLicenses,
                        NumberOfTemporaryLicenses = licensedBundle.NumberOfTemporaryLicenses,
                        ExpirationDateOfTemporaryLicenses = licensedBundle.ExpirationDateOfTemporaryLicenses,
                        LicensedBundleId = licensedBundle.Id,
                        Status = licensedBundle.Status,
                        NumberOfUsedLicenses = (from namedUserBundleLicense in _namedUserBundleLicenses 
                            where namedUserBundleLicense.LicensedBundleId == licensedBundle.Id
                            select namedUserBundleLicense).Count()
                    })
                .FirstOrDefaultAsync();
        }
    }
}