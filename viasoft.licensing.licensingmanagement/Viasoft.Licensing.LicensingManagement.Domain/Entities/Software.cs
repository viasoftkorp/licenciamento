using System;
using System.ComponentModel.DataAnnotations.Schema;
using Viasoft.Core.DDD.Entities.Auditing;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities
{
    [Table("Software")]
    public class Software: FullAuditedEntity, IMustHaveTenant
    {
        public string Name { get; set; }
        
        public string Identifier { get; set; }
        
        public string Company { get; set; }
        
        public bool IsActive { get; set; }
        
        public Guid TenantId { get; set; }
    }
}