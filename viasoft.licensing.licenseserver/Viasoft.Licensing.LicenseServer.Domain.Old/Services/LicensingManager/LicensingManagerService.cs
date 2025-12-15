using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Viasoft.Licensing.LicenseServer.Domain.Old.Extensions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Shared.Enums;
using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicensingManager
{
    public class LicensingManagerService: ILicensingManagerService
    {
        private readonly LicenseTenantStatusCurrentOld _licenseTenantStatusCurrentOld;
        private readonly ILicenseUsageService _licenseUsageService;
        private LicenseTenantStatusCurrentOld _currentOldState;
        private DateTime LastUpdate { get; }
        private readonly IConfiguration _configuration;

        private readonly ReleaseAppLicenseStatusOld[] _validReleaseLicenseStatusToBackupMemento = {
            ReleaseAppLicenseStatusOld.LicenseReleased, ReleaseAppLicenseStatusOld.AdditionalLicenseReleased, 
            ReleaseAppLicenseStatusOld.LicenseStillInUseByUser, ReleaseAppLicenseStatusOld.AdditionalLicenseStillInUseByUser
        };
        private readonly ConsumeAppLicenseStatusOld[] _validConsumeLicenseStatusToBackupMemento = {
            ConsumeAppLicenseStatusOld.LicenseConsumed, ConsumeAppLicenseStatusOld.AdditionalLicenseConsumed, 
            ConsumeAppLicenseStatusOld.LicenseAlreadyInUseByUser, ConsumeAppLicenseStatusOld.AdditionalLicenseAlreadyInUseByUser
        };
        
        public LicensingManagerService(LicenseByTenantIdOld tenantDetails, ILicenseUsageService licenseUsageService, IConfiguration configuration, IProvideHardwareIdService provideHardwareIdService)
        {
            _licenseUsageService = licenseUsageService;
            _configuration = configuration;
            _licenseTenantStatusCurrentOld = new LicenseTenantStatusCurrentOld(tenantDetails, provideHardwareIdService);
            LastUpdate = DateTime.UtcNow;
            UpdateCurrentState();
        }

        public ConsumeLicenseOutputOld ConsumeLicense(ConsumeLicenseInput input)
        {
            EvaluateAndReleaseLicensesBasedOnHeartbeat();
            var consumeLicenseOutput = _licenseTenantStatusCurrentOld.ConsumeLicense(input);
            UpdateCurrentState(consumeLicenseOutput);
            return consumeLicenseOutput;
        }

        public void EvaluateAndReleaseLicensesBasedOnHeartbeat()
        {
            var minimumAllowedHeartbeatInSeconds = _configuration[EnvironmentVariableConsts.MinimumAllowedHeartbeatInSeconds];
            
            var minimumAllowedHeartbeat = string.IsNullOrEmpty(minimumAllowedHeartbeatInSeconds)
                ? DateTime.UtcNow.AddSeconds(-DefaultConfigurationConsts.MinimumAllowedHeartbeatInSeconds)
                : DateTime.UtcNow.AddSeconds(-Convert.ToDouble(minimumAllowedHeartbeatInSeconds));
            
            var licensesToRelease = _licenseTenantStatusCurrentOld.GetLicensesToReleaseBasedOnHeartbeat(minimumAllowedHeartbeat);
            foreach (var licenseToRelease in licensesToRelease)
                ReleaseLicense(licenseToRelease);
        }

        public ReleaseLicenseOutputOld ReleaseLicense(ReleaseLicenseInput input)
        {
            var releaseLicenseOutput = _licenseTenantStatusCurrentOld.ReleaseLicense(input);
            UpdateCurrentState(releaseLicenseOutput);
            SaveReleaseLog(input, releaseLicenseOutput);
            return releaseLicenseOutput;
        }

        private void SaveReleaseLog(ReleaseLicenseInput releaseLicenseInput, ReleaseLicenseOutputOld releaseLicenseOutputOld)
        {
            var validStatusToLog = new [] { ReleaseAppLicenseStatusOld.LicenseReleased, ReleaseAppLicenseStatusOld.AdditionalLicenseReleased };

            if (releaseLicenseOutputOld.ReleaseAppLicenseStatus.In(validStatusToLog) &&
                releaseLicenseOutputOld.LicenseUsageEndTime.HasValue && releaseLicenseOutputOld.LicenseUsageStartTime.HasValue)
            {
                var storeDoneUsageLog = new StoreDoneUsageLog
                (
                    releaseLicenseInput.TenantId,
                    releaseLicenseInput.User,
                    releaseLicenseOutputOld.Cnpj,
                    releaseLicenseOutputOld.AppIdentifier,
                    releaseLicenseOutputOld.AppName,
                    releaseLicenseOutputOld.BundleIdentifier,
                    releaseLicenseOutputOld.BundleName,
                    releaseLicenseOutputOld.LicenseUsageStartTime.Value,
                    releaseLicenseOutputOld.LicenseUsageEndTime.Value,
                    releaseLicenseOutputOld.ReleaseAppLicenseStatus == ReleaseAppLicenseStatusOld.AdditionalLicenseReleased,
                    releaseLicenseOutputOld.SoftwareIdentifier,
                    releaseLicenseOutputOld.SoftwareName,
                    releaseLicenseOutputOld.LicenseUsageAdditionalInformation
                );
                _licenseUsageService.StoreDoneUsageLog(storeDoneUsageLog);
            }
        }

        public LicenseTenantStatusCurrentOld GetCurrentState()
        {
            return _currentOldState;
        }

        private void UpdateCurrentState(ReleaseLicenseOutputOld releaseLicenseOutputOld)
        {
            if (releaseLicenseOutputOld.ReleaseAppLicenseStatus.In(_validReleaseLicenseStatusToBackupMemento)) 
                UpdateCurrentState();
        }
        
        private void UpdateCurrentState(ConsumeLicenseOutputOld consumeLicenseOutputOld)
        {
            if (consumeLicenseOutputOld.ConsumeAppLicenseStatus.In(_validConsumeLicenseStatusToBackupMemento))
                UpdateCurrentState();
        }
        
        private void UpdateCurrentState()
        {
            var newCurrentState = _licenseTenantStatusCurrentOld.Clone();
            Interlocked.Exchange(ref _currentOldState, newCurrentState);
        }
        
        public RefreshAppLicenseInUseByUserOutputOld RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInputOld inputOld)
        {
            if (_licenseTenantStatusCurrentOld.RefreshAppLicenseInUseByUser(inputOld))
            {
                UpdateCurrentState();
                return new RefreshAppLicenseInUseByUserOutputOld { Status = RefreshAppLicenseInUseByUserStatusOld.RefreshSuccessful };
            }

            var consumeLicenseInput = new ConsumeLicenseInput
            {
                TenantId = inputOld.TenantId,
                AppIdentifier = inputOld.AppIdentifier,
                User = inputOld.User,
                Cnpj = inputOld.Cnpj,
                CustomAppName = inputOld.CustomAppName,
                LicenseUsageAdditionalInformation = inputOld.LicenseUsageAdditionalInformation
            };
            
            var consumeLicenseOutput = ConsumeLicense(consumeLicenseInput);
            return new RefreshAppLicenseInUseByUserOutputOld
                {
                    Status = consumeLicenseOutput.ConsumeAppLicenseStatus.In(_validConsumeLicenseStatusToBackupMemento) 
                        ? RefreshAppLicenseInUseByUserStatusOld.RefreshSuccessfulLicenseConsumed
                        : RefreshAppLicenseInUseByUserStatusOld.RefreshFailedLicenseNotAvailable
                };
        }

        public TenantLicensedAppsOutput GetTenantLicensedApps()
        {
            return new TenantLicensedAppsOutput
            {
                Status = TenantLicensedAppsStatus.Successful,
                AppsIdentifiers = _licenseTenantStatusCurrentOld.GetTenantLicensedApps() 
            };
        }

        public TenantLicenseStatus GetTenantLicenseStatus()
        {
            var output = _licenseTenantStatusCurrentOld.TenantDetails.Status.GetTenantLicenseStatusFromLicensingStatus();
            return output;
        }

        public bool IsTenantCnpjLicensed(string cnpj)
        {
            cnpj = Regex.Replace(cnpj, @"\D", "");
            return _licenseTenantStatusCurrentOld.TenantDetails.Cnpjs.Contains(cnpj);
        }

        public DateTime GetLastUpdatedDateTime() => LastUpdate;

        public IEnumerable<AppLicenseConsumer> GetAllLicensesInUse()
        {
            return _licenseTenantStatusCurrentOld.GetAllLicensesInUse();
        }

        public void RestoreLicensesInUse(IEnumerable<AppLicenseConsumer> licensesInUse)
        {
            if (_licenseTenantStatusCurrentOld.RestoreLicensesInUse(licensesInUse))
                UpdateCurrentState();
        }
    }
}