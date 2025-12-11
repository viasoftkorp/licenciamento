using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Viasoft.Core.DDD.Entities.Auditing;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities
{
    [Table("LicensedBundle")]
    public class LicensedBundle: FullAuditedEntity, IMustHaveTenant
    { 
        public Guid BundleId { get; set; }
        public Guid LicensedTenantId { get; set; }
        public LicensedBundleStatus Status { get; set; }
        public int NumberOfLicenses { get; set; }
        public Guid TenantId { get; set; }
        public int NumberOfTemporaryLicenses { get; set; }
        public DateTime? ExpirationDateOfTemporaryLicenses { get; set; }
        [Required]
        public LicensingModels LicensingModel { get; set; }
        public LicensingModes? LicensingMode { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public bool IsNamedLicense()
        {
            return LicensingModel == LicensingModels.Named;
        }
    }
}