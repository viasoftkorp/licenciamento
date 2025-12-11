using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.DTO
{
    public class InfrastructureConfigurationCache
    {
        public Guid LicensedTenantId { get; set; }
        public string GatewayAddress { get; set; }
        public string DesktopDatabaseName { get; set; }
    }
}