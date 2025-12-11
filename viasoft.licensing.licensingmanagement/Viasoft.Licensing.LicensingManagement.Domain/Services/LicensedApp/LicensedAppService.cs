using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Services.NamedUserAppLicense;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.LicensedApp
{
    public class LicensedAppService: ILicensedAppService, ITransientDependency
    {
        private readonly IRepository<Entities.LicensedApp> _licensedApps;
        private readonly IRepository<Entities.NamedUserAppLicense> _namedUserAppLicenses;
        private readonly INamedUserAppLicenseService _namedUserAppLicenseService;

        public LicensedAppService(IRepository<Entities.LicensedApp> licensedApps, IRepository<Entities.NamedUserAppLicense> namedUserAppLicenses, INamedUserAppLicenseService namedUserAppLicenseService)
        {
            _licensedApps = licensedApps;
            _namedUserAppLicenses = namedUserAppLicenses;
            _namedUserAppLicenseService = namedUserAppLicenseService;
        }

        public async Task RemoveLicensedApp(Entities.LicensedTenant licensedTenant, Entities.LicensedApp licensedApp)
        {
            if (licensedApp.IsNamed())
                await RemoveNamedUserAppLicenses(licensedTenant, licensedApp);

            await _licensedApps.DeleteAsync(licensedApp);
        }

        public async Task<Entities.LicensedApp> UpdateLicensedApp(Entities.LicensedTenant licensedTenant, Entities.LicensedApp licensedApp, LicensedAppUpdateInput input)
        {
            if (licensedApp.IsNamed() && input.LicensingModel == LicensingModels.Floating)
                await RemoveNamedUserAppLicenses(licensedTenant, licensedApp);

            licensedApp.Status = input.Status;
            licensedApp.LicensingMode = input.LicensingMode;
            licensedApp.LicensingModel = input.LicensingModel;
            licensedApp.NumberOfLicenses = input.NumberOfLicenses;
            licensedApp.NumberOfTemporaryLicenses = input.NumberOfTemporaryLicenses;
            licensedApp.AdditionalNumberOfLicenses = input.AdditionalNumberOfLicenses;
            licensedApp.ExpirationDateOfTemporaryLicenses = input.ExpirationDateOfTemporaryLicenses;
            licensedApp.ExpirationDateTime = input.ExpirationDateTime;

            var output = await _licensedApps.UpdateAsync(licensedApp);

            return output;
        }

        public async Task<GetNamedUserFromLicensedAppOutput> GetNamedUserFromLicensedApp(
            Entities.LicensedTenant licensedTenant, Guid licensedAppId, GetAllNamedUserAppInput input)
        {
            var licensedApp = await _licensedApps.FirstOrDefaultAsync(a => a.Id == licensedAppId && a.LicensedTenantId == licensedTenant.Id);

            if (licensedApp == null)
                return new GetNamedUserFromLicensedAppOutput
                {
                    ValidationCode = GetNamedUserFromLicensedAppValidationCode.NoLicensedApp
                };

            return await _namedUserAppLicenseService.GetNamedUserFromLicensedApp(licensedTenant, licensedApp, input);
        }

        public async Task<AddNamedUserToLicensedAppOutput> AddNamedUserToLicensedApp(Entities.LicensedTenant licensedTenant, Guid licensedAppId, AddNamedUserToLicensedAppInput input)
        {
            var licensedApp = await _licensedApps.FirstOrDefaultAsync(a => a.Id == licensedAppId && a.LicensedTenantId == licensedTenant.Id);

            if (licensedApp == null)
                return new AddNamedUserToLicensedAppOutput
                {
                    ValidationCode = AddNamedUserToLicensedAppValidationCode.NoLicensedApp,
                };

            if (!licensedApp.IsNamed())
                return new AddNamedUserToLicensedAppOutput
                {
                    ValidationCode = AddNamedUserToLicensedAppValidationCode.LicensedAppIsNotNamed 
                };

            return await _namedUserAppLicenseService.AddNamedUserToLicensedApp(licensedTenant, licensedApp, input);
        }

        public async Task<UpdateNamedUsersFromAppOutput> UpdateNamedUsersFromApp(Entities.LicensedTenant licensedTenant,
            Guid licensedAppId, UpdateNamedUsersFromAppInput input, Guid namedUserId)
        {
            var licensedApp = await _licensedApps.FirstOrDefaultAsync(a =>
                a.Id == licensedAppId && a.LicensedTenantId == licensedTenant.Id);

            if (licensedApp == null)
                return new UpdateNamedUsersFromAppOutput
                {
                    ValidationCode = UpdateNamedUserAppLicenseValidationCode.NoLicensedApp
                };

            return await _namedUserAppLicenseService.UpdateNamedUsersFromApp(licensedTenant, licensedApp, input, namedUserId);
        }

        public async Task<DeleteNamedUsersFromAppOutput> DeleteNamedUsersFromApp(Entities.LicensedTenant licensedTenant,
            Guid licensedAppId, Guid namedUserId)
        {
            var licensedApp = await _licensedApps.FirstOrDefaultAsync(a =>
                a.Id == licensedAppId && a.LicensedTenantId == licensedTenant.Id);

            if (licensedApp == null)
                return new DeleteNamedUsersFromAppOutput
                {
                    ValidationCode = DeleteNamedUsersFromAppValidationCode.NoLicensedApp
                };

            return await _namedUserAppLicenseService.DeleteNamedUsersFromApp(licensedTenant, licensedApp, namedUserId);
        }

        private async Task RemoveNamedUserAppLicenses(Entities.LicensedTenant licensedTenant,
            Entities.LicensedApp licensedApp)
        {
            var namedUserApps = await _namedUserAppLicenses.Where(a => a.LicensedTenantId == licensedTenant.Id && a.LicensedAppId == licensedApp.Id).ToListAsync();

            foreach (var namedUserApp in namedUserApps)
            {
                await _namedUserAppLicenses.DeleteAsync(namedUserApp);
            }
        }
    }
}