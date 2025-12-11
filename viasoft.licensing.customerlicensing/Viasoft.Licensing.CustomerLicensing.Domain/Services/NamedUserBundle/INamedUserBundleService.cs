using System;
using System.Threading.Tasks;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserProduct;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Services.NamedUserBundle
{
    public interface INamedUserBundleService
    {
        Task<PagedResultDto<GetAllUsersOutput>> GetAllUsers(Guid licensingIdentifier, GetAllUsersInput input);
        Task<AddNamedUserToProductOutput> AddNamedUserToProduct(Guid licensedTenantId, Guid productId, CreateNamedUserProductLicenseInput input);
        Task<UpdateNamedUsersFromProductOutput> UpdateNamedUserFromProduct(Guid licensedTenantId, Guid productId, string namedUserEmail, UpdateNamedUserProductLicenseInput input);
        Task<RemoveNamedUserFromProductOutput> RemoveNamedUserFromProduct(Guid licensedTenant, Guid productId, string namedUserEmail, ProductType productType);
    }
}