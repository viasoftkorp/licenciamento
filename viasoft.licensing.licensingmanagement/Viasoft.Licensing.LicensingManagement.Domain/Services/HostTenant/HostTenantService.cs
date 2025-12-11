using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.HostTenantId;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.HostTenant
{
    public class HostTenantService : IHostTenantService, ITransientDependency
    {
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;

        public HostTenantService(IRepository<Entities.LicensedTenant> licensedTenants)
        {
            _licensedTenants = licensedTenants;
        }

        public async Task<HostTenantIdOutput> GetHostTenantIdFromLicensingIdentifier(Guid licensingIdentifier)
        {
            // The only column that can be selected here is the identifier column. Because this method is called by a routine that runs before ef core migrations.
            // See LicensingIdentifierToTenantIdContributor
            var licensedTenant = await _licensedTenants.Select(t => new { t.Identifier, t.TenantId })
                .FirstOrDefaultAsync(t => t.Identifier == licensingIdentifier);
            if (licensedTenant != null)
            {
                return new HostTenantIdOutput
                {
                    TenantId = licensedTenant.TenantId
                };
            }
            return null;
        }
        
        public async Task<HostTenantIdOutput> GetHostTenantIdFromLicensedTenantId(Guid licensedTenantId)
        {
            // The only column that can be selected here is the identifier column. Because this method is called by a routine that runs before ef core migrations.
            // See LicensingIdentifierToTenantIdContributor
            var licensedTenant = await _licensedTenants.Select(t => new { t.Id, t.TenantId })
                .FirstOrDefaultAsync(t => t.Id == licensedTenantId);
            if (licensedTenant != null)
            {
                return new HostTenantIdOutput
                {
                    TenantId = licensedTenant.TenantId
                };
            }
            return null;
        }
    }
}