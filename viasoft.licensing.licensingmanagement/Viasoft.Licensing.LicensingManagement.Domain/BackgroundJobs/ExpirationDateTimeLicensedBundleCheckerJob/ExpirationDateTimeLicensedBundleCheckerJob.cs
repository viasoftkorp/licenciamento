using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.BackgroundJobs.Abstractions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.ExpirationDateTimeLicensedBundleCheckerJob;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Services.LicensedBundle;

namespace Viasoft.Licensing.LicensingManagement.Domain.BackgroundJobs.ExpirationDateTimeLicensedBundleCheckerJob
{
    [UserNotRequired]
    public class ExpirationDateTimeLicensedBundleCheckerJob : IBackgroundJob<ExpirationDateTimeLicensedBundleCheckerJobData>
    {
        private readonly IRepository<Entities.LicensedBundle> _licensedBundles;
        private readonly IRepository<Entities.LicensedTenant> _licensedTenants;
        private readonly ILicensedBundleService _licensedBundleService;
        private readonly IUnitOfWork _unitOfWork;

        public ExpirationDateTimeLicensedBundleCheckerJob(IRepository<Entities.LicensedBundle> licensedBundles, IRepository<Entities.LicensedTenant> licensedTenants, ILicensedBundleService licensedBundleService, IUnitOfWork unitOfWork)
        {
            _licensedBundles = licensedBundles;
            _licensedTenants = licensedTenants;
            _licensedBundleService = licensedBundleService;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(ExpirationDateTimeLicensedBundleCheckerJobData input)
        {
            var utcDate = DateTime.UtcNow;
            
            Expression<Func<Entities.LicensedBundle, bool>> overdueQuery = l => l.ExpirationDateTime < utcDate && l.Status != LicensedBundleStatus.BundleBlocked;

            var overdueBundles = await _licensedBundles.Where(overdueQuery)
                .Join(_licensedTenants, e => e.LicensedTenantId, e => e.Id, (k, p) => new
                {
                    licensedBundle = k,
                    licensedTennat = p
                })
                .ToDictionaryAsync(e => e.licensedBundle, e => e.licensedTennat);

            using (_unitOfWork.Begin())
            {
                foreach (var (bundle, tenant) in overdueBundles)
                {
                    var updateLicensedAppInput = new LicensedBundleUpdateInput()
                    {
                        Status = LicensedBundleStatus.BundleBlocked,
                        BundleId = bundle.BundleId,
                        LicensingMode = bundle.LicensingMode,
                        LicensingModel = bundle.LicensingModel,
                        LicensedTenantId = bundle.LicensedTenantId,
                        NumberOfLicenses = bundle.NumberOfLicenses,
                        NumberOfTemporaryLicenses = bundle.NumberOfTemporaryLicenses,
                        ExpirationDateOfTemporaryLicenses = bundle.ExpirationDateOfTemporaryLicenses,
                        ExpirationDateTime = bundle.ExpirationDateTime
                    };
                    await _licensedBundleService.UpdateLicensedBundle(tenant, bundle, updateLicensedAppInput);
                }
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}