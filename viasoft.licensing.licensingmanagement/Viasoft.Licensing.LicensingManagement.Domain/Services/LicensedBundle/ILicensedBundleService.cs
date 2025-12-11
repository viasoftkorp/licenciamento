using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.LicensedBundle
{
    public interface ILicensedBundleService
    {
        public Task RemoveLicensedBundleFromLicense(Entities.LicensedTenant licensedTenant, Entities.LicensedBundle licensedBundle);

        public Task<Entities.LicensedBundle> UpdateLicensedBundle(Entities.LicensedTenant licensedTenant,
            Entities.LicensedBundle licensedBundle, LicensedBundleUpdateInput updateInput);

        public Task<NamedUserBundleLicenseOutput> AddNamedUserToLicensedBundle(Entities.LicensedTenant licensedTenant,
            Guid licensedBundleId, CreateNamedUserBundleLicenseInput input);

        public Task<UpdateNamedUsersFromBundleOutput> UpdateNamedUsersFromBundle(Entities.LicensedTenant licensedTenant, Guid licensedBundleId, UpdateNamedUserBundleLicenseInput input, Guid namedUserId);

        public Task<RemoveNamedUserFromBundleOutput> RemoveNamedUserFromBundle(Entities.LicensedTenant licensedTenant,
            Guid licensedBundleId, Guid namedUserId);
        public Task<GetNamedUserFromBundleOutput> GetNamedUserFromBundle(Entities.LicensedTenant licensedTenant,
            Guid licensedBundleId, GetAllNamedUserBundleInput input);

        public Task<LicensedBundleOutput> GetLicensedBundleById(Guid licensedBundleId);
    }
}