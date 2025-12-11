using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantSettings.Messages
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.SeedLicensingTenantSettings")]
    public class SeedLicensingTenantSettingsMessage : IMessage, IInternalMessage
    {
    }
}