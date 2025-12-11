using System;
using System.Linq.Expressions;
using AutoMapper;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.DTO;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class LicensedTenantViewController : BaseReadonlyController<LicensedTenantView, LicensedTenantViewOutput, GetAllLicensedTenantViewInput, string>
    {
        public LicensedTenantViewController(IReadOnlyRepository<LicensedTenantView> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override (Expression<Func<LicensedTenantView, String>>, bool) DefaultGetAllSorting()
        {
            return (l => l.AccountCompanyName, true);
        }
    }
}