using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.ZipCode;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.ZipCodeSearch
{
    public interface IZipCodeSearchService
    {
        Task<ZipCodeResponseDto> GetAddress(string cep);
    }
}