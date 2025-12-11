using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.NamedUserAppLicense
{
    public interface INamedUserAppLicenseService
    {
        public Task<GetNamedUserFromLicensedAppOutput> GetNamedUserFromLicensedApp(
            Entities.LicensedTenant licensedTenant, Entities.LicensedApp licensedApp,
            GetAllNamedUserAppInput input);

        public Task<AddNamedUserToLicensedAppOutput> AddNamedUserToLicensedApp(Entities.LicensedTenant licensedTenant, Entities.LicensedApp licensedApp, AddNamedUserToLicensedAppInput input);

        public Task<UpdateNamedUsersFromAppOutput> UpdateNamedUsersFromApp(Entities.LicensedTenant licensedTenant,
            Entities.LicensedApp licensedApp, UpdateNamedUsersFromAppInput input, Guid namedUserId);

        public Task<DeleteNamedUsersFromAppOutput> DeleteNamedUsersFromApp(Entities.LicensedTenant licensedTenant,
            Entities.LicensedApp licensedApp, Guid namedUserId);
    }
}