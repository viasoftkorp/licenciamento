using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.NamedUserBundleLicense
{
    public interface INamedUserBundleLicenseService
    {
        public Task<GetNamedUserFromBundleOutput> GetNamedUserFromBundle(Entities.LicensedTenant licensedTenant,
            Entities.LicensedBundle licensedBundle, GetAllNamedUserBundleInput input);
        public Task<NamedUserBundleLicenseOutput> AddNamedUserToLicensedBundle(Entities.LicensedTenant licensedTenant, Entities.LicensedBundle licensedBundle, CreateNamedUserBundleLicenseInput input);
        public Task<UpdateNamedUsersFromBundleOutput> UpdateNamedUsersFromBundle(Entities.LicensedTenant licensedTenant,
            Entities.LicensedBundle licensedBundle, UpdateNamedUserBundleLicenseInput input, Guid namedUserId);
        public Task<RemoveNamedUserFromBundleOutput> RemoveNamedUserFromBundle(Entities.LicensedTenant licensedTenant,
            Entities.LicensedBundle licensedBundle, Guid namedUserId);
    }
}