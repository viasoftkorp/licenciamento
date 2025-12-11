using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.ZipCode;
using Viasoft.Licensing.LicensingManagement.Domain.Services.ZipCodeSearch;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class ZipCodeController
    {
        private readonly IMapper _mapper;
        private readonly IZipCodeSearchService _zipCodeSearchService;

        public ZipCodeController(IMapper mapper, IZipCodeSearchService zipCodeSearchService)
        {
            _mapper = mapper;
            _zipCodeSearchService = zipCodeSearchService;
        }


        [HttpGet]
        public async Task<ZipCodeAdressDto> GetAddressByZipcode([FromQuery]GetAdressInput input)
        {
            var addressResponse = await _zipCodeSearchService.GetAddress(input.Zipcode);
            var output = _mapper.Map<ZipCodeAdressDto>(addressResponse);
            return output;

        }
    }
}