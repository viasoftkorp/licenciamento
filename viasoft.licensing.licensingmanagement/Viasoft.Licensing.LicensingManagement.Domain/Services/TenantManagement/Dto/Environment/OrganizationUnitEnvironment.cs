using System;
using Viasoft.Core.DDD.Entities;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment
{
    public class OrganizationUnitEnvironment : Entity
    {
        [StrictRequired]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsProduction { get; set; }
        public bool IsMobile { get; set; }
        public bool IsWeb { get; set; }
        public bool IsDesktop { get; set; }
        public string DatabaseName { get; set; }
        public Guid OrganizationUnitId { get; set; }
        public bool IsActive { get; set; }
        public string DesktopDatabaseVersion { get; set; }
    }
}