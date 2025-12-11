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
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.LicensingManagement.Domain.AppMessages;
using Viasoft.Licensing.LicensingManagement.Domain.Authorizations;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.App;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Bundle;
using Viasoft.Licensing.LicensingManagement.Domain.Repositories.Software;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class AppController: BaseCrudController<App, AppCreateOutput, AppCreateInput, AppUpdateInput, AppUpdateOutput , GetAllAppsInput, AppDeleteOutput, OperationValidation, string>
    {
        private readonly IRepository<App> _apps;
        private readonly IMapper _mapper;
        private readonly IBundleRepository _bundleRepository;
        private readonly ILicenseRepository _licenseRepository;
        private readonly ISoftwareRepository _softwareRepository;
        private readonly IRepository<BundledApp> _bundledApps;
        private readonly IServiceBus _serviceBus;
        private readonly IUnitOfWork _unitOfWork;

        public AppController(IRepository<App> apps, IMapper mapper, IBundleRepository bundleRepository, ILicenseRepository licenseRepository, ISoftwareRepository softwareRepository, IRepository<BundledApp> bundledApps, IServiceBus serviceBus, IUnitOfWork unitOfWork) : base(mapper, apps)
        {
            _apps = apps;
            _mapper = mapper;
            _bundleRepository = bundleRepository;
            _licenseRepository = licenseRepository;
            _softwareRepository = softwareRepository;
            _bundledApps = bundledApps;
            _serviceBus = serviceBus;
            _unitOfWork = unitOfWork;

            Authorizations.Create = Policy.CreateApp;
            Authorizations.Update = Policy.UpdateApp;
            Authorizations.Delete = Policy.DeleteApp;
        }

        [HttpGet]
        public override async Task<PagedResultDto<AppCreateOutput>> GetAll([FromQuery] GetAllAppsInput input)
        {
            var output = await base.GetAll(input);
            
            var softwareIds = output.Items.Select(item => item.SoftwareId).Distinct().ToList();
            var softwareNames = await _softwareRepository.GetSoftwareNamesFromIdList(softwareIds);
            
            output.Items.ForEach(bundle => bundle.SoftwareName = softwareNames[bundle.SoftwareId]);
            
            return output;
        }

        protected override IQueryable<App> ApplyCustomFilters(IQueryable<App> query, GetAllAppsInput input)
        {
            return query
                .WhereIf(input.SoftwareId != Guid.Empty, app => app.SoftwareId == input.SoftwareId)
                .WhereIf(input.AlreadyLicensedApps != null, app => !input.AlreadyLicensedApps.Contains(app.Id))
                .Where(app => app.IsActive);
        }

        [HttpGet]
        public async Task<PagedResultDto<AppCreateOutput>> GetAllActiveApps([FromQuery] GetAllAppsInput input)
        {

            var totalCount = await _apps.CountAsync();
            
            var apps = await _apps.PageBy(input.SkipCount, input.MaxResultCount).Where(app => app.IsActive).ToListAsync();
            
            var items = Mapper.Map<List<AppCreateOutput>>(apps);
            
            var ids = items.Select(item => item.SoftwareId).Distinct().ToList();
            var names = await _softwareRepository.GetSoftwareNamesFromIdList(ids);
            
            items.ForEach(app => app.SoftwareName = names[app.SoftwareId]);
            
            var output = new PagedResultDto<AppCreateOutput>
            {
                Items = items, TotalCount = totalCount
            };
            
            return output;
        }
        
        [HttpPost]
        [Authorize(Policy.CreateApp)]
        public override async Task<AppCreateOutput> Create(AppCreateInput input)
        {
            AppCreateOutput output;
            if (await _apps.AnyAsync(a => a.Identifier == input.Identifier))
            {
                output  = _mapper.Map<AppCreateOutput>(input);
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
            }
            else
            {
                var app = _mapper.Map<App>(input);
                var newApp = await _apps.InsertAsync(app);
                output = _mapper.Map<AppCreateOutput>(newApp);
            }
            return output;
        }
        
        [HttpPost]
        [Authorize(Policy.UpdateApp)]
        public override async Task<AppUpdateOutput> Update(AppUpdateInput input)
        {
            AppUpdateOutput output;
            if (await _apps.AnyAsync(a => a.Identifier == input.Identifier && a.Id != input.Id))
            {
                output  = _mapper.Map<AppUpdateOutput>(input);
                output.OperationValidation = OperationValidation.DuplicatedIdentifier;
            }
            else
            {
                var app = await _apps.FindAsync(input.Id);
                _mapper.Map(input, app);
                
                using (_unitOfWork.Begin())
                {
                    var updatedApp = await _apps.UpdateAsync(app);
                    var updatedAppMessage = _mapper.Map<AppUpdatedMessage>(updatedApp);
                    await _serviceBus.Publish(updatedAppMessage);
                    output = _mapper.Map<AppUpdateOutput>(updatedApp);
                    await _unitOfWork.CompleteAsync();
                }
            }

            return output;
        }
        
        [HttpDelete]
        [Authorize(Policy.DeleteApp)]
        public override async Task<AppDeleteOutput> Delete(Guid id)
        {
            var appUsedByOtherRegister = await _bundleRepository.IsAppBeingUsedByBundles(id) || await _licenseRepository.IsAppBeingUsedByLicense(id);

            if (!appUsedByOtherRegister)
            {
                await _apps.DeleteAsync(id);
                return new AppDeleteOutput { Success = true };
            }
            
            return new AppDeleteOutput
            {
                Success = false,
                Errors = new List<BaseCrudResponseError<OperationValidation>>
                {
                    new BaseCrudResponseError<OperationValidation>()
                    {
                        ErrorCode = OperationValidation.UsedByOtherRegister
                    }
                }
            };
        }
        
        protected override (Expression<Func<App, string>>, bool) DefaultGetAllSorting()
        {
            return (m => m.Identifier, true);
        }
        
        [HttpGet]
        public async Task<PagedResultDto<AppOutput>> GetAllAppsInBundle([FromQuery] GetAllAppsInBundleInput input)
        {
            var query = from bundledApp in _bundledApps where bundledApp.BundleId == input.BundleId 
                join app in _apps on bundledApp.AppId equals app.Id
                select new AppOutput 
                {
                    Id = app.Id,
                    Identifier = app.Identifier,
                    SoftwareId = app.SoftwareId,
                    IsActive = app.IsActive,
                    Name = app.Name,
                    Domain = app.Domain
                };
            var totalCount = await query.CountAsync();
            
            query = !string.IsNullOrEmpty(input.Sorting) ? query.ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting)
                : query.ApplyAdvancedFilter(input.AdvancedFilter, bundle => bundle.Name);
            
            var result = await AdvancedFilterBasicOperations.PageBy(query, input.SkipCount, input.MaxResultCount)
                .AsNoTracking()
                .ToListAsync();
            
            return new PagedResultDto<AppOutput>
            {
                Items = result,
                TotalCount = totalCount
            };
        }
    }
}