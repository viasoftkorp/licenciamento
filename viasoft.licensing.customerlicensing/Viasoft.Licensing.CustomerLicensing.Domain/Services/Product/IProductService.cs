using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Product;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.Product
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductOutput>> GetAll(Guid licensedTenantId, GetAllProductsInput input);
        Task<ProductOutput> GetById(Guid licensedTenantId, Guid productId, GetProductByIdInput input);
        Task<List<GetAllLicenseUsageOutput>> GetAllLicenseUsage(Guid licensingIdentifier, List<string> bundleIdentifiers, List<string> appIdentifiers);
    }
}