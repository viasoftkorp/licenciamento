using System;
using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle
{
    public class GetAllBundlesInput: PagedFilteredAndSortedRequestInput
    {
        public Guid? LicensedTenantId { get; set; }
    }
}