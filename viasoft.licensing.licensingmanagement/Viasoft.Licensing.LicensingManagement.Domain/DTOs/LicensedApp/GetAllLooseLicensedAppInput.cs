using System;
using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp
{
    public class GetAllLooseLicensedAppInput : PagedFilteredAndSortedRequestInput
    {
        public Guid LicensedTenantId { get; set; }
    }
}