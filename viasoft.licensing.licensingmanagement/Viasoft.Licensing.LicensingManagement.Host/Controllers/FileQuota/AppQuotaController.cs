using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Extensions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.LicensingManagement.Domain.Authorizations;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.FileQuota;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.Services.FileQuota;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers.FileQuota
{
    public class AppQuotaController: BaseController
    {
        private readonly IFileQuotaCallerService _fileQuotaCallerService;
        private readonly ILicenseRepository _licenseRepository;
        private readonly IFileQuotaViewService _fileQuotaViewService;
        private readonly IRepository<FileAppQuotaView> _appQuotaViewRepository;
        private readonly IRepository<LicensedApp> _licensedApp;
        private readonly IRepository<App> _apps;
        
        public AppQuotaController(IFileQuotaCallerService fileQuotaCallerService, ILicenseRepository licenseRepository,
            IFileQuotaViewService fileQuotaViewService, IRepository<FileAppQuotaView> appQuotaViewRepository, IRepository<LicensedApp> licensedApp, IRepository<App> apps)
        {
            _fileQuotaCallerService = fileQuotaCallerService;
            _licenseRepository = licenseRepository;
            _fileQuotaViewService = fileQuotaViewService;
            _appQuotaViewRepository = appQuotaViewRepository;
            _licensedApp = licensedApp;
            _apps = apps;
        }

        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public async Task Insert(FileAppQuotaInput input)
        {
            var appExists = await _fileQuotaViewService.DoesAppQuotaExists(input.LicensedTenantId, input.AppId);
            if(appExists)
                return;
            
            var licensedApp = await _licenseRepository.GetQuotaAppDetailsByAppIdentifier(input.AppId, input.LicensedTenantId);
            if (licensedApp == null)
                throw new ArgumentException(nameof(licensedApp));

            var result = await _fileQuotaCallerService.AddOrUpdateFileAppQuota(licensedApp.LicencedTenantIdentifier, licensedApp.Identifier, input.QuotaLimit);
            if (result == null)
                throw new HttpRequestException("File Provider App Quota Call failed");
            await _fileQuotaViewService.AddAppQuotaView(input.LicensedTenantId, input.AppId, licensedApp.Name, result.QuotaLimit);
        }

        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public async Task Update(FileAppQuotaInput input)
        {
            var licensedApp = await _licenseRepository.GetQuotaAppDetailsByAppIdentifier(input.AppId, input.LicensedTenantId);
            if (licensedApp == null)
                throw new ArgumentException(nameof(licensedApp));

            var result = await _fileQuotaCallerService.AddOrUpdateFileAppQuota(licensedApp.LicencedTenantIdentifier, licensedApp.Identifier, input.QuotaLimit);
            if (result == null)
                throw new HttpRequestException("File Provider App Quota Call failed");
            await _fileQuotaViewService.UpdateAppQuotaView(input.LicensedTenantId, input.AppId, result.QuotaLimit);
        }

        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public async Task AddOrUpdateAppQuota(FileAppQuotaInput input)
        {
            var licensedApp = await _licenseRepository.GetQuotaAppDetailsByAppIdentifier(input.AppId, input.LicensedTenantId);
            if (licensedApp == null)
                throw new ArgumentException(nameof(licensedApp));

            var result = await _fileQuotaCallerService.AddOrUpdateFileAppQuota(licensedApp.LicencedTenantIdentifier, licensedApp.Identifier, input.QuotaLimit);
            if (result == null)
                throw new HttpRequestException("File Provider App Quota Call failed");
            await _fileQuotaViewService.AddOrUpdateAppQuotaView(input.LicensedTenantId, input.AppId, licensedApp.Name, result.QuotaLimit);
        }

        [HttpGet]
        public async Task<PagedResultDto<FileAppQuotaView>> GetAll([FromQuery] GetAllAppQuotaInput input)
        {
            var query = _appQuotaViewRepository
                .Where(aq => input.LicensedTenantId == aq.LicensedTenantId)
                .PageBy(input.SkipCount, input.MaxResultCount)
                .OrderBy(x => x.AppName)
                .ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting);
            var totalCount = await query.CountAsync();
            var appQuotaResult = await query.ToListAsync();

            return new PagedResultDto<FileAppQuotaView>
            {
                Items = appQuotaResult,
                TotalCount = totalCount
            };
        }

        [HttpGet]
        [Authorize(Policy.UpdateLicense)]
        public async Task<PagedResultDto<BundledAndLooseAppOutput>> GetLicensedAppsForQuotaConfiguration([FromQuery] GetAllAppQuotaInput input)
        {
            var appsIdsToIgnore = await _appQuotaViewRepository
                .Where(aq => input.LicensedTenantId == aq.LicensedTenantId)
                .Select(aq => aq.AppId)
                .ToListAsync();
            var totalCount = await _licensedApp.CountAsync(la => 
                la.LicensedTenantId == input.LicensedTenantId
                && !appsIdsToIgnore.Contains(la.AppId));
            var licensedApps = await _licensedApp
                .Where(la => la.LicensedTenantId == input.LicensedTenantId)
                .WhereIf(appsIdsToIgnore.Any(), la => !appsIdsToIgnore.Contains(la.AppId))
                .PageBy(input.SkipCount, input.MaxResultCount)
                .Select(la => new
                {
                    la.NumberOfLicenses,
                    la.Status,
                    la.AdditionalNumberOfLicenses,
                    la.AppId
                })
                .ToDictionaryAsync(arg => arg.AppId, arg => arg);

            var licensedAppIds = licensedApps.Keys.ToList();

            var apps = await _apps.Select(m => new BundledAndLooseAppOutput
                {
                    Id = m.Id,
                    Name = m.Name,
                    SoftwareId = m.SoftwareId
                })
                .WhereIf(!string.IsNullOrEmpty(input.Filter),
                    la => la.Name.Contains(input.Filter))
                .Where(a => licensedAppIds.Contains(a.Id)).ToListAsync();
            
            foreach (var app in apps)
            {
                if (licensedApps.TryGetValue(app.Id, out var licensedApp))
                {
                    app.NumberOfLicenses = licensedApp.NumberOfLicenses;
                    app.Status = licensedApp.Status;
                    app.AdditionalNumberOfLicenses = licensedApp.AdditionalNumberOfLicenses;
                }
            }
            
            return new PagedResultDto<BundledAndLooseAppOutput>
            {
                Items = apps,
                TotalCount = totalCount
            };
        }

        [HttpGet]
        public async Task<FileAppQuotaView> GetAppQuota(Guid licensedTenantId, Guid appId)
        {
            return await _fileQuotaViewService.GetAppQuota(licensedTenantId, appId);
        }

        [HttpDelete]
        public async Task DeleteAppQuota(Guid licensedTenantId, Guid appId)
        {
            var appQuota = await _fileQuotaViewService.GetAppQuota(licensedTenantId, appId);
            if (appQuota == null) return;
            
            var licensedApp = await _licenseRepository.GetQuotaAppDetailsByAppIdentifier(appQuota.AppId, appQuota.LicensedTenantId);
            if (licensedApp == null)
                throw new ArgumentException(nameof(licensedApp));

            await _fileQuotaCallerService.DeleteAppQuota(licensedApp.LicencedTenantIdentifier, licensedApp.Identifier);

            await _fileQuotaViewService.DeleteAppQuota(licensedTenantId, appId);
        }
    }
}