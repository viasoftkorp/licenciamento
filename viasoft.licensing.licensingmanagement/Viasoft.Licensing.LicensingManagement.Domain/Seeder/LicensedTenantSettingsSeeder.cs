using System.Threading.Tasks;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Data.Seeder.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings.Commands;

namespace Viasoft.Licensing.LicensingManagement.Domain.Seeder
{
    public class LicensedTenantSettingsSeeder : ISeedData
    {
        private readonly IServiceBus _serviceBus;

        public LicensedTenantSettingsSeeder(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
        }

        public async Task SeedDataAsync()
        {
            await _serviceBus.SendLocal(new SeedLicensingTenantSettingsCommand());
        }
    }
}