using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Settings
{
    public class InfrastructureConfigurationCreateInput
    {
        public Guid LicensedTenantId { get; set; }
        public string GatewayAddress { get; set; }
    }
}