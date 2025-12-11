using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO
{
    public class DeleteLicenseOutput
    {
        public OperationValidation OperationValidation { get; set; }
        
        public bool Success { get; set; }
    }
}