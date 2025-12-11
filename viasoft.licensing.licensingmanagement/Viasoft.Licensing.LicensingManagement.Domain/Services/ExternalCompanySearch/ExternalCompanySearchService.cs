using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Service;
using Viasoft.Licensing.LicensingManagement.Domain.Consts;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Company;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Extensions;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.ExternalCompanySearch
{
    public class ExternalCompanySearchService : IExternalCompanySearchService, ITransientDependency
    {
        private readonly IHttpClientFactory _factory;
        private readonly IRepository<Entities.Account> _accounts;
        private readonly IAccountsService _accountsService;
        private readonly IConfiguration _configuration;

        public ExternalCompanySearchService(IHttpClientFactory factory, IRepository<Entities.Account> accounts, IAccountsService accountsService, IConfiguration configuration)
        {
            _factory = factory;
            _accounts = accounts;
            _accountsService = accountsService;
            _configuration = configuration;
        }
        
        public async Task<CompanyDto> GetCompanyByCnpj(string cnpj, Guid id)
        {
            if (_accountsService.CheckIfCnpjIsAlreadyRegistered(cnpj, id))
            {
                var account = _accounts.First(acc => acc.CnpjCpf == cnpj);
                return new CompanyDto
                {
                    OperationValidation = OperationValidation.CnpjAlreadyRegistered,
                     Id = account.Id,
                     Name = account.CompanyName
                };
            }
            using var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            client.DefaultRequestHeaders.Add("x-api-key", _configuration.GetCompanySearchToken());
            var response = await client.GetAsync(KorpApiConsts.GetCompanyByCnpj(cnpj));
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CompanyDto>(result);
        }
    }
}