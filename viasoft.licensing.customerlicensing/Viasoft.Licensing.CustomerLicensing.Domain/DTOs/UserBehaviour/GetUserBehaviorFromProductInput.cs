using System;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Data.Attributes;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.UserBehaviour
{
    public class GetUserBehaviorFromProductInput : PagedFilteredAndSortedRequestInput
    {
        [StrictRequired]
        public Guid LicensingIdentifier { get; set; }
        public Guid LicensedTenantId { get; set; }
        public Guid ProductId { get; set; }
        public ProductType ProductType { get; set; }
    }
}