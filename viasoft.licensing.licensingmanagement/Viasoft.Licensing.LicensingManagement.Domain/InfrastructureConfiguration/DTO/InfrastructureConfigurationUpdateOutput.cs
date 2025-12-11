using System;
using Viasoft.Core.DDD.Application.Dto.BaseCrud;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.DTO
{
    public class InfrastructureConfigurationUpdateOutput: BaseCrudDefaultResponse<OperationValidation>
    {
        public Guid LicensedTenantId { get; set; }
        public string GatewayAddress { get; set; }
        public string DesktopDatabaseName { get; set; }
    }
}