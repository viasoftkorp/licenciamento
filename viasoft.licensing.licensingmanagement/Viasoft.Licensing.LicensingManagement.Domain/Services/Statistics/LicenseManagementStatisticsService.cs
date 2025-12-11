using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Statistics;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.Statistics
{
    public class LicenseManagementStatisticsService : ILicenseManagementStatisticsService, ITransientDependency
    {
        private readonly IRepository<Entities.LicensedApp> _licensedApps;

        public LicenseManagementStatisticsService(IRepository<Entities.LicensedApp> licensedApps)
        {
            _licensedApps = licensedApps;
        }

        public async Task<NumberOfAppsTotalOutput> GetNumberOfAppsInTotal()
        {
            var count = await _licensedApps.Select(x => x.AppId).Distinct().CountAsync();
            return new NumberOfAppsTotalOutput
            {
                NumberOfAppsTotal = count
            };
        }
    }
}