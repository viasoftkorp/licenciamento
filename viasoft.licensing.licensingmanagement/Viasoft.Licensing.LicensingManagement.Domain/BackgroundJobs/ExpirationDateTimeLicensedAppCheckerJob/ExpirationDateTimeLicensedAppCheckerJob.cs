using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.BackgroundJobs.Abstractions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.ExpirationDateTimeLicensedAppCheckerJob;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensedApp;

namespace Viasoft.Licensing.LicensingManagement.Domain.BackgroundJobs.ExpirationDateTimeLicensedAppCheckerJob
{
    [UserNotRequired]
    public class ExpirationDateTimeLicensedAppCheckerJob : IBackgroundJob<ExpirationDateTimeLicensedAppCheckerJobData>
    {
        private readonly IRepository<Entities.LicensedApp> _licensedApps;
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        private readonly ILicensedAppService _licensedAppService;
        private readonly IUnitOfWork _unitOfWork;

        public ExpirationDateTimeLicensedAppCheckerJob(IRepository<Entities.LicensedApp> licensedApps, IRepository<Entities.LicensedTenant> licensedTenants,ILicensedAppService licensedAppService, IUnitOfWork unitOfWork)
        {
            _licensedApps = licensedApps;
            _licensedTenants = licensedTenants;
            _licensedAppService = licensedAppService;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(ExpirationDateTimeLicensedAppCheckerJobData input)
        {
            var utcDate = DateTime.UtcNow;
            
                Expression<Func<Entities.LicensedApp, bool>> overdueQuery = l => l.ExpirationDateTime < utcDate && l.Status != LicensedAppStatus.AppBlocked;

            var overdueApps = await _licensedApps.Where(overdueQuery)
                .Join(_licensedTenants, e => e.LicensedTenantId, e => e.Id, (k, p) => new
                {
                    licensedApp = k,
                    licensedTennat = p
                })
                .ToDictionaryAsync(e => e.licensedApp, e => e.licensedTennat);

            using (_unitOfWork.Begin())
            {
                foreach (var (app, tenant) in overdueApps)
                {
                    var updateLicensedAppInput = new LicensedAppUpdateInput()
                    {
                        Status = LicensedAppStatus.AppBlocked,
                        AppId = app.AppId,
                        LicensingMode = app.LicensingMode,
                        LicensingModel = app.LicensingModel,
                        LicensedBundleId = app.LicensedBundleId,
                        LicensedTenantId = app.LicensedTenantId,
                        NumberOfLicenses = app.NumberOfLicenses,
                        AdditionalNumberOfLicenses = app.AdditionalNumberOfLicenses,
                        NumberOfTemporaryLicenses = app.NumberOfTemporaryLicenses,
                        ExpirationDateOfTemporaryLicenses = app.ExpirationDateOfTemporaryLicenses,
                        ExpirationDateTime = app.ExpirationDateTime
                    };
                    await _licensedAppService.UpdateLicensedApp(tenant, app, updateLicensedAppInput);
                }
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}