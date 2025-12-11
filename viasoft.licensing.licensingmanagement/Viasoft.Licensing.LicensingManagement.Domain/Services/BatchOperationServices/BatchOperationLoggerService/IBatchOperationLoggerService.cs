using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.BatchOperationServices.BatchOperationLoggerService
{
    public interface IBatchOperationLoggerService
    {
        Task LogInsertAppsInBundles(List<string> appIdentifiers, List<string> bundleIdentifiers);

        Task LogInsertAppsFromBundlesInLicenses(List<string> appIdentifiers, List<Guid> licenseTenantIdentifiers);

        Task LogRemoveAppFromBundleInLicenses(string appIdentifier, List<Guid> licenseTenantIdentifiers);

        Task LogInsertBundleInLicenses(int licensesNumber, List<string> bundleIdentifiers, List<Guid> licenseTenantIdentifiers);

        Task LogAppsInLicenses(int licensesNumber, List<string> appIdentifiers, List<Guid> licenseTenantIdentifiers);
    }
}