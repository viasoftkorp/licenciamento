using System;
using System.ComponentModel.DataAnnotations.Schema;
using Viasoft.Core.DDD.Entities.Auditing;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities
{
    [Table("BundledApp")]
    public class BundledApp: FullAuditedEntity, IMustHaveTenant
    {
        public Guid BundleId { get; set; }
        
        public Guid AppId { get; set; }
        
        public Guid TenantId { get; set; }
    }
}