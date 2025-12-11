using System;
using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageInRealTime
{
    public class GetAllLicenseUsageInRealTimeInput : PagedFilteredAndSortedRequestInput
    {
        public Guid TenantId { get; set; }
    }
}