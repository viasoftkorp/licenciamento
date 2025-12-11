using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSaga;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service
{
    public interface ILicensedTenantService
    {
        Task<LicenseTenantCreateOutput> CreateNewTenantLicensing(LicenseTenantCreateInput input, bool shouldUseUnitOfWork);
        Task<DeleteLicenseOutput> DeleteTenantLicensing(Guid id);
        Task<LicenseTenantUpdateOutput> UpdateTenantLicensing(LicenseTenantUpdateInput input, LicensedTenantSagaInfo sagaInfo = null);
        
        Task<LicensedBundleCreateOutput> AddBundleToLicense(LicensedBundleCreateInput input);
        Task<LicenseTenantDeleteOutput> RemoveBundleFromLicense(LicensedBundleDeleteInput input);
        Task<LicensedBundleUpdateOutput> UpdateBundleFromLicense(LicensedBundleUpdateInput input);
        
        Task<LicensedAppCreateOutput> AddLooseAppToLicense(LicensedAppCreateInput input);
        Task<RemoveAppFromLicenseOutput> RemoveAppFromLicense(LicensedAppDeleteInput input);
        Task<List<Guid>> RemoveAppsFromLicenses(List<LicensedApp> licensedAppToDeleteFromLicenses, Dictionary<Guid, Guid> licensedTenantIdToIdentifier);
        
        Task<LicensedAppUpdateOutput> UpdateBundledAppFromLicense(LicensedAppUpdateInput input);
        Task<LicensedApp> AddBundledAppsToLicense(Guid appId, LicensedBundleCreateInput input);
        Task<LicensedBundle> CreateLicensedBundle(LicensedBundleCreateInput input);
        
        Task<LicensedApp> CreateNewLicensedApp(LicensedAppCreateInput input);
        Task<LicensedAppUpdateOutput> UpdateLooseAppFromLicense(LicensedAppUpdateInput input);
        
        Task PublishLicensingDetailsUpdatedEvent(Guid licensedTenantIdentifier);
        Task PublishLicensingDetailsUpdatedEvents(List<Guid> licensedTenantIdentifier);
        
        Task<NamedUserBundleLicenseOutput> AddNamedUserToLicensedBundle(Guid licensedTenantId, Guid licensedBundleId, CreateNamedUserBundleLicenseInput input);
        Task<UpdateNamedUsersFromBundleOutput> UpdateNamedUsersFromBundle(Guid licensedTenantId, Guid licensedBundleId, UpdateNamedUserBundleLicenseInput input, Guid namedUserId);
        Task<RemoveNamedUserFromBundleOutput> RemoveNamedUserFromBundle(Guid licensedTenantId, Guid licensedBundleId, Guid namedUserId);
        Task<GetNamedUserFromBundleOutput> GetNamedUserFromBundle(Guid licensedTenantId, Guid licensedBundleId,
            GetAllNamedUserBundleInput input);
        
        Task<GetNamedUserFromLicensedAppOutput> GetNamedUserFromLicensedApp(Guid licensedTenantId, Guid licensedAppId,
            GetAllNamedUserAppInput input);
        Task<AddNamedUserToLicensedAppOutput> AddNamedUserToLicensedApp(Guid licensedTenantId, Guid licensedAppId, AddNamedUserToLicensedAppInput input);
        Task<UpdateNamedUsersFromAppOutput> UpdateNamedUsersFromApp(Guid licensedTenantId, Guid licensedAppId,
            UpdateNamedUsersFromAppInput input, Guid namedUserId);
        Task<DeleteNamedUsersFromAppOutput> DeleteNamedUsersFromApp(Guid licensedTenantId, Guid licensedAppId,
            Guid namedUserId);
    }
}