using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment
{
    public class OrganizationUnitEnvironmentOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsProduction { get; set; }
        public bool IsWeb { get; set; }
        public bool IsDesktop { get; set; }
        public bool IsMobile { get; set; }
        public string DatabaseName { get; set; }
        public Guid OrganizationUnitId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid TenantId { get; set; }
        public bool IsActive { get; set; }
        public string DesktopDatabaseVersion { get; set; }
    }
}