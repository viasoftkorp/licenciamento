using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Messages;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicensedTenantOrchestrator
{
    public interface ILicensedTenantOrchestratorService
    {
        Task<LicenseTenantStatusCurrentOld> GetTenantCurrentLicenseStatus(Guid tenantId);

        Task<ConsumeLicenseOutputOld> ConsumeLicense(ConsumeLicenseInput input);
        
        Task<ReleaseLicenseOutputOld> ReleaseLicense(ReleaseLicenseInput input);

        IEnumerable<LicenseUsageInRealTime> GetTenantsLicensesUsageInRealTime();
        
        Task ReleaseLicenseBasedOnHeartbeat();
        
        Task<RefreshAppLicenseInUseByUserOutputOld> RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInputOld inputOld);

        Task<TenantLicensedAppsOutput> GetTenantLicensedApps(Guid tenantId);
        
        Task<TenantLicenseStatusOutput> GetTenantLicenseStatus(Guid tenantId);
        
        Task<IsTenantCnpjLicensedOutput> IsTenantCnpjLicensed(Guid tenantId, string cnpj);
        
        Task RefreshAllTenantsLicensing();
        
        Task RefreshTenantLicensing(LicensingDetailsUpdated licensingDetailsUpdated);
    }
}