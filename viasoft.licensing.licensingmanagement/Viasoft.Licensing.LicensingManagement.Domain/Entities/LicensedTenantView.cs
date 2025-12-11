using System;
using System.ComponentModel.DataAnnotations.Schema;
using Viasoft.Core.DDD.Entities.Auditing;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities
{
    [Table("LicensedTenantView")]
    public class LicensedTenantView : FullAuditedEntity, IMustHaveTenant
    {
        public LicensingStatus Status { get; set; }

        public Guid Identifier { get; set; }

        public Guid LicensedTenantId { get; set; }
        
        public DateTime? ExpirationDateTime { get; set; }
        
        public string LicensedCnpjs { get; set; }

        public string AdministratorEmail { get; set; }

        public Guid AccountId { get; set; }

        public string AccountCompanyName { get; set; }
        
        public Guid TenantId { get; set; }
        
        public string HardwareId { get; set; }
    }
}