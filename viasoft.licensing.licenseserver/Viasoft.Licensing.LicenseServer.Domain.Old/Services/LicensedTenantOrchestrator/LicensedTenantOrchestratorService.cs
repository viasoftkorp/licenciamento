using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Catalogs;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Messages;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicensedTenantOrchestrator
{
    public class LicensedTenantOrchestratorService: ILicensedTenantOrchestratorService, ISingletonDependency
    {
        private readonly TenantCatalog _tenantCatalog;

        public LicensedTenantOrchestratorService(IServiceProvider serviceProvider, IConfiguration configuration, IProvideHardwareIdService provideHardwareIdService)
        {
            _tenantCatalog = new TenantCatalog(serviceProvider, configuration, provideHardwareIdService);
        }

        public IEnumerable<LicenseUsageInRealTime> GetTenantsLicensesUsageInRealTime()
        {
            return _tenantCatalog.GetTenantsLicensesUsageInRealTime();
        }

        public async Task ReleaseLicenseBasedOnHeartbeat()
        {
            await _tenantCatalog.ReleaseLicenseBasedOnHeartbeat();
        }

        public Task<RefreshAppLicenseInUseByUserOutputOld> RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInputOld inputOld)
        {
            return _tenantCatalog.RefreshAppLicenseInUseByUser(inputOld);
        }

        public Task<TenantLicensedAppsOutput> GetTenantLicensedApps(Guid tenantId)
        {
            return _tenantCatalog.GetTenantLicensedApps(tenantId);
        }

        public Task<TenantLicenseStatusOutput> GetTenantLicenseStatus(Guid tenantId)
        {
            return _tenantCatalog.GetTenantLicenseStatus(tenantId);
        }

        public Task<IsTenantCnpjLicensedOutput> IsTenantCnpjLicensed(Guid tenantId, string cnpj)
        {
            return _tenantCatalog.IsTenantCnpjLicensed(tenantId, cnpj);
        }

        public Task RefreshAllTenantsLicensing()
        {
            return _tenantCatalog.RefreshAllTenantsLicensing();
        }

        public Task RefreshTenantLicensing(LicensingDetailsUpdated licensingDetailsUpdated)
        {
            return _tenantCatalog.RefreshTenantLicensing(licensingDetailsUpdated);
        }

        public Task<LicenseTenantStatusCurrentOld> GetTenantCurrentLicenseStatus(Guid tenantId)
        {
            return _tenantCatalog.GetTenantCurrentLicenseStatus(tenantId);
        }

        public Task<ConsumeLicenseOutputOld> ConsumeLicense(ConsumeLicenseInput input)
        {
            return _tenantCatalog.ConsumeLicense(input);
        }

        public Task<ReleaseLicenseOutputOld> ReleaseLicense(ReleaseLicenseInput input)
        {
            return _tenantCatalog.ReleaseLicense(input);
        }
    }
}