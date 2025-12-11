using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Company;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.ExternalCompanySearch
{
    public interface IExternalCompanySearchService
    {
        Task<CompanyDto> GetCompanyByCnpj(string cnpj, Guid id);
    }
}