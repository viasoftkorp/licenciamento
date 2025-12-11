using Viasoft.Core.AmbientData.Abstractions;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.Dashboard.Proxy;

namespace Viasoft.Licensing.CustomerLicensing.Host.Controllers
{
    [AmbientDataNotRequired]
    public class ProxyDashboardController: DashboardProxyController
    {
        public ProxyDashboardController(IDashboardProxyService dashboardProxyService, IAmbientDataCallOptionsResolver ambientDataCallOptionsResolver) : base(dashboardProxyService, ambientDataCallOptionsResolver)
        {
        }
    }
}