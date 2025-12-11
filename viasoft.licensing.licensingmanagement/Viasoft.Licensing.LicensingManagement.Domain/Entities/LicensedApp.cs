using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Viasoft.Core.DDD.Entities.Auditing;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities
{
    [Table("LicensedApp")]
    public class LicensedApp: FullAuditedEntity, IMustHaveTenant
    {
        public Guid LicensedTenantId { get; set; }

        public Guid? LicensedBundleId { get; set; }
        
        public Guid AppId { get; set; }
        
        public LicensedAppStatus Status { get; set; }

        public int NumberOfLicenses { get; set; }
        
        public int NumberOfTemporaryLicenses { get; set; }
        
        public DateTime? ExpirationDateOfTemporaryLicenses { get; set; }

        public int AdditionalNumberOfLicenses { get; set; }
        
        public Guid TenantId { get; set; }
        
        [Required]
        public LicensingModels LicensingModel { get; set; }
        
        public LicensingModes? LicensingMode { get; set; }
        
        public DateTime? ExpirationDateTime { get; set; }

        public bool IsNamed()
        {
            return LicensingModel == LicensingModels.Named;
        }
    }
}