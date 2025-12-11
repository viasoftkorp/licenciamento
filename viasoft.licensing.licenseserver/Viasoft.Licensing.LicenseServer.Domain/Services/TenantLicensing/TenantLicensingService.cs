using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.TenantLicensing;
using Viasoft.Licensing.LicenseServer.Domain.Repositories;
using Viasoft.Licensing.LicenseServer.Domain.Services.LicenseServer;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.HardwareId;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.TenantLicensing
{
    public class TenantLicensingService: ITenantLicensingService, ITransientDependency
    {
        private readonly ILicenseServerRepository _licenseServerRepository;
        private readonly IExternalLicensingManagementService _externalLicensingManagementService;
        private readonly IProvideHardwareIdService _provideHardwareIdService;
        private readonly ILogger<LicenseTenantStatusCurrent> _logger;

        public TenantLicensingService(IExternalLicensingManagementService externalLicensingManagementService, IProvideHardwareIdService provideHardwareIdService, 
            ILogger<LicenseTenantStatusCurrent> logger, ILicenseServerRepository licenseServerRepository)
        {
            _externalLicensingManagementService = externalLicensingManagementService;
            _provideHardwareIdService = provideHardwareIdService;
            _logger = logger;
            _licenseServerRepository = licenseServerRepository;
        }

        public async Task<TenantLicenses> GetTenantLicensing(Guid tenantId)
        {
            try
            {
                var licenseByTenantId = await _externalLicensingManagementService.GetLicenseByTenantId(tenantId);
                
                if (DefaultConfigurationConsts.IsRunningAsLegacy && string.IsNullOrEmpty(licenseByTenantId.LicensedTenant.HardwareId))
                {
                    await SyncHardwareId(tenantId, licenseByTenantId);
                }

                await _licenseServerRepository.StoreLicenseByTenantId(new TenantLicensesCache
                {
                    Id = licenseByTenantId.LicensedTenant.Identifier,
                    TenantLicenses = licenseByTenantId,
                    LogDateTime = DateTime.UtcNow
                });

                return licenseByTenantId;
            }
            catch (Exception e)
            {
                if (!e.Message.Equals(ExceptionMessageConsts.CouldNotLoadLicensesFromRemoteServer))
                    throw;

                await _licenseServerRepository.StoreLastLicenseServerRefreshInfo(TenantLicenseStatusRefreshInfo.RefreshFailed(tenantId));
                return await GetCachedLicense(tenantId);
            }
        }

        private async ValueTask SyncHardwareId(Guid tenantId, TenantLicenses licenseByTenantId)
        {
            var validBoolean = bool.TryParse(licenseByTenantId.LicensedTenantSettings.Value, out var useSimpleHardwareId);
            if (!validBoolean)
            {
                _logger.LogWarning("Não foi possível parsear a configuração de utilização de hardware id simples, e portando utilizaremos como false, o valor é {Value}", licenseByTenantId.LicensedTenantSettings.Value);
                useSimpleHardwareId = false;
            }
                    
            var updateHardwareIdInput = new UpdateHardwareIdInput
            {
                TenantId = tenantId,
                HardwareId = _provideHardwareIdService.ProvideHardwareId(useSimpleHardwareId)
            };
            var updateHardwareIdOutput = await _externalLicensingManagementService.UpdateHardwareId(updateHardwareIdInput);

            if (!updateHardwareIdOutput.IsSuccess)
            {
                throw new Exception($"Couldn't update hardwareId due to error: {updateHardwareIdOutput.Code.ToString()}");
            }

            licenseByTenantId.LicensedTenant.HardwareId = updateHardwareIdInput.HardwareId;
        }

        private async Task<TenantLicenses> GetCachedLicense(Guid tenantId)
        {
            var licenseByTenantId = await _licenseServerRepository.GetLicenseByTenantId(tenantId);
            if (licenseByTenantId == null || licenseByTenantId.LogDateTime < DateTime.UtcNow.AddHours(-72))
                return null;
            
            var license = licenseByTenantId.TenantLicenses;
            // essa propriedade só seria nula caso acontecesse o seguinte:
            // é a primeira vez que está carregando as licenças da nuvem com o esquema de LicensedTenantSettings
            // e houve um erro para carrega-las. como em todos os lugares do código essa propriedade é lida
            // sem verificação de null, colocamos esse valor default
            license.LicensedTenantSettings ??= new LicensedTenantSettingsOutput
            {
                Id = Guid.NewGuid(),
                Key = LicensedTenantSettingsKeys.UseSimpleHardwareId,
                Value = bool.FalseString,
                LicensingIdentifier = tenantId,
                TenantId = tenantId
            };
            return license;
        }
    }
}