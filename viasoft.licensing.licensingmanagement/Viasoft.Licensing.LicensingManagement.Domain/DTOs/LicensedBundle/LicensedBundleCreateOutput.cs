using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle
{
    public class LicensedBundleCreateOutput
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

        public static LicensedBundleCreateOutput Fail(LicensedBundleCreateInput input,
            OperationValidation operationValidation)
        {
            return new()
            {
                Status = input.Status,
                BundleId = input.BundleId,
                OperationValidation = operationValidation,
                LicensedTenantId = input.LicensedTenantId,
                NumberOfLicenses = input.NumberOfLicenses,
                NumberOfTemporaryLicenses = input.NumberOfTemporaryLicenses,
                ExpirationDateOfTemporaryLicenses = input.ExpirationDateOfTemporaryLicenses
            };
        }
    }
}