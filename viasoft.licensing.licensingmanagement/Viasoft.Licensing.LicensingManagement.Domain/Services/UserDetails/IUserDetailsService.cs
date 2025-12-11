using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.Services.UserDetails.DTO;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.UserDetails
{
    public interface IUserDetailsService
    {
        Task<UserDetail> GetUserDetailsAsync();
    }
}