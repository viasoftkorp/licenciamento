using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.LicensedApp
{
    public interface ILicensedAppService
    {
        public Task RemoveLicensedApp(Entities.LicensedTenant licensedTenant, Entities.LicensedApp licensedApp);

        public Task<Entities.LicensedApp> UpdateLicensedApp(Entities.LicensedTenant licensedTenant, Entities.LicensedApp licensedApp,
            LicensedAppUpdateInput input);

        public Task<GetNamedUserFromLicensedAppOutput> GetNamedUserFromLicensedApp(
            Entities.LicensedTenant licensedTenant, Guid licensedAppId, GetAllNamedUserAppInput input);

        public Task<AddNamedUserToLicensedAppOutput> AddNamedUserToLicensedApp(Entities.LicensedTenant licensedTenant, Guid licensedAppId, AddNamedUserToLicensedAppInput input);

        public Task<UpdateNamedUsersFromAppOutput> UpdateNamedUsersFromApp(Entities.LicensedTenant licensedTenant,
            Guid licensedAppId, UpdateNamedUsersFromAppInput input, Guid namedUserId);

        public Task<DeleteNamedUsersFromAppOutput> DeleteNamedUsersFromApp(Entities.LicensedTenant licensedTenant,
            Guid licensedAppId, Guid namedUserId);
    }
}