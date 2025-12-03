using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Messages;

namespace Viasoft.Licensing.LicenseServer.Domain.Catalogs
{
    public interface ITenantCatalog
    {
        Task<List<LicenseTenantStatusCurrent>> GetAllTenantCurrentLicenseStatus();
        Task<LicenseTenantStatusCurrent> GetTenantCurrentLicenseStatus(Guid tenantId);
        Task<ConsumeLicenseOutput> ConsumeLicense(ConsumeLicenseInput input);
        Task<ReleaseLicenseOutput> ReleaseLicense(ReleaseLicenseInput input);
        Task<List<LicenseUsageInRealTimeRawData>> GetTenantsLicensesUsageInRealTime();
        Task ReleaseLicenseBasedOnHeartbeat();
        Task<RefreshAppLicenseInUseByUserOutput> RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInput input);
        Task RefreshAllTenantsLicensing();
        Task RefreshTenantLicensing(LicensingDetailsUpdated licensingDetailsUpdated);
    }
}