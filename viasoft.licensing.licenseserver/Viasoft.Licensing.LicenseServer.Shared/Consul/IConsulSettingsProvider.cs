using Viasoft.Licensing.LicenseServer.Shared.Contracts.Consul;

namespace Viasoft.Licensing.LicenseServer.Shared.Consul
{
    public interface IConsulSettingsProvider
    {
        public ConsulSettings GetSettingsFromConsul();
        public void LoadSettingsFromConsul();
    }
}