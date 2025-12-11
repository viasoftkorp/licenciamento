using System;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Organization
{
    public class CreateOrUpdateOrganizationInput
    {
        public Guid Id { get; set; }
        [StrictRequired]
        public string Name { get; set; }
        [StrictRequired]
        public Guid TenantId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}