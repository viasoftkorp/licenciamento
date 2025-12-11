using System;
using System.Threading.Tasks;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Product;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.Product
{
    public interface IProductsService
    {
        Task<PagedResultDto<ProductOutput>> GetAll(Guid licensedTenantId, GetAllProductsInput input);
        Task<ProductOutput> GetById(Guid licensedTenantId, Guid productId, ProductType productType);
    }
}