using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Viasoft.Licensing.LicenseServer.Domain.Catalogs;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Classes.Snapshot;
using Viasoft.Licensing.LicenseServer.Domain.Controllers.DTOs;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Repositories;
using Viasoft.Licensing.LicenseServer.Domain.Services.TenantDatabaseMapping;
using Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration;
using Viasoft.Licensing.LicenseServer.Shared.Consts;

namespace Viasoft.Licensing.LicenseServer.Host.Controllers
{
    [ApiController]
    public class LicenseServerLegacyController: ControllerBase
    {
        private readonly ITenantCatalog _tenantCatalog;
        private readonly ILogger<LicenseServerLegacyController> _logger;
        private readonly ILicenseServerRepository _licenseServerRepository;
        private readonly ITenantDatabaseMappingProvider _tenantDatabaseMappingProvider;

        public LicenseServerLegacyController(ITenantCatalog tenantCatalog, ILogger<LicenseServerLegacyController> logger, 
            ITenantDatabaseMappingProvider tenantDatabaseMappingProvider, ILicenseServerRepository licenseServerRepository)
        {
            _tenantCatalog = tenantCatalog;
            _logger = logger;
            _tenantDatabaseMappingProvider = tenantDatabaseMappingProvider;
            _licenseServerRepository = licenseServerRepository;
        }

        [HttpGet("/licensing/license-server/licenses/{tenantId:guid}/legacy/usage")]
        public async Task<IActionResult> GetLicenseUsage([FromRoute] Guid tenantId)
        {
            if (!await IsTenantConfiguredInLegacyMode(tenantId))
            {
                _logger.LogWarning("There is no tenant mapped with {TenantId} id", tenantId);
                return BadRequest($"There is no tenant mapped with id {tenantId}");
            }
            
            var licenseStatus = await _tenantCatalog.GetTenantCurrentLicenseStatus(tenantId);

            if (licenseStatus == null)
            {
                _logger.LogWarning("Tried to get license usage for tenant {TenantId}, we had a problem to load them", tenantId);
                return BadRequest($"Tried to get license usage for tenant {tenantId}, we had a problem to load them");
            }

            return Ok(EnumerableGetLicenseUsage(licenseStatus));
        }

        private static IEnumerable<dynamic> EnumerableGetLicenseUsage(LicenseTenantStatusCurrent licenseStatus)
        {
            var licenseUsageInRealTimeDetails = licenseStatus.GetLicenseUsageInRealTime();

            foreach (var detail in licenseUsageInRealTimeDetails)
            {
                yield return new
                {
                    detail.User,
                    detail.Token,
                    detail.StartTime,
                    detail.LastHeartbeatDateTime,
                    detail.AppIdentifier,
                    detail.AppName,
                    detail.BundleIdentifier,
                    detail.BundleName,
                    detail.AppLicensesConsumed,
                    detail.AppLicenses
                };
            }
        }
        
        [HttpGet("/licensing/license-server/licenses/{tenantId:guid}/legacy/snapshot")]
        public async Task<IActionResult> GetLicenseSnapshot([FromRoute] Guid tenantId)
        {
            if (!await IsTenantConfiguredInLegacyMode(tenantId))
            {
                _logger.LogWarning("There is no tenant mapped with {TenantId} id", tenantId);
                return BadRequest($"There is no tenant mapped with id {tenantId}");
            }
            
            var licenseStatus = await _tenantCatalog.GetTenantCurrentLicenseStatus(tenantId);

            if (licenseStatus == null)
            {
                _logger.LogWarning("Tried to get license snapshot for tenant {TenantId}, we had a problem to load them", tenantId);
                return BadRequest($"Tried to get license snapshot for tenant {tenantId}, we had a problem to load them");
            }

            return Ok(new LicenseTenantStatusCurrentSnapshot(licenseStatus));
        }

        [HttpGet("/licensing/license-server/licenses/{tenantId:guid}/legacy")]
        public async Task<ActionResult<GetLicensedTenantLegacyControllerOutput>> GetLicensedTenant([FromRoute] Guid tenantId)
        {
            if (!await IsTenantConfiguredInLegacyMode(tenantId))
            {
                _logger.LogWarning("There is no tenant mapped with {TenantId} id", tenantId);
                return BadRequest($"There is no tenant mapped with id {tenantId}");
            }

            var licenseStatus = await _tenantCatalog.GetTenantCurrentLicenseStatus(tenantId);

            if (licenseStatus == null)
            {
                _logger.LogWarning("Could not find/load licenses for tenant {TenantId}", tenantId);
                return NotFound();
            }

            var appLicensesUsageCount = new Dictionary<string, int>();

            foreach (var pair in licenseStatus.Bundles)
            {
                foreach (var innerPair in pair.Value.OwnedApps)
                {
                    appLicensesUsageCount.Add(innerPair.Key, innerPair.Value.AppLicensesConsumed);
                }
            }

            foreach (var pair in licenseStatus.LooseApps)
            {
                appLicensesUsageCount.Add(pair.Key, pair.Value.AppLicensesConsumed);
            }

            var lastRefresh = await _licenseServerRepository.GetLastRefreshInfo(tenantId);

            var databases = await _tenantDatabaseMappingProvider.GetTenantDatabases(tenantId);

            var output = new GetLicensedTenantLegacyControllerOutput
            {
                TenantDetails = licenseStatus.TenantDetails,
                Refresh = new LicensedTenantRefreshOutput
                {
                    RefreshSucceed = lastRefresh?.RefreshSucceed,
                    LastRefreshDateTime = lastRefresh?.LastRefreshDateTime
                },
                Databases = databases,
                AppLicensesUsageCount = appLicensesUsageCount
            };

            return Ok(output);
        }


        [HttpPost("/licensing/license-server/licenses/{tenantId:guid}/legacy/consume")]
        public async Task<ActionResult<ConsumeLicenseOutput>> ConsumeLicense([FromBody] ConsumeLicenseLegacyControllerInput input, [FromRoute] Guid tenantId)
        {
            if (!await IsTenantConfiguredInLegacyMode(tenantId))
            {
                _logger.LogWarning("There is no tenant mapped with id {TenantId}", tenantId);
                return BadRequest($"There is no tenant mapped with id {tenantId}");
            }
            
            var methodInput = new ConsumeLicenseInput
            {
                Token = input.Sid,
                Cnpj = input.Cnpj,
                User = input.User,
                AppIdentifier = input.AppIdentifier,
                TenantId = tenantId,
                CustomAppName = input.CustomAppName,
                LicenseUsageAdditionalInformation = input.LicenseUsageAdditionalInformation,
                IsTerminalServer = input.IsTerminalServer
            };
            var result = await _tenantCatalog.ConsumeLicense(methodInput);

            if (result.ConsumeAppLicenseStatus is ConsumeAppLicenseStatus.LicenseConsumed or ConsumeAppLicenseStatus.LicenseAlreadyInUseByUser)
            {
                return Ok(result);
            }
            
            _logger.LogWarning("Consume not ok for user {User}, tenant {TenantId}, response {Response}", input.User, tenantId, JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

            return BadRequest(result);
        }
        
        [HttpPost("/licensing/license-server/licenses/{tenantId:guid}/legacy/release")]
        public async Task<ActionResult<ReleaseLicenseOutput>> ReleaseLicense([FromBody] ReleaseLicenseLegacyControllerInput input,[FromRoute] Guid tenantId)
        {
            if (!await IsTenantConfiguredInLegacyMode(tenantId))
            {
                _logger.LogWarning("There is no tenant mapped with {TenantId} id", tenantId);
                return BadRequest($"There is no tenant mapped with id {tenantId}");
            }
            
            var methodInput = new ReleaseLicenseInput
            {
                Cnpj = input.Cnpj,
                Token = input.Sid,
                User = input.User,
                AppIdentifier = input.AppIdentifier,
                TenantId = tenantId,
                IsTerminalServer = input.IsTerminalServer
            };
            var result = await _tenantCatalog.ReleaseLicense(methodInput);

            if (result.ReleaseAppLicenseStatus is ReleaseAppLicenseStatus.LicenseReleased or ReleaseAppLicenseStatus.LicenseStillInUseByUser)
            {
                return Ok(result);
            }
            
            _logger.LogWarning("Release not ok for user {User}, tenant {TenantId}, response {Response}", input.User, tenantId, JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

            return BadRequest(result);
        }
        
        [HttpPost("/licensing/license-server/licenses/{tenantId:guid}/legacy/refresh/{user}")]
        public async Task<ActionResult<RefreshAppLicenseInUseByUserOutput>> RefreshAppLicenseInUseByUser([FromBody] RefreshAppLicenseInUseByUserLegacyControllerInput input, 
            [FromRoute] Guid tenantId, [FromRoute] string user)
        {
            if (!await IsTenantConfiguredInLegacyMode(tenantId))
            {
                _logger.LogWarning("There is no tenant mapped with {TenantId} id", tenantId);
                return BadRequest($"There is no tenant mapped with id {tenantId}");
            }
            
            var methodInput = new RefreshAppLicenseInUseByUserInput
            {
                Cnpj = input.Cnpj,
                Token = input.Sid,
                User = user,
                AppIdentifier = input.AppIdentifier,
                TenantId = tenantId,
                CustomAppName = input.CustomAppName,
                LicenseUsageAdditionalInformation = input.LicenseUsageAdditionalInformation,
                IsTerminalServer = input.IsTerminalServer
            };
            var result = await _tenantCatalog.RefreshAppLicenseInUseByUser(methodInput);

            if (result.Status is RefreshAppLicenseInUseByUserStatus.RefreshSuccessful or RefreshAppLicenseInUseByUserStatus.RefreshSuccessfulLicenseConsumed)
            {
                return Ok(result);
            }
            
            _logger.LogWarning("Refresh not ok for user {User}, tenant {TenantId}, response {Response}", user, tenantId, JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

            return BadRequest(result);
        }

        [HttpPost("/licensing/license-server/licenses/refresh/legacy")]
        public async Task<ActionResult> RefreshAllTenantsLicensing()
        {
            try
            {
                await _tenantCatalog.RefreshAllTenantsLicensing();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "RefreshAllTenantsLicensing");
                return BadRequest();
            }

            return Ok();
        }
        
        //só está por compatibilidade, o Korp já utiliza a rota [HttpGet("/licensing/license-server/licenses/{tenantId:guid}/legacy")]
        [HttpGet("/licensing/license-server/licenses/legacy")]
        public async Task<ActionResult<List<GetLicensedTenantLegacyControllerOutput>>> GetLicensedTenants()
        {
            var result = await _tenantCatalog.GetAllTenantCurrentLicenseStatus();

            var databases = LicenseServerSettingsExtension.GetTenantLegacyDatabases();

            var output = await Task.WhenAll(result.Select(async r =>
            {
                var refresh = await _licenseServerRepository.GetLastRefreshInfo(r.TenantDetails.Identifier);
                var databasesForThisTenant = databases.FirstOrDefault(d => d.TenantId == r.TenantDetails.Identifier)
                    ?.LicensedDatabases ?? new List<string>();
                
                return new GetLicensedTenantLegacyControllerOutput
                {
                    TenantDetails = r.TenantDetails,
                    Refresh = new LicensedTenantRefreshOutput
                    {
                        RefreshSucceed = refresh.RefreshSucceed,
                        LastRefreshDateTime = refresh.LastRefreshDateTime
                    },
                    Databases = databasesForThisTenant
                };
            }));

            return Ok(output);
        }
        
        private async Task<bool> IsTenantConfiguredInLegacyMode(Guid tenantId)
        {
            return DefaultConfigurationConsts.IsRunningAsLegacyWithBroker || await _tenantDatabaseMappingProvider.IsTenantMapped(tenantId);
        }
    }
}