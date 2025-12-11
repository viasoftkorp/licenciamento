using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp
{
    public class BundledAppCreateOutput
    {
        public Guid BundleId { get; set; }
        
        public Guid AppId { get; set; }
        
        public OperationValidation OperationValidation { get; set; }

        public string OperationValidationDescription => OperationValidation.ToString();
        
        public bool IsBundleLicensedInAnyLicensing { get; set; }
    }
}