using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.BundledApps
{
    public interface IRemoveDuplicatedBundledAppsService
    {
        public Task<List<BundledAppsOutput>> GetBundledApps(List<Guid> bundleIds, List<Guid> defaultAppIds);
    }
}