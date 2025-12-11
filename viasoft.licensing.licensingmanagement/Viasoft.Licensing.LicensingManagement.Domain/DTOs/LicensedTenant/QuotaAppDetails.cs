using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant
{
    public class QuotaAppDetails
    {
        public string Identifier { get; set; }
        
        public Guid AppId { get; set; }
        
        public string Name { get; set; }
        
        public Guid LicencedTenantIdentifier { get; set; }
    }
}