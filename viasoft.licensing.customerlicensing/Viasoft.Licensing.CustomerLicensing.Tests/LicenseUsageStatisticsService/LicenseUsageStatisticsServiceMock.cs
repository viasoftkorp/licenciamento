using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageStatistics;
using Viasoft.Licensing.CustomerLicensing.Domain.Entities;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageStatistics;

namespace Viasoft.Licensing.CustomerLicensing.Tests.LicenseUsageStatisticsService
{
    public class LicenseUsageStatisticsServiceMock : ILicenseUsageStatisticsService
    {
        private readonly IRepository<LicenseUsageInRealTime> _usageInRealTime;
        private readonly IRepository<OwnedAppCount> _ownedAppCount;

        public LicenseUsageStatisticsServiceMock(IRepository<LicenseUsageInRealTime> usageInRealTime, IRepository<OwnedAppCount> ownedAppCount)
        {
            _usageInRealTime = usageInRealTime;
            _ownedAppCount = ownedAppCount;
        }

        public Task<OnlineTenantCountOutput> GetOnlineTenantCountAsync()
        {
            return null;
        }

        public Task<OnlineUserCountOutput> GetOnlineUserCountAsync(string advancedFilter)
        {
            return null;
        }

        public Task<OnlineUserCountOutput> GetOnlineUserCountAsync(string advancedFilter, Guid licensingIdentifier)
        {
            return null;
        }

        public async Task<OnlineAppsCountOutput> GetAppsInUseCountAsync(string advancedFilter)
        {
            var (defaultSorting, ascSorting) = DefaultGetAllSorting();
            var count = await _usageInRealTime
                .AsQueryable()
                .ApplyAdvancedFilter(advancedFilter, defaultSorting, ascSorting)
                .Select(u => u.AppIdentifier)
                .Distinct()
                .CountAsync();

            var numberOfAppsTotal = new NumberOfAppsTotalOutput
            {
                NumberOfAppsTotal = 10
            };

            return new OnlineAppsCountOutput
            {
                TotalApps = numberOfAppsTotal.NumberOfAppsTotal,
                AppsInUse = count
            };
        }

        public async Task<OnlineAppsCountOutput> GetAppsInUseCountAsync(string advancedFilter, Guid licensingIdentifier)
        {
            var (defaultSorting, ascSorting) = DefaultGetAllSorting();
            var count = await _usageInRealTime
                .AsQueryable()
                .ApplyAdvancedFilter(advancedFilter, defaultSorting, ascSorting)
                .Where(u => u.LicensingIdentifier == licensingIdentifier)
                .Select(u => u.AppIdentifier)
                .Distinct()
                .CountAsync();

            var totalCount = await _ownedAppCount.FirstOrDefaultAsync(h => h.LicensingIdentifier == licensingIdentifier);

            if (totalCount != null)
            {
                return new OnlineAppsCountOutput
                {
                    TotalApps = totalCount.Count,
                    AppsInUse = count
                };
            }
            return new OnlineAppsCountOutput
            {
                TotalApps = 0,
                AppsInUse = count
            };
        }

        public Task<List<Guid>> GetLicenseIdentifiersForUsageReporting(string advancedFilter)
        {
            return null;
        }

        public Task<List<LicenseUsageReportingOutput>> GetLicenseUsageForReporting()
        {
            return null;
        }

        private static (Expression<Func<LicenseUsageInRealTime, string>>, bool) DefaultGetAllSorting()
        {
            return (license => license.User, true);
        }
        
    }
}