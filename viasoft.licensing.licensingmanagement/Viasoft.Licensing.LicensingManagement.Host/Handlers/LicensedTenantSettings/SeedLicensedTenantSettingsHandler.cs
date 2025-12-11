using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rebus.Handlers;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.EntityFrameworkCore.Extensions;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings.Messages;

namespace Viasoft.Licensing.LicensingManagement.Host.Handlers.LicensedTenantSettings
{
    public class SeedLicensedTenantSettingsHandler : IHandleMessages<SeedLicensingTenantSettingsMessage>
    {
        private readonly IRepository<Domain.Entities.LicensedTenant> _licensedTenants;
        private readonly IRepository<Domain.Entities.LicensedTenantSettings> _licensedTenantSettings;
        private readonly IUnitOfWork _unitOfWork;

        public SeedLicensedTenantSettingsHandler(IRepository<Domain.Entities.LicensedTenant> licensedTenants, IRepository<Domain.Entities.LicensedTenantSettings> licensedTenantSettings, IUnitOfWork unitOfWork)
        {
            _licensedTenants = licensedTenants;
            _licensedTenantSettings = licensedTenantSettings;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SeedLicensingTenantSettingsMessage message)
        {
            do
            {
                var licensedTenantsSettingsToInsert = await (
                    from licensedTenants in _licensedTenants
                    join licensedTenantSettings in _licensedTenantSettings on 
                        new { licensedTenants.TenantId, LicensingIdentifier = licensedTenants.Identifier } equals
                        new { licensedTenantSettings.TenantId, licensedTenantSettings.LicensingIdentifier }
                        into joinedQuery
                    from licensedTenantSettings in joinedQuery.DefaultIfEmpty()
                    where licensedTenantSettings == null
                    select new Domain.Entities.LicensedTenantSettings
                    {
                        TenantId = licensedTenants.TenantId,
                        LicensingIdentifier = licensedTenants.Identifier, 
                        Key = LicensedTenantSettingsKeys.UseSimpleHardwareIdKey,
                        Value = bool.FalseString
                    })
                    .OrderBy(e => e.TenantId)
                    .Take(300)
                    .ToListAsync();

                if (licensedTenantsSettingsToInsert.Any())
                {
                    using (_unitOfWork.Begin())
                    {
                        await _licensedTenantSettings.InsertRangeAsync(licensedTenantsSettingsToInsert);
                        await _unitOfWork.CompleteAsync();
                    }

                    var dbContext = _licensedTenants.GetUnderlyingDbContext();
                    dbContext.ChangeTracker.Clear();
                }
                else
                {
                    break;
                }

            } while (true);
        }
    }
}