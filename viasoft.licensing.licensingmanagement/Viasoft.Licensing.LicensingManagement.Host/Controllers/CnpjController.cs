using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Company;
using Viasoft.Licensing.LicensingManagement.Domain.Services.ExternalCompanySearch;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class CnpjController : BaseController
    {
        private readonly IExternalCompanySearchService _searchAccountService;
        private readonly IMapper _mapper;
        public CnpjController(IExternalCompanySearchService searchAccountService, IMapper mapper)
        {
            _mapper = mapper;
            _searchAccountService = searchAccountService;
        }
        
        [HttpGet]
        public async Task<GetCompanyByCnpjOutput> GetCompanyByCnpj(string cnpj, Guid id)
        {
            return _mapper.Map<GetCompanyByCnpjOutput>(await _searchAccountService.GetCompanyByCnpj(cnpj, id));
        }
    }
}