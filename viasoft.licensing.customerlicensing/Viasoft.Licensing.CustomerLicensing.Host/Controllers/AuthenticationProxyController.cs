using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.Authentication.Proxy;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    [AmbientDataNotRequired]
    public class AuthenticationProxyController : Core.Authentication.Proxy.AuthenticationProxyController
    {
        public AuthenticationProxyController(IAuthenticationProxyService authenticationProxyService, IAmbientDataCallOptionsResolver ambientDataCallOptionsResolver) : base(authenticationProxyService, ambientDataCallOptionsResolver)
        {
        }
    }
}