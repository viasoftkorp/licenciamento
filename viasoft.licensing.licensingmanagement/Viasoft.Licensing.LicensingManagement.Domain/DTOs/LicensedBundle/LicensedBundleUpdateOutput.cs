using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle
{
    public class LicensedBundleUpdateOutput
    {
        public Guid LicensedTenantId { get; set; }
        
        public Guid BundleId { get; set; }
        
        public LicensedBundleStatus Status { get; set; }
        
        public int NumberOfLicenses { get; set; }
        
        public int NumberOfTemporaryLicenses { get; set; }
        
        public DateTime? ExpirationDateOfTemporaryLicenses { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public OperationValidation OperationValidation { get; set; }
        public string OperationValidationDescription => OperationValidation.ToString();
    }
}