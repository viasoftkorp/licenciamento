using Viasoft.Core.DDD.Application.Dto.BaseCrud;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle
{
    public class BundleDeleteOutput : BaseCrudDefaultResponse<OperationValidation>
    {
        public bool IsBundleLicensedInAnyLicensing { get; set; }
    }
}