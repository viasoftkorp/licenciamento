using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment
{
    public class GetEnvironmentByNameInput
    {
        public Guid UnitId { get; set; }
        public string Name { get; set; }
    }
}