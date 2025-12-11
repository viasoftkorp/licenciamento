using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp
{
    public class LicensedAppCreateOutput
    {
        public Guid LicensedTenantId { get; set; }

        public Guid? LicensedBundleId { get; set; }
        
        public Guid AppId { get; set; }
        
        public LicensedAppStatus Status { get; set; }

        public int NumberOfLicenses { get; set; }

        public int AdditionalNumberOfLicenses { get; set; }
        
        public int NumberOfTemporaryLicenses { get; set; }

        public DateTime? ExpirationDateOfTemporaryLicenses { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public OperationValidation OperationValidation { get; set; }
        
        public string OperationValidationDescription => OperationValidation.ToString();

        public static LicensedAppCreateOutput Fail(LicensedAppCreateInput input, OperationValidation operationValidation)
        {
            return new()
            {
                Status = input.Status,
                AppId = input.AppId,
                OperationValidation = operationValidation,
                LicensedBundleId = input.LicensedBundleId,
                LicensedTenantId = input.LicensedTenantId,
                NumberOfLicenses = input.NumberOfLicenses,
                AdditionalNumberOfLicenses = input.AdditionalNumberOfLicenses,
                NumberOfTemporaryLicenses = input.NumberOfTemporaryLicenses,
                ExpirationDateOfTemporaryLicenses = input.ExpirationDateOfTemporaryLicenses
            };
        }
    }
}