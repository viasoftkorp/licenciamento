using System.Collections.Generic;
using Viasoft.Core.DDD.Application.Dto.BaseCrud;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO
{
    public class LicenseTenantDeleteOutput: BaseCrudDefaultResponse<OperationValidation>
    {
        public static LicenseTenantDeleteOutput Fail(OperationValidation validation)
        {
            return new()
            {
                Success = false,
                Errors = new List<BaseCrudResponseError<OperationValidation>>
                {
                    new()
                    {
                        ErrorCode = validation
                    }
                }
            };
        }
    }
}