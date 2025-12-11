using System;
using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.FileQuota
{
    public class GetAllAppQuotaInput: PagedFilteredAndSortedRequestInput
    {
        public Guid LicensedTenantId { get; set; }
    }
}