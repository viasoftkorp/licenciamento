using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.BundledApp;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.BundledApps
{
    public class RemoveDuplicatedBundledAppsService: IRemoveDuplicatedBundledAppsService, ITransientDependency
    {
        private readonly IBundledAppRepository _bundledAppRepository;

        public RemoveDuplicatedBundledAppsService(IBundledAppRepository bundledAppRepository)
        {
            _bundledAppRepository = bundledAppRepository;
        }

        public async Task<List<BundledAppsOutput>> GetBundledApps(List<Guid> bundleIds, List<Guid> defaultAppIds)
        {
            var list = await _bundledAppRepository.GetBundledAppsFromList(bundleIds); 
            list.RemoveAll(b => defaultAppIds.Contains(b.AppId));
            var output = list.GroupBy(b => b.AppId).Select(g => g.First()).ToList();
            return output;
        }
    }
}