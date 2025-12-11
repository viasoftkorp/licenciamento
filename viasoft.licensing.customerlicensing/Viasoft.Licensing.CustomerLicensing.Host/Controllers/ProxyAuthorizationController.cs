using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.Authorization.Proxy;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    [AmbientDataNotRequired]
    public class ProxyAuthorizationController: AuthorizationProxyController
    {
        public ProxyAuthorizationController(IAuthorizationProxyService authorizationProxyService, IAmbientDataCallOptionsResolver ambientDataCallOptionsResolver) : base(authorizationProxyService, ambientDataCallOptionsResolver)
        {
            
        }

        [HttpGet("authorizations/policies")]
        public List<string> GetPolicies()
        {
            return new List<string>
            {
                "Settings.Read",
                "Settings.Update",
                "Environments.Read",
                "Environments.Update"
            };
        }
    }
}