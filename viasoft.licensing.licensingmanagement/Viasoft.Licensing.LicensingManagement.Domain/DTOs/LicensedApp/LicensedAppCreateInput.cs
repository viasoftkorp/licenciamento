using System;
using Viasoft.Data.Attributes;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp
{
    public class LicensedAppCreateInput
    {
        [StrictRequired]
        public Guid LicensedTenantId { get; set; }

        public Guid? LicensedBundleId { get; set; }
        
        [StrictRequired]
        public Guid AppId { get; set; }
        
        [StrictRequired(AllowZeroNumeric = true)]
        public LicensedAppStatus Status { get; set; }
        
        [StrictRequired(AllowNegativeNumeric = false, AllowZeroNumeric = true)]
        public int NumberOfLicenses { get; set; }

        [StrictRequired(AllowNegativeNumeric = false, AllowZeroNumeric = true)]
        public int AdditionalNumberOfLicenses { get; set; }
        
        [StrictRequired(AllowNegativeNumeric = false, AllowZeroNumeric = true)]
        public int NumberOfTemporaryLicenses { get; set; }

        public DateTime? ExpirationDateOfTemporaryLicenses { get; set; }
        public DateTime? ExpirationDateTime { get; set; }

        [StrictRequired]
        public LicensingModels LicensingModel { get; set; }
        
        public LicensingModes? LicensingMode { get; set; }
    }
}