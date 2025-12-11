namespace Viasoft.Licensing.LicenseServer.Shared.Contracts.Consul
{
    public class ConsulAuthentication
    {
        public string Authority { get; set; }
        public string CustomerLicensingSecret { get; set; }
        public string LicensingManagementSecret { get; set; }
    }
}