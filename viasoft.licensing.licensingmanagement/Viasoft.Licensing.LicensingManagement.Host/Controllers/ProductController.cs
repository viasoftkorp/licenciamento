using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AspNetCore.Controller;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Product;
using Viasoft.Licensing.LicensingManagement.Domain.Services.Product;
using Viasoft.Licensing.LicensingManagement.Host.AmbientData.Contributor;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductsService _productsService;
        
        public ProductController(IProductsService productsService)
        {
            _productsService = productsService;
        }
        
        [HttpGet("/licensing/licensing-management/licenses/{licensedTenantId:guid}/products/")]
        [TenantIdParameterHint("licensedTenantId", ParameterLocation.Route, TenantIdParameterKind.LicensedTenantId)]
        public async Task<PagedResultDto<ProductOutput>> GetAll([FromRoute] Guid licensedTenantId, [FromQuery] GetAllProductsInput input)
        {
            return await _productsService.GetAll(licensedTenantId, input);
        }
        
        [HttpGet("/licensing/licensing-management/licenses/{licensedTenantId:guid}/products/{productId:guid}")]
        [TenantIdParameterHint("licensedTenantId", ParameterLocation.Route, TenantIdParameterKind.LicensedTenantId)]
        public async Task<ProductOutput> GetById([FromRoute] Guid licensedTenantId, [FromRoute] Guid productId, [FromQuery] GetProductByIdInput input)
        {
            return await _productsService.GetById(licensedTenantId, productId, input.ProductType);
        }
    }
}