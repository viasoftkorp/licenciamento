using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageStatistics;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageStatistics
{
    public interface ILicenseUsageStatisticsService
    {
        Task<OnlineTenantCountOutput> GetOnlineTenantCountAsync();
        Task<OnlineUserCountOutput> GetOnlineUserCountAsync(string advancedFilter);
        Task<OnlineUserCountOutput> GetOnlineUserCountAsync(string advancedFilter, Guid licensingIdentifier);
        Task<OnlineAppsCountOutput> GetAppsInUseCountAsync(string advancedFilter);
        Task<OnlineAppsCountOutput> GetAppsInUseCountAsync(string advancedFilter, Guid licensingIdentifier);
        Task<List<Guid>> GetLicenseIdentifiersForUsageReporting(string advancedFilter);
        Task<List<LicenseUsageReportingOutput>> GetLicenseUsageForReporting();
    }
}