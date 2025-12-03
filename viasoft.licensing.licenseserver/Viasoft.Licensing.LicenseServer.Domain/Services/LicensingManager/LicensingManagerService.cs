using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Viasoft.Licensing.LicenseServer.Domain.Classes;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.UserBehaviour;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Extensions;
using Viasoft.Licensing.LicenseServer.Domain.Repositories;
using Viasoft.Licensing.LicenseServer.Domain.Services.LicenseServer;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.LicensingManager
{
    public class LicensingManagerService: ILicensingManagerService
    {
        private readonly LicenseTenantStatusCurrent _licenseTenantStatusCurrent;
        private readonly IConfiguration _configuration;
        private readonly ILicenseServerRepository _licenseServerRepository;
        
        private readonly ConsumeAppLicenseStatus[] _validConsumeLicenseStatusToBackupMemento = {
            ConsumeAppLicenseStatus.LicenseConsumed, ConsumeAppLicenseStatus.LicenseAlreadyInUseByUser
        };
        
        public LicensingManagerService(LicenseTenantStatusCurrent licenseTenantStatusCurrent, IConfiguration configuration, ILicenseServerRepository licenseServerRepository)
        {
            _configuration = configuration;
            _licenseServerRepository = licenseServerRepository;
            _licenseTenantStatusCurrent = licenseTenantStatusCurrent;
        }

        public async Task<ConsumeLicenseOutput> ConsumeLicense(ConsumeLicenseInput input)
        {
            await EvaluateAndReleaseLicensesBasedOnHeartbeat();
            var consumeLicenseOutput = await _licenseTenantStatusCurrent.ConsumeLicense(input);
            return consumeLicenseOutput;
        }

        public async Task EvaluateAndReleaseLicensesBasedOnHeartbeat()
        {
            var minimumAllowedHeartbeatInSeconds = _configuration[EnvironmentVariableConsts.MinimumAllowedHeartbeatInSeconds];
            
            var minimumAllowedHeartbeat = string.IsNullOrEmpty(minimumAllowedHeartbeatInSeconds)
                ? DateTime.UtcNow.AddSeconds(-DefaultConfigurationConsts.MinimumAllowedHeartbeatInSeconds)
                : DateTime.UtcNow.AddSeconds(-Convert.ToDouble(minimumAllowedHeartbeatInSeconds));
            
            var licensesToRelease = _licenseTenantStatusCurrent.GetLicensesToReleaseBasedOnHeartbeat(minimumAllowedHeartbeat);
            foreach (var licenseToRelease in licensesToRelease)
                await ReleaseLicense(licenseToRelease);
        }

        public async Task<ReleaseLicenseOutput> ReleaseLicense(ReleaseLicenseInput input)
        {
            var releaseLicenseOutput = _licenseTenantStatusCurrent.ReleaseLicense(input);
            await SaveReleaseLog(input, releaseLicenseOutput);
            return releaseLicenseOutput;
        }

        private async Task SaveReleaseLog(ReleaseLicenseInput releaseLicenseInput, ReleaseLicenseOutput releaseLicenseOutput)
        {
            if (releaseLicenseOutput.ReleaseAppLicenseStatus == ReleaseAppLicenseStatus.LicenseReleased &&
                releaseLicenseOutput.LicenseUsageEndTime.HasValue && releaseLicenseOutput.LicenseUsageStartTime.HasValue)
            {
                var log = new LicenseUsageBehaviourDetails
                {
                    Cnpj = releaseLicenseOutput.Cnpj,
                    Id = Guid.NewGuid(),
                    Language = releaseLicenseOutput.LicenseUsageAdditionalInformation?.Language,
                    User = releaseLicenseInput.User,
                    AdditionalLicense = false,
                    AppIdentifier = releaseLicenseOutput.AppIdentifier,
                    AppName = releaseLicenseOutput.AppName,
                    BrowserInfo = releaseLicenseOutput.LicenseUsageAdditionalInformation?.BrowserInfo,
                    BundleIdentifier = releaseLicenseOutput.BundleIdentifier,
                    BundleName = releaseLicenseOutput.BundleName,
                    DatabaseName = releaseLicenseOutput.LicenseUsageAdditionalInformation?.DatabaseName,
                    EndTime = releaseLicenseOutput.LicenseUsageEndTime.Value,
                    HostName = releaseLicenseOutput.LicenseUsageAdditionalInformation?.HostName,
                    HostUser = releaseLicenseOutput.LicenseUsageAdditionalInformation?.HostUser,
                    OsInfo = releaseLicenseOutput.LicenseUsageAdditionalInformation?.OsInfo,
                    SoftwareIdentifier = releaseLicenseOutput.SoftwareIdentifier,
                    SoftwareName = releaseLicenseOutput.SoftwareName,
                    SoftwareVersion = releaseLicenseOutput.LicenseUsageAdditionalInformation?.SoftwareVersion,
                    StartTime = releaseLicenseOutput.LicenseUsageStartTime.Value,
                    TenantId = releaseLicenseInput.TenantId,
                    DurationInSeconds = (int) (releaseLicenseOutput.LicenseUsageEndTime.Value - releaseLicenseOutput.LicenseUsageStartTime.Value).TotalSeconds,
                    LocalIpAddress = releaseLicenseOutput.LicenseUsageAdditionalInformation?.LocalIpAddress,
                    LogDateTime = DateTime.UtcNow
                };
                
                await _licenseServerRepository.StoreDoneUsageLog(log);
            }
        }

        public LicenseTenantStatusCurrent GetCurrentState()
        {
            return _licenseTenantStatusCurrent;
        }
        
        public async Task<RefreshAppLicenseInUseByUserOutput> RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInput input)
        {
            if (_licenseTenantStatusCurrent.RefreshAppLicenseInUseByUser(input))
            {
                return new RefreshAppLicenseInUseByUserOutput { Status = RefreshAppLicenseInUseByUserStatus.RefreshSuccessful };
            }

            var consumeLicenseInput = new ConsumeLicenseInput
            {
                TenantId = input.TenantId,
                AppIdentifier = input.AppIdentifier,
                User = input.User,
                Cnpj = input.Cnpj,
                CustomAppName = input.CustomAppName,
                LicenseUsageAdditionalInformation = input.LicenseUsageAdditionalInformation,
                IsTerminalServer = input.IsTerminalServer,
                Token = input.Token
            };
            
            var consumeLicenseOutput = await ConsumeLicense(consumeLicenseInput);
            return new RefreshAppLicenseInUseByUserOutput
                {
                    Status = consumeLicenseOutput.ConsumeAppLicenseStatus.In(_validConsumeLicenseStatusToBackupMemento) 
                        ? RefreshAppLicenseInUseByUserStatus.RefreshSuccessfulLicenseConsumed
                        : RefreshAppLicenseInUseByUserStatus.RefreshFailedLicenseNotAvailable
                };
        }

        public List<KeyValuePair<string, List<AppLicenseConsumer>>> GetAllLicensesInUse()
        {
            return _licenseTenantStatusCurrent.GetAllLicensesInUse();
        }

        public async Task RestoreLicensesInUse(List<KeyValuePair<string, List<AppLicenseConsumer>>> licensesInUse)
        {
            await _licenseTenantStatusCurrent.RestoreLicensesInUse(licensesInUse);
        }
    }
}