using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.DTO
{
    public class InfrastructureConfigurationCreateInput
    {
        public Guid LicensedTenantId { get; set; }
        public string GatewayAddress { get; set; }
    }
}