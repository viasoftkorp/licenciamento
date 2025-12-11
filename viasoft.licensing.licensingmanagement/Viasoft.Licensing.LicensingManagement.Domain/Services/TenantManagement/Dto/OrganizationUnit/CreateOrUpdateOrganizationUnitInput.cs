using System;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.OrganizationUnit
{
    public class CreateOrUpdateOrganizationUnitInput
    {
        public Guid Id { get; set; }
        [StrictRequired]
        public string Name { get; set; }
        public string Description { get; set; }
        [StrictRequired]
        public Guid OrganizationId { get; set; }
        public bool IsActive { get; set; }
    }
}