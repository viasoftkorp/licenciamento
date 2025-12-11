using System;
using System.Collections.Generic;
using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment
{
    public class GetEnvironmentByUnitInput : PagedFilteredAndSortedRequestInput
    {
        public Guid? UnitId { get; set; }
        public List<Guid> TenantIds { get; set; }
        public bool? ActiveOnly { get; set; }
        public bool? DesktopOnly { get; set; }
        public bool? WebOnly { get; set; }
        public bool? MobileOnly { get; set; }
        public bool? ProductionOnly { get; set; }
    }
}