using System;
using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.UserBehaviour
{
    public class GetAllLicenseUserBehaviour : PagedFilteredAndSortedRequestInput
    {
        public string TzDatabaseName { get; set; } = "America/Sao_Paulo";
        public Guid? LicensingIdentifier { get; set; }
    }
}