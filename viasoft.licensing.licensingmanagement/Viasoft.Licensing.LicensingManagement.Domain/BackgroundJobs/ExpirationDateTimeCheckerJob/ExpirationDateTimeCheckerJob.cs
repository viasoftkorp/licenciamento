using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.BackgroundJobs.Abstractions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.ExpirationDateTimeCheckerJob;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;

namespace Viasoft.Licensing.LicensingManagement.Domain.BackgroundJobs.ExpirationDateTimeCheckerJob
{
    [UserNotRequired]
    public class ExpirationDateTimeCheckerJob : IBackgroundJob<ExpirationDateTimeCheckerJobData>
    {
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        private readonly ILicensedTenantService _licensedTenantService;
        private readonly IMapper _mapper;

        public ExpirationDateTimeCheckerJob(IRepository<Entities.LicensedTenant> licensedTenants, ILicensedTenantService licensedTenantService, IMapper mapper)
        {
            _licensedTenants = licensedTenants;
            _licensedTenantService = licensedTenantService;
            _mapper = mapper;
        }

        public async Task ExecuteAsync(ExpirationDateTimeCheckerJobData input)
        {
            var utcDate = DateTime.UtcNow;
            Expression<Func<Entities.LicensedTenant, bool>> overdueQuery = l => l.ExpirationDateTime < utcDate && l.Status != LicensingStatus.Blocked;

            var overdueTenants = await _licensedTenants.Where(overdueQuery).ToListAsync();

            foreach (var tenant in overdueTenants)
            {
                var updateTenantInput = _mapper.Map<LicenseTenantUpdateInput>(tenant);
                updateTenantInput.Status = LicensingStatus.Blocked;
                updateTenantInput.Notes = tenant.NotesString;
                await _licensedTenantService.UpdateTenantLicensing(updateTenantInput);
            }
        }
    }
}