using System;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.OrganizationUnit
{
    public class CreateOrUpdateOrganizationUnitInput
    {
        [StrictRequired]
        public Guid Id { get; set; }
        [StrictRequired]
        public string Name { get; set; }
        public string Description { get; set; }
        [StrictRequired]
        public Guid OrganizationId { get; set; }
        public bool IsActive { get; set; }
    }
}