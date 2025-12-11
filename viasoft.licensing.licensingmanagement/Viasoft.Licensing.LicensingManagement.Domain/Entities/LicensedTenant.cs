using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Viasoft.Core.DDD.Entities.Auditing;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities
{
    [Table("LicensedTenant")]
    public class LicensedTenant: FullAuditedEntity, IMustHaveTenant
    {
        public Guid AccountId { get; set; }
        
        public LicensingStatus Status { get; set; }
        
        public Guid Identifier { get; set; }
        
        public DateTime? ExpirationDateTime { get; set; }
        
        public string LicensedCnpjs { get; set; }

        public string AdministratorEmail { get; set; }

        public byte[] Notes {get; set;}
        
        public LicenseConsumeType LicenseConsumeType { get; set; }

        [NotMapped] 
        public string NotesString => Notes == null ? string.Empty : Encoding.UTF8.GetString(Notes);

        [NotMapped]
        public List<string> LicensedCnpjList =>
            LicensedCnpjs != null ? LicensedCnpjs.Split(',').ToList() : new List<string>();
        
        public Guid TenantId { get; set; }
        
        public string HardwareId { get; set; }
        public byte[] SagaInfo { get; set; }
    }
}