using System;
using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.TenantManagement.Dto.OrganizationUnit
{
    public class GetByOrganizationInput : PagedFilteredAndSortedRequestInput
    {
        public Guid OrganizationId { get; set; }
    }
}