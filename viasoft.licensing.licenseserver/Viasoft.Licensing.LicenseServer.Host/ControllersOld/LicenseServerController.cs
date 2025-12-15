using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Licensing.LicenseServer.Domain.Old.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.UserBehaviour;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseCache;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicensedTenantOrchestrator;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.TenantLicensing;

namespace Viasoft.Licensing.LicenseServer.Host.ControllersOld
{
    public class LicenseServerController: BaseController
    {
        private readonly ILicensedTenantOrchestratorService _orchestratorService;
        private readonly ILicenseUsageService _licenseUsageService;
        private readonly ITenantLicensingService _tenantLicensingService;
        private readonly ILicenseCacheService _licenseCacheService;
        private readonly IMapper _mapper;
        private readonly ILogger<LicenseServerController> _logger;

        public LicenseServerController(ILicensedTenantOrchestratorService orchestratorService, ILicenseUsageService licenseUsageService,
            ILicenseCacheService licenseCacheService, ITenantLicensingService tenantLicensingService, IMapper mapper, ILogger<LicenseServerController> logger)
        {
            _orchestratorService = orchestratorService;
            _licenseUsageService = licenseUsageService;
            _licenseCacheService = licenseCacheService;
            _tenantLicensingService = tenantLicensingService;
            _mapper = mapper;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<TenantLicenseStatusOutput> GetTenantLicenseStatus([FromQuery]Guid tenantId)
        {
            return await _orchestratorService.GetTenantLicenseStatus(tenantId);
        }
        
        [HttpGet]
        public async Task<IsTenantCnpjLicensedOutput> IsTenantCnpjLicensed([FromQuery] Guid tenantId, [FromQuery] string cnpj)
        {
            return await _orchestratorService.IsTenantCnpjLicensed(tenantId, cnpj);
        }
        
        [HttpGet]
        public async Task<TenantLicensedAppsOutput> GetTenantLicensedApps([FromQuery] Guid tenantId)
        {
            return await _orchestratorService.GetTenantLicensedApps(tenantId);
        }
        
        [HttpGet]
        public async Task<List<LicenseUsageBehaviourDetails>> GetLicenseUsage(Guid tenantId)
        {
            return await _licenseUsageService.GetLicensesUsage(tenantId);
        }

        [HttpGet]
        public async Task<LicenseTenantStatusCurrentOld> GetLicenseStatus(Guid tenantId)
        {
            return await _orchestratorService.GetTenantCurrentLicenseStatus(tenantId);
        }

        [HttpGet]
        public async Task<TenantLicenseStatusRefreshInfo> GetLastRefreshInfo(Guid tenantId)
        {
            return await _tenantLicensingService.GetLastRefreshInfo(tenantId);
        }
        
        [HttpGet]
        public async Task<TenantLicenseDetailsOutput> GetLicensedTenantDetails(Guid tenantId)
        {
            var licenseByTenantId = await _licenseCacheService.GetLicenseByTenantId(tenantId);
            var output = _mapper.Map<TenantLicenseDetailsOutput>(licenseByTenantId);
            return output;
        }
        
        [HttpPost]
        public async Task<ConsumeLicenseOutputOld> ConsumeLicense(ConsumeLicenseInput input)
        {
            return await _orchestratorService.ConsumeLicense(input);
        }
        
        [HttpPost]
        public async Task<ReleaseLicenseOutputOld> ReleaseLicense(ReleaseLicenseInput input)
        {
            return await _orchestratorService.ReleaseLicense(input);
        }
        
        [HttpPost]
        public async Task<RefreshAppLicenseInUseByUserOutputOld> RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInputOld inputOld)
        {
            return await _orchestratorService.RefreshAppLicenseInUseByUser(inputOld);
        }

        [HttpPost]
        public async Task<bool> RefreshAllTenantsLicensing()
        {
            try
            {
                await _orchestratorService.RefreshAllTenantsLicensing();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not refresh all tenants licensing");
                return false;
            }

            return true;
        }

    }
}