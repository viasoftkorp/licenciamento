using System;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp
{
    public class GetAllLicensedAppInput : PagedFilteredAndSortedRequestInput
    {
        public Guid LicensedTenantId { get; set; }
        
        [StrictRequired]
        public Guid LicensedBundleId { get; set; }
    }
}