using System;
using System.ComponentModel.DataAnnotations.Schema;
using Viasoft.Core.DDD.Entities.Auditing;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities
{
    [Table("App")]
    public class App: FullAuditedEntity, IMustHaveTenant
    {
        public string Name { get; set; }

        public string Identifier { get; set; }

        public bool IsActive { get; set; }

        public Guid SoftwareId { get; set; }
        
        public bool Default { get; set; }
        
        public Enums.Domain Domain { get; set; }
        
        public Guid TenantId { get; set; }
    }
}