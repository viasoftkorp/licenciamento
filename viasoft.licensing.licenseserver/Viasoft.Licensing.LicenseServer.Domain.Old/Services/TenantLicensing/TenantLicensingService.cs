using System;
using System.Threading.Tasks;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseCache;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseServer;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.HardwareId;
using Viasoft.Licensing.LicenseServer.Shared.Initializer;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.TenantLicensing
{
    public class TenantLicensingService: ITenantLicensingService, ISingletonDependency
    {
        private readonly ILicenseServerService _licenseServerService;
        private readonly ILicenseCacheService _licenseCacheService;
        private readonly IProvideHardwareIdService _provideHardwareIdService;

        public TenantLicensingService(ILicenseServerService licenseServerService, ILicenseCacheService licenseCacheService, IProvideHardwareIdService provideHardwareIdService)
        {
            _licenseServerService = licenseServerService;
            _licenseCacheService = licenseCacheService;
            _provideHardwareIdService = provideHardwareIdService;
        }

        public async Task<LicenseByTenantIdOld> GetTenantLicensing(Guid tenantId)
        {
            LicenseByTenantIdOld licenseByTenantIdOld;
            try
            {
                licenseByTenantIdOld = await _licenseServerService.GetLicenseByTenantId(tenantId);
                if (licenseByTenantIdOld != null && DefaultConfigurationConsts.IsRunningAsLegacy && string.IsNullOrEmpty(licenseByTenantIdOld.HardwareId))
                {
                    var updateHardwareIdInput = new UpdateHardwareIdInput
                    {
                        TenantId = tenantId,
                        HardwareId = _provideHardwareIdService.ProvideHardwareId(false)
                    };
                    var updateHardwareIdOutput = await _licenseServerService.UpdateHardwareId(updateHardwareIdInput);

                    if (!updateHardwareIdOutput.IsSuccess)
                    {
                        throw new Exception($"Couldn't update hardwareId due to error: {updateHardwareIdOutput.Code.ToString()}");
                    }

                    licenseByTenantIdOld.HardwareId = updateHardwareIdInput.HardwareId;
                }

                if (licenseByTenantIdOld != null)
                {
                    await _licenseCacheService.StoreLicenseByTenantId(licenseByTenantIdOld);
                    await StoreLastLicenseServerRefreshInfo(tenantId, true);
                }

                return licenseByTenantIdOld;
            }
            catch (Exception e)
            {
                if (!e.Message.Equals(ExceptionMessageConsts.CouldNotLoadLicensesFromRemoteServer))
                    throw;
                licenseByTenantIdOld = await _licenseCacheService.GetLicenseByTenantId(tenantId);
                await StoreLastLicenseServerRefreshInfo(tenantId, false);
                return licenseByTenantIdOld;
            }
        }
        
        public Task<TenantLicenseStatusRefreshInfo> GetLastRefreshInfo(Guid tenantId)
        {
            using var db = LiteDbInitializer.OldNewReadonlyRepository(tenantId);
            
            var tenantLicenseStatusLastConnectionWithServer = db.FirstOrDefault<TenantLicenseStatusRefreshInfo>(l => l.TenantId == tenantId);
            
            return Task.FromResult(tenantLicenseStatusLastConnectionWithServer);
        }

        private Task StoreLastLicenseServerRefreshInfo(Guid tenantId, bool success)
        {
            var currentConnectionStatus = new TenantLicenseStatusRefreshInfo
            {
                TenantId = tenantId,
                LastRefreshDateTime = DateTime.UtcNow,
                RefreshSucceed = success
            };
            using (var db = LiteDbInitializer.NewDatabase(LiteDbConsts.BuildTenantConnectionStringOld(tenantId)))
            {
                var tenantLicenseStatusLastConnectionWithServerCollection = db.GetCollection<TenantLicenseStatusRefreshInfo>(nameof(TenantLicenseStatusRefreshInfo));
                tenantLicenseStatusLastConnectionWithServerCollection.Upsert(currentConnectionStatus.TenantId, currentConnectionStatus);
            }
            return Task.CompletedTask;
        }

    }
}