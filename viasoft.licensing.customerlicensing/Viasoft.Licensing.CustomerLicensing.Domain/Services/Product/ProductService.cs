using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Viasoft.Core.ApiClient;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.CustomerLicensing.Domain.Consts;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Product;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageInRealTime;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.Product
{
    public class ProductService : IProductService, ITransientDependency
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        private readonly ILicenseUsageInRealTimeService _licenseUsageInRealTimeService;

        public ProductService(IApiClientCallBuilder apiClientCallBuilder,
            ILicenseUsageInRealTimeService licenseUsageInRealTimeService)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
            _licenseUsageInRealTimeService = licenseUsageInRealTimeService;
        }

        public async Task<PagedResultDto<ProductOutput>> GetAll(Guid licensedTenantId, GetAllProductsInput input)
        {
            var gatewayCall = _apiClientCallBuilder
                .WithEndpoint(
                    ExternalServicesConsts.LicensingManagement.LicensedBundle.GetAllProducts(licensedTenantId, input))
                .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                
                .Build();

            var gatewayCallResponse = await gatewayCall.ResponseCallAsync<PagedResultDto<ProductOutput>>();

            return gatewayCallResponse;
        }

        public async Task<ProductOutput> GetById(Guid licensedTenantId, Guid productId, GetProductByIdInput input)
        {
            var gatewayCall = _apiClientCallBuilder 
                .WithEndpoint(
                    ExternalServicesConsts.LicensingManagement.LicensedBundle.GetProductById(licensedTenantId,
                        productId, input))
                .WithServiceName(ExternalServicesConsts.LicensingManagement.ServiceName)
                .WithHttpMethod(HttpMethod.Get)
                
                .Build();

            var gatewayCallResponse = await gatewayCall.ResponseCallAsync<ProductOutput>();

            if (gatewayCallResponse.LicensingModel == LicensingModels.Floating)
            {
                var identifierList = new List<string>()
                {
                    gatewayCallResponse.Identifier
                };

                var licenseUsage = gatewayCallResponse.ProductType == ProductType.LicensedBundle
                    ? await _licenseUsageInRealTimeService.GetLicensesConsumed(input.LicensingIdentifier,
                        identifierList, new List<string>())
                    : await _licenseUsageInRealTimeService.GetLicensesConsumed(input.LicensingIdentifier,
                        new List<string>(), identifierList);

                if (licenseUsage.TryGetValue(gatewayCallResponse.Identifier, out var numberOfUsedLicenses))
                {
                    gatewayCallResponse.NumberOfUsedLicenses = numberOfUsedLicenses;
                }
            }
            return gatewayCallResponse;
        }

        public async Task<List<GetAllLicenseUsageOutput>> GetAllLicenseUsage(Guid licensingIdentifier,
            List<string> bundleIdentifiers, List<string> appIdentifiers)
        {
            var licenseUsage = await _licenseUsageInRealTimeService.GetLicensesConsumed(licensingIdentifier,
                bundleIdentifiers,
                appIdentifiers);
            var licenseUsageList = new List<GetAllLicenseUsageOutput>();
            foreach (var bundleIdentifier in bundleIdentifiers)
            {
                licenseUsageList.Add(new GetAllLicenseUsageOutput()
                {
                    ProductIdentifier = bundleIdentifier,
                    AppLicensesConsumed = licenseUsage.TryGetValue(bundleIdentifier, out var licensesConsumed) ? licensesConsumed : 0
                });
            }

            foreach (var appIdentifier in appIdentifiers)
            {
                licenseUsageList.Add(new GetAllLicenseUsageOutput()
                {
                    ProductIdentifier = appIdentifier,
                    AppLicensesConsumed = licenseUsage.TryGetValue(appIdentifier, out var licensesConsumed) ? licensesConsumed : 0
                });
            }

            return licenseUsageList;
        }
    }
}