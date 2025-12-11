using System;
using System.Linq.Expressions;
using AutoMapper;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.AuditingLog;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class AuditingLogController: BaseReadonlyController<AuditingLog, AuditingLogOutput, GetAllAuditingLogInput, string>
    {
        public AuditingLogController(IReadOnlyRepository<AuditingLog> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override (Expression<Func<AuditingLog, string>>, bool) DefaultGetAllSorting()
        {
            return (l => l.UserName, true);
        }
    }
}