using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Core.Authentication.Proxy;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class AuthenticationProxyController : Core.Authentication.Proxy.AuthenticationProxyController
    {
        public AuthenticationProxyController(IAuthenticationProxyService authenticationProxyService, IAmbientDataCallOptionsResolver ambientDataCallOptionsResolver) : base(authenticationProxyService, ambientDataCallOptionsResolver)
        {
        }
    }
}