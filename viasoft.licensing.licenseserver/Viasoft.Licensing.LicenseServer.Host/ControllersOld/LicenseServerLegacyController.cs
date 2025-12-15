using System;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Enums;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseCache;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicensedTenantOrchestrator;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.TenantLicensing;
using Viasoft.Licensing.LicenseServer.Shared.Classes.Configuration;

namespace Viasoft.Licensing.LicenseServer.Host.ControllersOld
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LicenseServerLegacyController: ControllerBase
    {
        private readonly ILicensedTenantOrchestratorService _orchestratorService;
        private readonly ITenantLegacyDatabaseMapping _tenantLegacyDatabaseMapping;
        private readonly ILicenseCacheService _licenseCacheService;
        private readonly ITenantLicensingService _tenantLicensingService;
        private readonly IMapper _mapper;
        private readonly ILogger<LicenseServerLegacyController> _logger;

        public LicenseServerLegacyController(ILicensedTenantOrchestratorService orchestratorService, ITenantLegacyDatabaseMapping tenantLegacyDatabaseMapping,
            ILicenseCacheService licenseCacheService, ITenantLicensingService tenantLicensingService, IMapper mapper, ILogger<LicenseServerLegacyController> logger)
        {
            _orchestratorService = orchestratorService;
            _tenantLegacyDatabaseMapping = tenantLegacyDatabaseMapping;
            _licenseCacheService = licenseCacheService;
            _tenantLicensingService = tenantLicensingService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<LicenseTenantStatusCurrentOld> GetLicenseStatus([FromQuery] string databaseName)
        {
            var tenantId = GetTenantIdFromLicensedDatabase(databaseName);
            return await _orchestratorService.GetTenantCurrentLicenseStatus(tenantId);
        }
        
        [HttpGet]
        public async Task<TenantLicenseStatusOutput> GetTenantLicenseStatus([FromQuery] string databaseName)
        {
            var tenantId = GetTenantIdFromLicensedDatabase(databaseName);
            return await _orchestratorService.GetTenantLicenseStatus(tenantId);
        }
        
        [HttpGet]
        public async Task<IsTenantCnpjLicensedOutput> IsTenantCnpjLicensed([FromQuery] string databaseName, [FromQuery] string cnpj)
        {
            var tenantId = GetTenantIdFromLicensedDatabase(databaseName);
            return await _orchestratorService.IsTenantCnpjLicensed(tenantId, cnpj);
        }
        
        [HttpGet]
        public async Task<TenantLicensedAppsOutput> GetTenantLicensedApps([FromQuery] string databaseName)
        {
            var tenantId = GetTenantIdFromLicensedDatabase(databaseName);
            return await _orchestratorService.GetTenantLicensedApps(tenantId);
        }

        [HttpGet]
        public async Task<TenantLicenseStatusRefreshInfo> GetLastConnectionRefreshInfo([FromQuery] string databaseName)
        {
            var tenantId = GetTenantIdFromLicensedDatabase(databaseName);
            return await _tenantLicensingService.GetLastRefreshInfo(tenantId);
        }
        
        [HttpGet]
        public async Task<TenantLicenseDetailsOutput> GetLicensedTenantDetails([FromQuery] string databaseName)
        {
            var tenantId = GetTenantIdFromLicensedDatabase(databaseName);
            var licenseByTenantId = await _licenseCacheService.GetLicenseByTenantId(tenantId);
            var output = _mapper.Map<TenantLicenseDetailsOutput>(licenseByTenantId);
            return output;
        }
        
        [HttpPost]
        public async Task<ConsumeLicenseOutputOld> ConsumeLicense(ConsumeLicenseLegacyInput input)
        {
            var methodInput = CastToDefaultInputAndSetTenantIdFromDatabaseName<ConsumeLicenseInput>(input);
            var consumeOutput = await _orchestratorService.ConsumeLicense(methodInput);
            if (consumeOutput.ConsumeAppLicenseStatus is not ConsumeAppLicenseStatusOld.LicenseConsumed)
            {
                _logger.LogWarning("Consume not ok for user {User}, tenant {TenantId}, response {Response}",  input.User, methodInput.TenantId, JsonSerializer.Serialize(consumeOutput, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            }

            return consumeOutput;
        }
        
        [HttpPost]
        public async Task<ReleaseLicenseOutputOld> ReleaseLicense(ReleaseLicenseLegacyInput input)
        {
            var methodInput = CastToDefaultInputAndSetTenantIdFromDatabaseName<ReleaseLicenseInput>(input);
            var releaseOutput = await _orchestratorService.ReleaseLicense(methodInput);
            if (releaseOutput.ReleaseAppLicenseStatus is not ReleaseAppLicenseStatusOld.LicenseReleased)
            {
                _logger.LogWarning("Release not ok for user {User}, tenant {TenantId}, response {Response}", input.User, methodInput.TenantId, JsonSerializer.Serialize(releaseOutput, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            }

            return releaseOutput;
        }
        
        [HttpPost]
        public async Task<RefreshAppLicenseInUseByUserOutputOld> RefreshAppLicenseInUseByUser(RefreshLegacyAppLicenseInUseByUserInput input)
        {
            var methodInput = CastToDefaultInputAndSetTenantIdFromDatabaseName<RefreshAppLicenseInUseByUserInputOld>(input);
            var refreshOutput = await _orchestratorService.RefreshAppLicenseInUseByUser(methodInput);
            if (refreshOutput.Status is not (RefreshAppLicenseInUseByUserStatusOld.RefreshSuccessful or RefreshAppLicenseInUseByUserStatusOld.RefreshSuccessfulLicenseConsumed))
            {
                _logger.LogWarning("Refresh not ok for user {User}, tenant {TenantId}, response {Response}", input.User, methodInput.TenantId, JsonSerializer.Serialize(refreshOutput, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            }

            return refreshOutput;
        }

        //no legacy isso daqui precisa ficar
        [HttpPost]
        public async Task<bool> RefreshAllTenantsLicensing()
        {
            try
            {
                await _orchestratorService.RefreshAllTenantsLicensing();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "RefreshAllTenantsLicensing");
                return false;
            }

            return true;
        }

        private TDefaultInput CastToDefaultInputAndSetTenantIdFromDatabaseName<TDefaultInput>(object legacyInput) where TDefaultInput: class
        {
            var defaultInput = _mapper.Map<TDefaultInput>(legacyInput);
            var databaseName = (string) legacyInput.GetType().GetProperty(nameof(ConsumeLicenseLegacyInput.DatabaseName))?.GetValue(legacyInput);
            var tenantId = GetTenantIdFromLicensedDatabase(databaseName);
            defaultInput.GetType().GetProperty(nameof(ConsumeLicenseInput.TenantId))?.SetValue(defaultInput, tenantId);
            return defaultInput;
        }

        private Guid GetTenantIdFromLicensedDatabase(string databaseName)
        {
            return _tenantLegacyDatabaseMapping.GetTenantIdFromLegacyLicensedDatabase(databaseName);
        }
    }
}