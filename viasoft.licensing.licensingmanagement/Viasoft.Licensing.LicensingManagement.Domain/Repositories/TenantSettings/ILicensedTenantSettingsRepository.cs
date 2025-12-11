using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.TenantSettings
{
    public interface ILicensedTenantSettingsRepository
    {
        Task<IQueryable<Entities.LicensedTenantSettings>> GetLicensedTenantSettings(List<Guid> licensedTenantIdentifiers);

        Task<Entities.LicensedTenantSettings> AddLicensedTenantSettings(Guid licensedTenantIdentifier);
        
        Task RemoveLicensedTenantSettings(Guid licensedTenantIdentifier);
    }
}