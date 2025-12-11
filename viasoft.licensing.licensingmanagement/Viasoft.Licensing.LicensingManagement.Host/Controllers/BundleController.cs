using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.BaseCrud;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Extensions;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedBundle;
using Viasoft.Licensing.LicensingManagement.Domain.Authorizations;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Software;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class BundleController: BaseCrudController<Bundle, BundleCreateOutput, BundleCreateInput, BundleUpdateInput, BundleUpdateOutput, GetAllBundlesInput, BundleDeleteOutput, OperationValidation, string>
    {
        private readonly IRepository<Bundle> _bundles;
        private readonly IRepository<BundledApp> _bundledApps;
        private readonly IMapper _mapper;
        private readonly ILicenseRepository _licenseRepository;
        private readonly ISoftwareRepository _softwareRepository;
        private readonly IRepository<LicensedBundle> _licensedBundle;
        private readonly IBundleRepository _bundleRepository;
        private readonly IRepository<Software> _softwares;
        private readonly IRepository<NamedUserBundleLicense> _namedUserBundleLicense;

        public BundleController(IRepository<Bundle> bundles, IRepository<BundledApp> bundledApps, IMapper mapper, ILicenseRepository licenseRepository,
            ISoftwareRepository softwareRepository, IRepository<LicensedBundle> licensedBundle, IBundleRepository bundleRepository, IRepository<Software> softwares, IRepository<NamedUserBundleLicense> namedUserBundleLicense) : base(mapper, bundles)
        {
            _bundles = bundles;
            _bundledApps = bundledApps;
            _mapper = mapper;
            _licenseRepository = licenseRepository;
            _softwareRepository = softwareRepository;
            _licensedBundle = licensedBundle;
            _bundleRepository = bundleRepository;
            _softwares = softwares;
            _namedUserBundleLicense = namedUserBundleLicense;

            Authorizations.Create = Policy.CreateBundle;
            Authorizations.Update = Policy.UpdateBundle;
            Authorizations.Delete = Policy.DeleteBundle;
        }

        [HttpGet]
        public override async Task<PagedResultDto<BundleCreateOutput>> GetAll([FromQuery] GetAllBundlesInput input)
        {
            var query = from bundle in _bundles join software in _softwares on 
                    bundle.SoftwareId equals software.Id
                select new BundleCreateOutput()
                {
                    Id = bundle.Id,
                    Name = bundle.Name,
                    Identifier = bundle.Identifier,
                    IsActive = bundle.IsActive,
                    IsCustom = bundle.IsCustom,
                    SoftwareId = bundle.SoftwareId,
                    SoftwareName = software.Name
                };
            var totalCount = await query.CountAsync();

            query = !string.IsNullOrEmpty(input.Sorting) ? query.ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting)
                : query.ApplyAdvancedFilter(input.AdvancedFilter, bundle => bundle.Name);
            
            var result = await AdvancedFilterBasicOperations.PageBy(query, input.SkipCount, input.MaxResultCount)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResultDto<BundleCreateOutput>()
            {
                TotalCount = totalCount,
                Items = result
            };
        }

        [HttpPost]
        [Authorize(Policy.CreateBundle)]
        public override async Task<BundleCreateOutput> Create(BundleCreateInput input)
        {
            BundleCreateOutput output;
            
            if (_bundles.Any(a => a.Identifier == input.Identifier))
            {
                output  = _mapper.Map<BundleCreateOutput>(input);
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
            }
            else
            {
                var bundle = _mapper.Map<Bundle>(input);
                var newBundle = await _bundles.InsertAsync(bundle);
                output = _mapper.Map<BundleCreateOutput>(newBundle);
            }
            return output;
        }
        
        [HttpPost]
        [Authorize(Policy.UpdateBundle)]
        public async Task<BundledAppCreateOutput> AddAppToBundle(BundledAppCreateInput input)
        {
            BundledAppCreateOutput output;

            var isLicensedBundle = await _bundleRepository.CheckIfBundleIsLicensedInAnyLicensing(input.BundleId);
            
            if (_bundledApps.Any(a => a.BundleId == input.BundleId && a.AppId == input.AppId))
            {
                output  = _mapper.Map<BundledAppCreateOutput>(input);
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
            }
            else
            {
                var bundledApp = _mapper.Map<BundledApp>(input);
                var newBundledApp = await _bundledApps.InsertAsync(bundledApp);
                output = _mapper.Map<BundledAppCreateOutput>(newBundledApp);
            }
            output.IsBundleLicensedInAnyLicensing = isLicensedBundle;
            return output;
        }
        
        [HttpPost]
        [Authorize(Policy.UpdateBundle)]
        public override async Task<BundleUpdateOutput> Update(BundleUpdateInput input)
        {
            BundleUpdateOutput output;
            if (_bundles.Any(a => a.Identifier == input.Identifier && a.Id != input.Id))
            {
                output  = _mapper.Map<BundleUpdateOutput>(input);
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
            }
            else
            {
                var bundle = await _bundles.FindAsync(input.Id);
                _mapper.Map(input, bundle);
                var updatedBundle = await _bundles.UpdateAsync(bundle);
                output = _mapper.Map<BundleUpdateOutput>(updatedBundle);
            }

            return output;
        }
        
        [HttpDelete]
        [Authorize(Policy.DeleteBundle)]
        public override async Task<BundleDeleteOutput> Delete(Guid id)
        {

            var isBundleBeingUsed = await _licenseRepository.IsBundleBeingUsedByLicense(id);
            
            if (!isBundleBeingUsed)
            {
                await _bundles.DeleteAsync(id);
                return new BundleDeleteOutput
                {
                    Success = true
                };
            }
                
            return new BundleDeleteOutput
            {
                Success = false,
                Errors = new List<BaseCrudResponseError<OperationValidation>>
                {
                    new BaseCrudResponseError<OperationValidation>
                    {
                        ErrorCode = OperationValidation.UsedByOtherRegister
                    }
                }
            };
        }

        [HttpPost]
        [Authorize(Policy.UpdateBundle)]
        public async Task<BundleDeleteOutput> RemoveAppFromBundle(BundledAppDeleteInput input)
        {            
            var bundledAppToDelete = await _bundledApps.FirstOrDefaultAsync(a => a.BundleId == input.BundleId && a.AppId == input.AppId);
            
            var isLicensedBundle = await _bundleRepository.CheckIfBundleIsLicensedInAnyLicensing(input.BundleId);

            if(bundledAppToDelete != null)
                await _bundledApps.DeleteAsync(bundledAppToDelete);

            return new BundleDeleteOutput
            {
                Success = true,
                IsBundleLicensedInAnyLicensing = isLicensedBundle
            };
        }

        protected override (Expression<Func<Bundle, string>>, bool) DefaultGetAllSorting()
        {
            return (bundle => bundle.Identifier, true);
        }

        [HttpGet]
        public async Task<PagedResultDto<LicensedBundleOutput>> GetAllLicensedBundle([FromQuery] GetAllLicensedBundleInput input)
        {
            var query = from licensedBundle in _licensedBundle
                    .Where(lb => lb.LicensedTenantId == input.LicensedTenantId)
                join bundle in _bundles on licensedBundle.BundleId equals bundle.Id
                join software in _softwares on bundle.SoftwareId equals software.Id
                select new LicensedBundleOutput
                {
                    Id = bundle.Id,
                    Identifier = bundle.Identifier,
                    Name = bundle.Name,
                    IsActive = bundle.IsActive,
                    IsCustom = bundle.IsCustom,
                    LicensingMode = licensedBundle.LicensingMode,
                    LicensingModel = licensedBundle.LicensingModel,
                    SoftwareId = software.Id,
                    SoftwareName = software.Name,
                    NumberOfLicenses = licensedBundle.NumberOfLicenses,
                    NumberOfTemporaryLicenses = licensedBundle.NumberOfTemporaryLicenses,
                    ExpirationDateOfTemporaryLicenses = licensedBundle.ExpirationDateOfTemporaryLicenses,
                    LicensedBundleId = licensedBundle.Id,
                    Status = licensedBundle.Status,
                    NumberOfUsedLicenses = (from namedUserBundleLicense in _namedUserBundleLicense 
                        where namedUserBundleLicense.LicensedBundleId == licensedBundle.Id
                        select namedUserBundleLicense).Count()
                };
            query = query.ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting);
            var totalCount = await query.CountAsync();
            var result = await query.PageBy(input.SkipCount, input.MaxResultCount).ToListAsync();
            
            return new PagedResultDto<LicensedBundleOutput>
            {
                Items = result,
                TotalCount = totalCount
            };
        }

        [HttpGet]
        public async Task<PagedResultDto<BundleCreateOutput>> GetAllBundlesMinusLicensedBundles([FromQuery] GetAllBundlesInput input)
        {
            IQueryable<Bundle> query;
            
            var licensedBundles = await _licensedBundle.Where(bundle => bundle.LicensedTenantId == input.LicensedTenantId)
                .Select(bundle => bundle.BundleId)
                .ToListAsync();
            
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                query = Repository.AsQueryable().ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting);
            }
            else
            { 
                var (defaultSorting, ascSorting) = DefaultGetAllSorting();
                query = Repository.AsQueryable().ApplyAdvancedFilter(input.AdvancedFilter, defaultSorting, ascSorting);
            }

            query = query.Where(bundle => !licensedBundles.Contains(bundle.Id) && bundle.IsActive);

            query = ApplyCustomFilters(query, input);

            var totalCount = await query.CountAsync();

            var bundles = await AdvancedFilterBasicOperations.PageBy(query, input.SkipCount, input.MaxResultCount).ToListAsync();

            var items = Mapper.Map<List<BundleCreateOutput>>(bundles);

            var ids = items.Select(item => item.SoftwareId).Distinct().ToList();

            var names = await _softwareRepository.GetSoftwareNamesFromIdList(ids);

            foreach (var item in items)
            {
                item.SoftwareName = names[item.SoftwareId];
            }
            
            var output = new PagedResultDto<BundleCreateOutput>
            {
                Items = items,
                TotalCount = totalCount
            };

            return output;
        }

        [HttpPost]
        public async Task<IActionResult> GetBundleIdsFromBundleIdentifiers([FromBody] List<string> bundleIdentifiers)
        {
            if (bundleIdentifiers.Count == 0)
            {
                return BadRequest();
            }
            
            var output = await _bundles.Where(b => bundleIdentifiers.Contains(b.Identifier)).Select(b => b.Id).ToListAsync();
            
            return Ok(output);
        }
    }
}