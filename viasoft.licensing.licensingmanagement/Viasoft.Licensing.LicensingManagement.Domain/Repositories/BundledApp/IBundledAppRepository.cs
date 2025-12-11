using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp;

namespace Viasoft.Licensing.LicensingManagement.Domain.Repositories.BundledApp
{
    public interface IBundledAppRepository
    {
        public Task<List<BundledAppsOutput>> GetBundledAppsFromList(List<Guid> bundleIds);
    }
}