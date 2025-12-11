using System;
using Viasoft.Core.DDD.Entities;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.OrganizationUnit
{
    public class OrganizationUnit : Entity
    {
        [StrictRequired]
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid OrganizationId { get; set; }
        public bool IsActive { get; set; }
    }
}