using System;
using Viasoft.Data.Attributes;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle
{
    public class LicensedBundleUpdateInput
    {
        [StrictRequired]
        public Guid LicensedTenantId { get; set; }
        
        [StrictRequired]
        public Guid BundleId { get; set; }

        [StrictRequired(AllowZeroNumeric = true)]
        public LicensedBundleStatus Status { get; set; }
        
        [StrictRequired(AllowNegativeNumeric = false, AllowZeroNumeric = true)]
        public int NumberOfLicenses { get; set; }
        
        [StrictRequired(AllowNegativeNumeric = false, AllowZeroNumeric = true)]
        public int NumberOfTemporaryLicenses { get; set; }

        public DateTime? ExpirationDateOfTemporaryLicenses { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        
        [StrictRequired]
        public LicensingModels LicensingModel { get; set; }
        
        public LicensingModes? LicensingMode { get; set; }
    }
}