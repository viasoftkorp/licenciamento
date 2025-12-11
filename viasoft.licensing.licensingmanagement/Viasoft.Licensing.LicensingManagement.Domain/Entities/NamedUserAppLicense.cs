using System;
using System.ComponentModel.DataAnnotations;
using Viasoft.Core.DDD.Entities.Auditing;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities
{
    public class NamedUserAppLicense: FullAuditedEntity, IMustHaveTenant
    {
        public Guid TenantId { get; set; }
        [Required]
        public Guid LicensedTenantId { get; set; }
        [Required]
        public Guid LicensedAppId { get; set; }
        [Required]
        public Guid NamedUserId { get; set; }
        [Required]
        public string NamedUserEmail { get; set; }
        public string DeviceId { get; set; }
    }
}