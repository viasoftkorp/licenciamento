using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO
{
    public class LicensedTenantOutput
    {
        public Guid Id { get; set; }
        
        public Guid AccountId { get; set; }
        
        public string AccountName { get; set; }
        
        public LicensingStatus Status { get; set; }
        
        public Guid Identifier { get; set; }
        
        public DateTime? ExpirationDateTime { get; set; }
        
        public string LicensedCnpjs { get; set; }

        public string AdministratorEmail { get; set; }
        
        public LicenseConsumeType LicenseConsumeType { get; set; }

        public string LicenseConsumeDescription => LicenseConsumeType.ToString();

        public string Notes { get; set; }
        
        public string HardwareId { get; set; }
    }
}