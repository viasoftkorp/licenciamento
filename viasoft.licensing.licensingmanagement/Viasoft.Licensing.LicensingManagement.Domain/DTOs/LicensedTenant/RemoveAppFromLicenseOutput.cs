using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant
{
    public class RemoveAppFromLicenseOutput
    {
        public OperationValidation ErrorCode { get; set; }
        public bool Success { get; set; }

        public static RemoveAppFromLicenseOutput Fail(OperationValidation operationValidation)
        {
            return new()
            {
                Success = false,
                ErrorCode = operationValidation
            };
        }
    }
}