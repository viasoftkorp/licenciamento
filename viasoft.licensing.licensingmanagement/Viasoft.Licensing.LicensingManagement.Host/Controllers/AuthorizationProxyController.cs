using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Core.Authorization.Proxy;

namespace Viasoft.Licensing.LicensingManagement.Host.Controllers
{
    public class AuthorizationProxyController: Core.Authorization.Proxy.AuthorizationProxyController
    {
        public AuthorizationProxyController(IAuthorizationProxyService authorizationProxyService, IAmbientDataCallOptionsResolver ambientDataCallOptionsResolver) : base(authorizationProxyService, ambientDataCallOptionsResolver)
        {
        }
    }
}