using System;

namespace Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant
{
    public class LicensedTenantSettingsOutput
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid LicensingIdentifier { get; set; }    
        public string Key { get; set;}
        public string Value { get; set; }
    }
}