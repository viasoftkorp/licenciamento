using System;
using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle
{
    public class GetAllLicensedBundleInput : PagedFilteredAndSortedRequestInput
    {
        public Guid LicensedTenantId { get; set; }
    }
}