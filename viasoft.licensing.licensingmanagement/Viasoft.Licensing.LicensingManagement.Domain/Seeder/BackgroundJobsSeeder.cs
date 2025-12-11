using System;
using System.Threading.Tasks;
using Hangfire;
using Viasoft.Core.BackgroundJobs.Abstractions;
using Viasoft.Core.BackgroundJobs.Abstractions.Manager;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Data.Seeder.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.ExpirationDateTimeCheckerJob;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.ExpirationDateTimeLicensedAppCheckerJob;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.ExpirationDateTimeLicensedBundleCheckerJob;

namespace Viasoft.Licensing.LicensingManagement.Domain.Seeder
{
    public class BackgroundJobsSeeder : ISeedData
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ICurrentTenant _currentTenant;

        public BackgroundJobsSeeder(IBackgroundJobManager backgroundJobManager, ICurrentTenant currentTenant)
        {
            _backgroundJobManager = backgroundJobManager;
            _currentTenant = currentTenant;
        }
        public async Task SeedDataAsync()
        {
            if (_currentTenant.Id == Guid.Empty)
                return;
            
            // The CurrentTenantId is always a hostTenantId, thanks to the contributor LicensingIdentifierToTenantIdContributor
            await _backgroundJobManager.AddOrUpdateRecurringJobAsync(new ExpirationDateTimeCheckerJobData(),
                Guid.Parse("F6B9347A-8347-4F7F-8516-403EA7B95F65"), Cron.Daily(3), JobIdKeyStrategy.TenantIdOnly);
            
            await _backgroundJobManager.AddOrUpdateRecurringJobAsync(new ExpirationDateTimeLicensedAppCheckerJobData(),
                Guid.Parse("03A79096-9DB1-4BB5-AC04-99FD355710AF"), Cron.Daily(3), JobIdKeyStrategy.TenantIdOnly);
         
            await _backgroundJobManager.AddOrUpdateRecurringJobAsync(new ExpirationDateTimeLicensedBundleCheckerJobData(),
                Guid.Parse("A384797B-0C73-43F3-82A9-659055C7E709"), Cron.Daily(3), JobIdKeyStrategy.TenantIdOnly);
        }
    }
}