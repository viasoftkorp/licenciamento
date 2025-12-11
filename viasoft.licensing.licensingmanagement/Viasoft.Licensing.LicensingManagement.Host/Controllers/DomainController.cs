using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Domain;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.App;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class DomainController : BaseController
    {
        private readonly IAppRepository _appRepository;

        public DomainController(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        [HttpGet]
        public Task<Dictionary<string, Domain.Enums.Domain>> GetDomains()
        {
            var domains = new Dictionary<string, Domain.Enums.Domain>
            {
                {"VD", Domain.Enums.Domain.Sales},
                {"CP", Domain.Enums.Domain.Purchases},
                {"FA", Domain.Enums.Domain.Billing},
                {"FI", Domain.Enums.Domain.Financial},
                {"RMA", Domain.Enums.Domain.Rma},
                {"LG", Domain.Enums.Domain.Logistics},
                {"FC", Domain.Enums.Domain.Fiscal},
                {"CT", Domain.Enums.Domain.Accounting},
                {"EN", Domain.Enums.Domain.Engineering},
                {"PD", Domain.Enums.Domain.Production},
                {"MT", Domain.Enums.Domain.Maintenance},
                {"QA", Domain.Enums.Domain.QualityAssurance},
                {"RH", Domain.Enums.Domain.HumanResources},
                {"CG", Domain.Enums.Domain.Configurations},
                {"DEV", Domain.Enums.Domain.Development},
                {"CST", Domain.Enums.Domain.Customized},
                {"LS", Domain.Enums.Domain.Licensing},
                {"MOB", Domain.Enums.Domain.Mobile},
                {"REP", Domain.Enums.Domain.Reports},
                {"PROJ", Domain.Enums.Domain.Projects}
            };
            return Task.FromResult(domains);
        }

        [HttpPost]
        [TenantIdParameterHint(nameof(GetDomainsFromAppIdsInput.LicensingIdentifier), ParameterLocation.Body)]
        public async Task<Dictionary<string, Domain.Enums.Domain>> GetDomainsFromAppIds(GetDomainsFromAppIdsInput input)
        {
            return await _appRepository.GetDomainsByAppIdentifiers(input.AppsIds);
        }
    }
}