using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.ApiClient;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.CustomerLicensing.Domain.Consts;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageStatistics;
using Viasoft.Licensing.CustomerLicensing.Domain.Entities;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageStatistics
{
    public class LicenseUsageStatisticsService : ILicenseUsageStatisticsService, ITransientDependency
    {
        private readonly IRepository<Entities.LicenseUsageInRealTime> _usageInRealTime;
        private readonly IRepository<OwnedAppCount> _ownedAppCount;
        private readonly IApiClientCallBuilder _apiClientCallBuilder;

        public LicenseUsageStatisticsService(IRepository<Entities.LicenseUsageInRealTime> usageInRealTime, IRepository<OwnedAppCount> ownedAppCount, IApiClientCallBuilder apiClientCallBuilder)
        {
            _usageInRealTime = usageInRealTime;
            _ownedAppCount = ownedAppCount;
            _apiClientCallBuilder = apiClientCallBuilder;
        }

        public async Task<OnlineTenantCountOutput> GetOnlineTenantCountAsync()
        {
            var count = await _usageInRealTime
                .Select(u => u.LicensingIdentifier)
                .Distinct()
                .CountAsync();
            
            return new OnlineTenantCountOutput
            {
                TenantCount = count
            };
        }

        public async Task<OnlineUserCountOutput> GetOnlineUserCountAsync(string advancedFilter)
        {
            var (defaultSorting, ascSorting) = DefaultGetAllSorting();
            
            var countUsers = await (from usageInRealTime in _usageInRealTime.AsQueryable().ApplyAdvancedFilter(advancedFilter, defaultSorting, ascSorting) 
                    group usageInRealTime by usageInRealTime.LicensingIdentifier into realUsage
                    select new 
                    {
                        UsageCount = _usageInRealTime.Where(l => l.LicensingIdentifier == realUsage.Key).Select(l => l.User).Distinct().Count()
                    }
                ).SumAsync(u => u.UsageCount);
            
            return new OnlineUserCountOutput
            {
                OnlineUserCount = countUsers
            };
        }

        public async Task<OnlineUserCountOutput> GetOnlineUserCountAsync(string advancedFilter, Guid licensingIdentifier)
        {
            var (defaultSorting, ascSorting) = DefaultGetAllSorting();
            var count = await _usageInRealTime
                .AsQueryable()
                .ApplyAdvancedFilter(advancedFilter, defaultSorting, ascSorting)
                .Where(u => u.LicensingIdentifier == licensingIdentifier)
                .Select(u => u.User)
                .Distinct()
                .CountAsync();

            return new OnlineUserCountOutput
            {
                OnlineUserCount = count
            };
        }

        public async Task<OnlineAppsCountOutput> GetAppsInUseCountAsync(string advancedFilter)
        {
            var numberOfAppsTotalTask = GetNumberOfAppsTotal();
            
            var (defaultSorting, ascSorting) = DefaultGetAllSorting();
            var count = await _usageInRealTime
                .AsQueryable()
                .ApplyAdvancedFilter(advancedFilter, defaultSorting, ascSorting)
                .Select(u => u.AppIdentifier)
                .Distinct()
                .CountAsync();

            var numberOfAppsTotal = await numberOfAppsTotalTask;

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

        public async Task<List<Guid>> GetLicenseIdentifiersForUsageReporting(string advancedFilter)
        {
            var (defaultSorting, ascSorting) = DefaultGetAllSorting();
            return await _usageInRealTime
                .AsQueryable()
                .ApplyAdvancedFilter(advancedFilter, defaultSorting, ascSorting)
                .Select(u => u.LicensingIdentifier)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<LicenseUsageReportingOutput>> GetLicenseUsageForReporting()
        {
            var licenseByIdentifierGroups = await (from usageInRealTime in _usageInRealTime 
                    group usageInRealTime by usageInRealTime.LicensingIdentifier into realUsage
                    select new LicenseUsageReportingOutput
                    {
                        LicensingIdentifier = realUsage.Key,
                        UsageCount = _usageInRealTime.Where(l => l.LicensingIdentifier == realUsage.Key).Select(l => l.User).Distinct().Count()
                    }
                    ).ToListAsync();
            return licenseByIdentifierGroups;
        }

        private async Task<NumberOfAppsTotalOutput> GetNumberOfAppsTotal()
        {
            var gatewayCall = _apiClientCallBuilder
                .WithEndpoint(ExternalServicesConsts.LicensingManagement.LicensingManagementStatistics.GetNumberOfAppsInTotal)
                .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                
                .Build();

            var gatewayCallResponse = await gatewayCall.ResponseCallAsync<NumberOfAppsTotalOutput>();

            return gatewayCallResponse;
        }

        private static (Expression<Func<Entities.LicenseUsageInRealTime, string>>, bool) DefaultGetAllSorting()
        {
            return (license => license.User, true);
        }
    }
}