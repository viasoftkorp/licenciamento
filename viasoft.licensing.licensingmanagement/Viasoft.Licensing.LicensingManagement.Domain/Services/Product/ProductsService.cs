using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Data.Extensions.Filtering.AdvancedFilter;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Product;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.Extensions;


namespace Viasoft.Licensing.LicensingManagement.Domain.Services.Product
{
    public class ProductsService: IProductsService, ITransientDependency
    {
        private readonly IRepository<Entities.LicensedBundle> _licensedBundles;
        private readonly IRepository<Entities.LicensedApp> _licensedApps;
        private readonly IRepository<Entities.Bundle> _bundles;
        private readonly IRepository<Entities.App> _apps;
        private readonly IRepository<Entities.Software> _softwares;
        private readonly IRepository<Entities.NamedUserBundleLicense> _namedUserBundleLicenses;
        private readonly IRepository<Entities.NamedUserAppLicense> _namedUserAppLicenses;

        public ProductsService(IRepository<Entities.LicensedBundle> licensedBundles, IRepository<Entities.LicensedApp> licensedApps, IRepository<Entities.Bundle> bundles, IRepository<Entities.App> apps, IRepository<Entities.Software> softwares, IRepository<Entities.NamedUserBundleLicense> namedUserBundleLicenses, IRepository<Entities.NamedUserAppLicense> namedUserAppLicenses)
        {
            _licensedBundles = licensedBundles;
            _licensedApps = licensedApps;
            _bundles = bundles;
            _apps = apps;
            _softwares = softwares;
            _namedUserBundleLicenses = namedUserBundleLicenses;
            _namedUserAppLicenses = namedUserAppLicenses;
        }
        public async Task<PagedResultDto<ProductOutput>> GetAll(Guid licensedTenantId, GetAllProductsInput input)
        {
            var query = (from licensedBundle in _licensedBundles
                where licensedBundle.LicensedTenantId == licensedTenantId
                join bundle in _bundles on licensedBundle.BundleId equals bundle.Id
                join software in _softwares on bundle.SoftwareId equals software.Id
                select new ProductOutput()
                {
                    Id = licensedBundle.Id,
                    ProductId = bundle.Id,
                    Identifier = bundle.Identifier,
                    Name = bundle.Name,
                    IsActive = bundle.IsActive,
                    SoftwareId = bundle.SoftwareId,
                    SoftwareName = software.Name,
                    NumberOfLicenses = licensedBundle.NumberOfLicenses,
                    LicensingModel = licensedBundle.LicensingModel,
                    LicensingMode = licensedBundle.LicensingMode,
                    Status = (ProductStatus) licensedBundle.Status,
                    ProductType = ProductType.LicensedBundle,
                    ExpirationDateTime = licensedBundle.ExpirationDateTime,
                    NumberOfUsedLicenses = (from namedUserBundleLicense in _namedUserBundleLicenses
                        where namedUserBundleLicense.LicensedBundleId == licensedBundle.Id
                        select namedUserBundleLicense).Count()
                }).Union(from licensedApp in _licensedApps
                where licensedApp.LicensedTenantId == licensedTenantId && licensedApp.LicensedBundleId == null
                join app in _apps on licensedApp.AppId equals app.Id
                join software in _softwares on app.SoftwareId equals software.Id
                select new ProductOutput()
                {
                    Id = licensedApp.Id,
                    ProductId = app.Id,
                    Identifier = app.Identifier,
                    Name = app.Name,
                    IsActive = app.IsActive,
                    SoftwareId = app.SoftwareId,
                    SoftwareName = software.Name,
                    NumberOfLicenses = licensedApp.NumberOfLicenses,
                    LicensingModel = licensedApp.LicensingModel,
                    LicensingMode = licensedApp.LicensingMode,
                    Status = (ProductStatus)licensedApp.Status,
                    ProductType = ProductType.LicensedApp,
                    ExpirationDateTime = licensedApp.ExpirationDateTime,
                    NumberOfUsedLicenses = (from namedUserAppLicense in _namedUserAppLicenses 
                        where namedUserAppLicense.LicensedAppId == licensedApp.Id
                        select namedUserAppLicense).Count()
                });

            var totalCount = await query.CountAsync();
            
            if (!string.IsNullOrEmpty(input.Sorting))
                query = query.ApplyAdvancedFilter(input.AdvancedFilter, input.Sorting);
            else
            {
                var (defaultSorting, ascSorting) = DefaultGetAllSorting();
                query = query.ApplyAdvancedFilter(input.AdvancedFilter, defaultSorting, ascSorting);
            }

            var result = await AdvancedFilterBasicOperations.PageBy(query, input.SkipCount, input.MaxResultCount)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResultDto<ProductOutput>()
            {
                TotalCount = totalCount,
                Items = result
            };  
        }

        public async Task<ProductOutput> GetById(Guid licensedTenantId, Guid productId, ProductType productType)
        {
            if (productType == ProductType.LicensedBundle)
            {
                return await _licensedBundles
                    .Where(e => e.Id == productId && e.LicensedTenantId == licensedTenantId)
                    .Join(_bundles, licensedBundle => licensedBundle.BundleId, bundle => bundle.Id,
                        (licensedBundle, bundle) => new ProductOutput()
                        {
                            Id = licensedBundle.Id,
                            Identifier = bundle.Identifier,
                            Name = bundle.Name,
                            IsActive = bundle.IsActive,
                            SoftwareId = bundle.SoftwareId,
                            NumberOfLicenses = licensedBundle.NumberOfLicenses,
                            LicensingModel = licensedBundle.LicensingModel,
                            LicensingMode = licensedBundle.LicensingMode,
                            Status = licensedBundle.Status.ToProductStatus(),
                            ProductType = ProductType.LicensedBundle,
                            ExpirationDateTime = licensedBundle.ExpirationDateTime,
                            NumberOfUsedLicenses = (from namedUserBundleLicense in _namedUserBundleLicenses 
                                where namedUserBundleLicense.LicensedBundleId == licensedBundle.Id
                                select namedUserBundleLicense).Count()
                        })
                    .FirstOrDefaultAsync();
            }

            return await _licensedApps
                .Where(e => e.Id == productId && e.LicensedTenantId == licensedTenantId)
                .Join(_apps, licensedApp => licensedApp.AppId, app => app.Id,
                    (licensedApp, app) => new ProductOutput()
                    {
                        Id = licensedApp.Id,
                        Identifier = app.Identifier,
                        Name = app.Name,
                        IsActive = app.IsActive,
                        SoftwareId = app.SoftwareId,
                        NumberOfLicenses = licensedApp.NumberOfLicenses,
                        LicensingModel = licensedApp.LicensingModel,
                        LicensingMode = licensedApp.LicensingMode,
                        Status = licensedApp.Status.ToProductStatus(),
                        ProductType = ProductType.LicensedApp,
                        ExpirationDateTime = licensedApp.ExpirationDateTime,
                        NumberOfUsedLicenses = (from namedUserAppLicense in _namedUserAppLicenses
                            where namedUserAppLicense.LicensedAppId == licensedApp.Id
                            select namedUserAppLicense).Count()
                    }).FirstOrDefaultAsync();
        }
        
        private static (Expression<Func<ProductOutput, string>>, bool) DefaultGetAllSorting()
        {
            return (b => b.Name, true);
        }
    }
}