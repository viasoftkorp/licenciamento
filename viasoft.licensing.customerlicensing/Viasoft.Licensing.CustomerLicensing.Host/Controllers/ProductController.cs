using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Product;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.Product;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        
        [HttpGet("/licensing/customer-licensing/licenses/{licensedTenantId:guid}/products")]
        public async Task<PagedResultDto<ProductOutput>> GetAll([FromRoute] Guid licensedTenantId, [FromQuery] GetAllProductsInput input)
        {
            return await _productService.GetAll(licensedTenantId, input);
        }
        
        [HttpGet("/licensing/customer-licensing/licenses/{licensedTenantId:guid}/products/{productId:guid}")]
        public async Task<ProductOutput> GetById([FromRoute] Guid licensedTenantId, [FromRoute] Guid productId, [FromQuery] GetProductByIdInput input)
        {
            return await _productService.GetById(licensedTenantId, productId, input);
        }

        [HttpGet("/licensing/customer-licensing/licenses/{licensingIdentifier:guid}/products/license-usage")]
        public async Task<List<GetAllLicenseUsageOutput>> GetAllLicenseUsage([FromRoute] Guid licensingIdentifier, [FromQuery] List<string> bundleIdentifiers, [FromQuery] List<string> appIdentifiers)
        {
            return await _productService.GetAllLicenseUsage(licensingIdentifier, bundleIdentifiers, appIdentifiers);
        }
    }
}