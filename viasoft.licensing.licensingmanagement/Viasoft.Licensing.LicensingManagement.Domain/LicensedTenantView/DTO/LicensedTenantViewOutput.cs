using System;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.DTO
{
    public class LicensedTenantViewOutput
    {
        public LicensingStatus Status { get; set; }

        public Guid Identifier { get; set; }

        public Guid LicensedTenantId { get; set; }
        
        public DateTime? ExpirationDateTime { get; set; }
        
        public string LicensedCnpjs { get; set; }

        public string AdministratorEmail { get; set; }

        public Guid AccountId { get; set; }

        public string AccountCompanyName { get; set; }
        
        public string HardwareId { get; set; }
    }
}