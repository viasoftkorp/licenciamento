using System;
using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp
{
    public class GetAllAppsInBundleInput: PagedFilteredAndSortedRequestInput
    {
        public Guid BundleId { get; set; }
    }
}