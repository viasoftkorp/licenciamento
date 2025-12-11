using System.Threading.Tasks;
using Viasoft.Core.Authentication.Proxy.Dtos.Outputs;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.Authentication;

public interface IAuthenticationCaller
{
    Task<GetUserOutput> GetUserIdByEmail(string email);
}