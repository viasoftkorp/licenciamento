using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.BaseCrud;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Extensions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.Authorizations;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.ConnectionError;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSaga;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;
using Viasoft.Licensing.LicensingManagement.Domain.Messages;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class LicensingController: BaseCrudController<LicensedTenant, LicenseTenantCreateOutput, LicenseTenantCreateInput, LicenseTenantUpdateInput, LicenseTenantUpdateOutput, GetAllLicensesInput, LicenseTenantDeleteOutput, OperationValidation, string>
    {
        private readonly IRepository<LicensedTenant> _licensedTenant;
        private readonly IRepository<App> _apps;
        private readonly ILicenseRepository _licenseRepository;
        private readonly ILicensedTenantService _licensedTenantService;
        private readonly IRepository<LicensedApp> _licensedApp;
        private readonly IAccountRepository _accountRepository;
        private readonly ILicensedTenantCacheService _cacheService;
        private readonly IRepository<Software> _softwares;

        public LicensingController(IMapper mapper, IRepository<LicensedTenant> licensedTenant, ILicensedTenantService licensedTenantService, 
            IRepository<LicensedApp> licensedApp, IRepository<App> apps, ILicenseRepository licenseRepository, IAccountRepository accountRepository,
            ILicensedTenantCacheService cacheService, IRepository<Software> softwares) : base(mapper, licensedTenant)
        {
            _licensedTenant = licensedTenant;
            _licensedTenantService = licensedTenantService;
            _licensedApp = licensedApp;
            _apps = apps;
            _licenseRepository = licenseRepository;
            _accountRepository = accountRepository;
            _cacheService = cacheService;
            _softwares = softwares;

            Authorizations.Create = Policy.CreateLicense;
            Authorizations.Update = Policy.UpdateLicense;
            Authorizations.Delete = Policy.DeleteLicense;
        }

        [HttpGet]
        public override async Task<PagedResultDto<LicenseTenantCreateOutput>> GetAll(GetAllLicensesInput input)
        {
            var output = await base.GetAll(input);
            var accountsIds = output.Items.Select(acc => acc.AccountId).Distinct().ToList();
            var accountsNames = _accountRepository.GetAccountsNamesFromIdList(accountsIds);
            foreach (var tenant in output.Items)
            {
                accountsNames.TryGetValue(tenant.AccountId, out var accountName);
                tenant.AccountName = accountName;
            }
            return output;
        }
        
        [HttpGet]
        [TenantIdParameterHint("tenantId", ParameterLocation.Query)]
        [UserNotRequired]
        public async Task<LicenseTenantCreateOutput> GetByTenantId([FromQuery] Guid tenantId)
        {
            var licensedTenant = await Repository
                .Select(t => new LicenseTenantCreateOutput
                {
                     Id = t.Id,
                     AccountId = t.AccountId,
                     Status = t.Status,
                     Identifier = t.Identifier,
                     ExpirationDateTime = t.ExpirationDateTime,
                     LicensedCnpjs = t.LicensedCnpjs,
                     AdministratorEmail = t.AdministratorEmail,
                     LicenseConsumeType = t.LicenseConsumeType,
                     Notes = t.NotesString,
                })
                .FirstOrDefaultAsync(t => t.Identifier == tenantId);
            if (licensedTenant != null)
                licensedTenant.AccountName = _accountRepository.GetAccountNameFromId(licensedTenant.AccountId);
            return licensedTenant;
        }

        [HttpGet]
        public override async Task<LicenseTenantCreateOutput> GetById(Guid id)
        {
            var output = await base.GetById(id);
            output.SagaInfo ??= new LicensedTenantSagaInfo
            {
                AmCreatingNewLicensedTenant = false,
                Status = CurrentSagaStatus.CompletedSuccessfully
            };
            var accountId = output.AccountId;
            var accountName = _accountRepository.GetAccountNameFromId(accountId);
            output.AccountName = accountName;
            return output;
        }

        protected override (Expression<Func<LicensedTenant, string>>, bool) DefaultGetAllSorting()
        {
            return (l => l.AdministratorEmail, true);
        }
        
        [HttpPost]
        [Authorize(Policy.CreateLicense)]
        public override async Task<LicenseTenantCreateOutput> Create(LicenseTenantCreateInput input)
        {
            return await _licensedTenantService.CreateNewTenantLicensing(input, true);
        }
        
        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public override async Task<LicenseTenantUpdateOutput> Update(LicenseTenantUpdateInput input)
        {
            return await _licensedTenantService.UpdateTenantLicensing(input);
        }

        [HttpDelete]
        [Authorize(Policy.DeleteLicense)]
        public override async Task<LicenseTenantDeleteOutput> Delete(Guid id)
        {
            var output = await _licensedTenantService.DeleteTenantLicensing(id);
            return new LicenseTenantDeleteOutput
            {
                Success = output.Success,
                Errors = new List<BaseCrudResponseError<OperationValidation>>
                {
                    new()
                    {
                        ErrorCode = output.OperationValidation
                    }
                }
            };
        }

        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public async Task<LicensedBundleCreateOutput> AddBundleToLicense(LicensedBundleCreateInput input)
        {
            return await _licensedTenantService.AddBundleToLicense(input);
        }
        
        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public async Task<LicensedBundleUpdateOutput> UpdateBundleFromLicense(LicensedBundleUpdateInput input)
        {
            return await _licensedTenantService.UpdateBundleFromLicense(input);
        }

        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public async Task<LicenseTenantDeleteOutput> RemoveBundleFromLicense(LicensedBundleDeleteInput input)
        {
            return await _licensedTenantService.RemoveBundleFromLicense(input);
        }

        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public async Task<LicensedAppCreateOutput> AddAppToLicense(LicensedAppCreateInput input)
        {
            return await _licensedTenantService.AddLooseAppToLicense(input);
        }

        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public async Task<LicenseTenantDeleteOutput> RemoveAppFromLicense(LicensedAppDeleteInput input)
        {
            var output =  await _licensedTenantService.RemoveAppFromLicense(input);
            return new LicenseTenantDeleteOutput
            {
                Success = output.Success,
                Errors = new List<BaseCrudResponseError<OperationValidation>>()
                {
                    new()
                    {
                        ErrorCode = output.ErrorCode
                    }
                }
            };
        }
        
        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public async Task<LicensedAppUpdateOutput> UpdateBundledAppFromLicense(LicensedAppUpdateInput input)
        {
            return await _licensedTenantService.UpdateBundledAppFromLicense(input);
        }

        [HttpPost]
        [Authorize(Policy.UpdateLicense)]
        public async Task<LicensedAppUpdateOutput> UpdateLooseAppFromLicense(LicensedAppUpdateInput input)
        {
            return  await _licensedTenantService.UpdateLooseAppFromLicense(input);
        }

        [HttpGet]
        public async Task<PagedResultDto<BundledLicensedAppOutput>> GetAllLicensedAppsInBundle([FromQuery] GetAllLicensedAppInput input)
        {
            var query = from licensedApp in _licensedApp
                where licensedApp.LicensedTenantId == input.LicensedTenantId &&
                      licensedApp.LicensedBundleId == input.LicensedBundleId && licensedApp.LicensedBundleId.HasValue
                join app in _apps on licensedApp.AppId equals app.Id join software in _softwares on app.SoftwareId equals software.Id
                select new BundledLicensedAppOutput
                {
                    Id = app.Id,
                    Name = app.Name,
                    Domain = app.Domain,
                    Status = licensedApp.Status,
                    SoftwareName = software.Name
                }; 
            
            query = string.IsNullOrEmpty(input.Sorting) ? query.ApplyAdvancedFilter(input.AdvancedFilter, app => app.Name) :
                query.ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting);

            var totalCount = await query.CountAsync();
            
            var apps = await AdvancedFilterBasicOperations.PageBy(query, input.SkipCount, input.MaxResultCount)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResultDto<BundledLicensedAppOutput>
            {
                Items = apps,
                TotalCount = totalCount
            };
        }

        [HttpGet]
        [Authorize(Policy.UpdateLicense)]
        public async Task<PagedResultDto<LooseLicensedAppOutput>> GetAllLooseLicensedApps([FromQuery] GetAllLooseLicensedAppInput input)
        {
            var totalCount = await _licensedApp.CountAsync(la => la.LicensedTenantId == input.LicensedTenantId && la.LicensedBundleId == null);
            var query = from licensedApp in _licensedApp
                    .Where(la => la.LicensedTenantId == input.LicensedTenantId && la.LicensedBundleId == null)
                    .PageBy(input.SkipCount, input.MaxResultCount)
                join app in _apps on licensedApp.AppId equals app.Id
                join software in _softwares on app.SoftwareId equals software.Id
                select new LooseLicensedAppOutput
                {
                    Domain = app.Domain,
                    Name = app.Name,
                    Id = app.Id,
                    Status = licensedApp.Status,
                    LicensingMode = licensedApp.LicensingMode,
                    LicensingModel = licensedApp.LicensingModel,
                    SoftwareId = software.Id,
                    SoftwareName = software.Name,
                    NumberOfLicenses = licensedApp.NumberOfLicenses,
                    AdditionalNumberOfLicenses = licensedApp.AdditionalNumberOfLicenses,
                    NumberOfTemporaryLicenses = licensedApp.NumberOfTemporaryLicenses,
                    ExpirationDateOfTemporaryLicenses = licensedApp.ExpirationDateOfTemporaryLicenses,
                    LicensedAppId = licensedApp.Id
                };

            var result = await query.ToListAsync();
            
            return new PagedResultDto<LooseLicensedAppOutput>
            {
                Items = result,
                TotalCount = totalCount
            };
        }

        [HttpGet]
        [TenantIdParameterHint("tenantId", ParameterLocation.Query)]
        public async Task<List<string>> GetLicensedAppsIdentifiersFromTenant(Guid tenantId)
        {
            return await _cacheService.GetOrCreateCacheForLicensedAppsIdentifiersFromTenant(tenantId, 
                async () => await _licenseRepository.GetLicensedAppsIdentifiersFromTenant(tenantId));
        }

        [HttpGet]
        [TenantIdParameterHint("tenantId", ParameterLocation.Query)]
        [UserNotRequired]
        public async Task<bool> CheckLicenseTenantIdExistence(Guid tenantId)
        {
            return await _licenseRepository.CheckLicenseTenantIdExistence(tenantId);
        }
        
        [HttpGet]
        [TenantIdParameterHint("tenantId", ParameterLocation.Query)]
        [UserNotRequired]
        public async Task<RootUserFromTenantId> GetRootUserFromTenantId(Guid tenantId)
        {
            var userRoot = await _licensedTenant.Select(tenant => new { tenant.Identifier, tenant.AdministratorEmail })
                    .FirstOrDefaultAsync(t => t.Identifier == tenantId);
            
            return new RootUserFromTenantId
            {
                Email = userRoot.AdministratorEmail,
                TenantId = userRoot.Identifier
            };
        }
        
        [HttpGet]
        [Authorize(Policy.UpdateLicense)]
        public async Task<PagedResultDto<BundledAndLooseAppOutput>> GetAllLicensedApps([FromQuery] GetAllBundledAndLooseLicensedAppInput input)
        {
            var totalCount = await _licensedApp.CountAsync(la => la.LicensedTenantId == input.LicensedTenantId);
            var licensedApps = await _licensedApp.Where(la => la.LicensedTenantId == input.LicensedTenantId)
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
            }).Where(a => licensedAppIds.Contains(a.Id)).ToListAsync();
            
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
        
        // CompanyId, TenantId and UserId is not available in following method
        [HttpGet]
        [TenantIdParameterHint("id", ParameterLocation.Query)]
        [UserNotRequired]
        public async Task<LicensingStatus?> GetTenantStatus(Guid id)
        {
            var tenant = await Repository.FirstOrDefaultAsync(t => t.Identifier == id);
            return tenant?.Status;
        }

        [HttpGet("/Licensing/LicensingManagement/Licensing/{cnpj}")]
        [UserNotRequired]
        [TenantNotRequired]
        public async Task<IActionResult> GetLicensingByCnpj([FromRoute] string cnpj)
        {
            var query = await Repository
                .Where(l => l.LicensedCnpjs.Contains(cnpj))
                .Select(l => new LicenseTenantCreateOutput
                {
                    Id = l.Id,
                    Identifier = l.Identifier,
                    Notes = l.NotesString,
                    Status = l.Status,
                    AccountId = l.AccountId,
                    AdministratorEmail = l.AdministratorEmail,
                    LicensedCnpjs = l.LicensedCnpjs,
                    OperationValidation = OperationValidation.NoError,
                    ExpirationDateTime = l.ExpirationDateTime,
                    LicenseConsumeType = l.LicenseConsumeType
                }).SingleOrDefaultAsync();
            
            
            if (query != null)
            {
                return Ok(query);
            }

            return NotFound(new NotFoundOutput
            {
                Code = NotFoundEnum.CouldNotFind
            });
        }
    }
}