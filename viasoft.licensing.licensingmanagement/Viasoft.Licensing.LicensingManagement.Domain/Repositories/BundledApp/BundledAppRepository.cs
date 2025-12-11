using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.BundledApp
{
    public class BundledAppRepository: IBundledAppRepository, ITransientDependency
    {
        private readonly IRepository<Entities.BundledApp> _bundledApps;

        public BundledAppRepository(IRepository<Entities.BundledApp> bundledApps)
        {
            _bundledApps = bundledApps;
        }

        public async Task<List<BundledAppsOutput>> GetBundledAppsFromList(List<Guid> bundleIds)
        {
            var output = new List<BundledAppsOutput>();

            foreach (var bundleId in bundleIds)
            {
                var apps = await _bundledApps.Where(b => b.BundleId == bundleId).Select(b => new BundledAppsOutput
                {
                    AppId = b.AppId,
                    BundleId = b.BundleId
                }).ToListAsync();
                output.AddRange(apps);
            }

            return output;
        }
    }
}