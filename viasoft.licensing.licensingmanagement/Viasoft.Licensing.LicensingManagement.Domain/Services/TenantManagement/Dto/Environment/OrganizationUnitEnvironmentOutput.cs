using System;
using Viasoft.Core.DDD.Application.Dto.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment
{
    public class OrganizationUnitEnvironmentOutput : EntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsProduction { get; set; }
        public bool IsMobile { get; set; }
        public bool IsWeb { get; set; }
        public bool IsDesktop { get; set; }
        public string DatabaseName { get; set; }
        public Guid OrganizationUnitId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid TenantId { get; set; }
        public bool IsActive { get; set; }
    }
}