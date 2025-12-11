using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Software
{
    public class GetAllSoftwaresInput: PagedFilteredAndSortedRequestInput
    {
        public bool OnlyActiveSoftwares { get; set; }
    }
}