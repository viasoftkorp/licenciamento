using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings.Commands
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.SeedLicensingTenantSettings", "Viasoft.Licensing.LicensingManagement")]
    public class SeedLicensingTenantSettingsCommand : ICommand, IInternalMessage
    {
    }
}