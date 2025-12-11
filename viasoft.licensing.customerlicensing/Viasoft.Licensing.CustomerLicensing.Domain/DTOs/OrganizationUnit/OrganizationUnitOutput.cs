using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.OrganizationUnit
{
    public class OrganizationUnitOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid TenantId { get; set; }
        public bool IsActive { get; set; }
    }
}