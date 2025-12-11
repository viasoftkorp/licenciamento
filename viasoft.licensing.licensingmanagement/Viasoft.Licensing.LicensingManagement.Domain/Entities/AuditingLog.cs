using System;
using System.ComponentModel.DataAnnotations.Schema;
using Viasoft.Core.DDD.Entities.Auditing;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities
{
    [Table("AuditingLog")]
    public class AuditingLog: FullAuditedEntity, IMustHaveTenant
    {
        public string UserName { get; set; }
        
        public Guid UserId { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public string Details { get; set; }
        
        public string ActionName { get; set; }
        
        public LogAction Action { get; set; }
        
        public LogType Type { get; set; }
        
        public Guid TenantId { get; set; }
    }
}