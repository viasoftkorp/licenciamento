using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings
{
    public class InfrastructureConfigurationCreateOutput
    {
        public Guid LicensedTenantId { get; set; }
        public string GatewayAddress { get; set; }
        public string DesktopDatabaseName { get; set; }
    }
}