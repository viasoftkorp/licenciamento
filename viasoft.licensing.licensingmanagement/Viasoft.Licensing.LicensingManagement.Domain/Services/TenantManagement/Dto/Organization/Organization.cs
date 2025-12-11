using Viasoft.Core.DDD.Entities;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Organization
{
    public class Organization : Entity
    {
        [StrictRequired]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}