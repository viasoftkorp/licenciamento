using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp
{
    public class LicensedAppUpdateOutput
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
        
        public static LicensedAppUpdateOutput Fail(OperationValidation operationValidation,
            LicensedAppUpdateInput input)
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
                ExpirationDateOfTemporaryLicenses = input.ExpirationDateOfTemporaryLicenses,
                ExpirationDateTime = input.ExpirationDateTime
            };
        }

        public static LicensedAppUpdateOutput Success(Entities.LicensedApp licensedApp)
        {
            return new()
            {
                Status = licensedApp.Status,
                AppId = licensedApp.AppId,
                OperationValidation = OperationValidation.NoError,
                LicensedBundleId = licensedApp.LicensedBundleId,
                LicensedTenantId = licensedApp.LicensedTenantId,
                NumberOfLicenses = licensedApp.NumberOfLicenses,
                AdditionalNumberOfLicenses = licensedApp.AdditionalNumberOfLicenses,
                NumberOfTemporaryLicenses = licensedApp.NumberOfTemporaryLicenses,
                ExpirationDateOfTemporaryLicenses = licensedApp.ExpirationDateOfTemporaryLicenses,
                ExpirationDateTime = licensedApp.ExpirationDateTime
            };
        }
    }
}