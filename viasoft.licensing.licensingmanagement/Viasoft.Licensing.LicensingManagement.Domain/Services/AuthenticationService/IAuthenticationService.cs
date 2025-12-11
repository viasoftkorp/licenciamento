using System;
using System.Threading.Tasks;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Authentication;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.User;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        public Task<PagedResultDto<UsersGetAllOutput>> GetAllUsersFromTenantIdentifier(Guid identifier,
            GetAllUsersInput getAllUsersInput);
    }
}