using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.UserBehaviour;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseUsage
{
    public interface ILicenseUsageService
    {
        void StoreDoneUsageLog(StoreDoneUsageLog input);
        Task<List<LicenseUsageBehaviourDetails>> GetLicensesUsage(Guid tenantId);
        LicenseUsageInRealTime GetLastUploadedLicenseUsageInRealTime(Guid tenantId);
        void StoreLastUploadedLicenseUsageInRealTime(LicenseUsageInRealTime licenseUsageInRealTime);
    }
}