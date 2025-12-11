using System;
using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.Environment
{
    public class GetEnvironmentByUnitInput : PagedFilteredAndSortedRequestInput
    {
        public Guid UnitId { get; set; }
        public bool? ActiveOnly { get; set; }
        public bool? DesktopOnly { get; set; }
        public bool? WebOnly { get; set; }
        public bool? ProductionOnly { get; set; }
        public bool? MobileOnly { get; set; }
    }
}