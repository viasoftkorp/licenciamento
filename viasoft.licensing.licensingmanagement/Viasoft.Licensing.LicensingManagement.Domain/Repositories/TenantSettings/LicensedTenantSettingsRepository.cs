using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.TenantSettings
{
    public class LicensedTenantSettingsRepository: ILicensedTenantSettingsRepository, ITransientDependency
    {
        private readonly IRepository<Entities.LicensedTenantSettings> _licensedTenantSettings;
        private readonly ICurrentTenant _currentTenant;
        
        public LicensedTenantSettingsRepository(IRepository<Entities.LicensedTenantSettings> licensedTenantSettings, ICurrentTenant currentTenant)
        {
            _licensedTenantSettings = licensedTenantSettings;
            _currentTenant = currentTenant;
        }
        
        public Task<IQueryable<Entities.LicensedTenantSettings>> GetLicensedTenantSettings(List<Guid> licensedTenantIdentifiers)
        {
            var query = _licensedTenantSettings.Where(settings => licensedTenantIdentifiers.Contains(settings.LicensingIdentifier))
                .Where(settings => settings.TenantId == _currentTenant.Id);
            
            return Task.FromResult(query);
        }

        public async Task<Entities.LicensedTenantSettings> AddLicensedTenantSettings(Guid licensedTenantIdentifier)
        {
            var settings = new Entities.LicensedTenantSettings
            {
                TenantId = _currentTenant.Id,
                LicensingIdentifier = licensedTenantIdentifier,
                Key = LicensedTenantSettingsKeys.UseSimpleHardwareIdKey,
                Value = bool.FalseString
            };
            
            await _licensedTenantSettings.InsertAsync(settings);

            return settings;
        }

        public async Task RemoveLicensedTenantSettings(Guid licensedTenantIdentifier)
        {
            var tenantSettingsQuery = await GetLicensedTenantSettings(new List<Guid> { licensedTenantIdentifier });
            var licensedTenantSettingsToDelete = await tenantSettingsQuery.FirstAsync();
            
            await _licensedTenantSettings.DeleteAsync(licensedTenantSettingsToDelete);
        }
    }
}