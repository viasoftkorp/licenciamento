using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageInRealTime;
using Viasoft.Licensing.CustomerLicensing.Domain.Messages;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageInRealTime
{
    public interface ILicenseUsageInRealTimeService
    {
        Task UpdateLicensedApps(LicensingDetailsUpdated licensingDetails);
        Task ImportLicenseUsage(LicenseUsageInRealTimeImportInput input);
        Task<Dictionary<string, int>> GetLicensesConsumed(Guid licensingIdentifier, List<string> bundleIdentifiers, List<string> appIdentifiers);
    }
}