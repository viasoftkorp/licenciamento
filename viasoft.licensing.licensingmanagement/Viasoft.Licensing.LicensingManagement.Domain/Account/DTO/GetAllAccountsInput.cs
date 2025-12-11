using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.DTO
{
    public class GetAllAccountsInput : PagedFilteredAndSortedRequestInput
    {
        public bool OnlyActiveAccounts { get; set; }
    }
}