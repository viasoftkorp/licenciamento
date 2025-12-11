using System.Threading.Tasks;
using Rebus.Handlers;
using Viasoft.Licensing.LicenseServer.Domain.Catalogs;
using Viasoft.Licensing.LicenseServer.Domain.Messages;

namespace Viasoft.Licensing.LicenseServer.Host.Handlers
{
    public class LicensingDetailsUpdatedHandler: IHandleMessages<LicensingDetailsUpdated>
    {
        private readonly ITenantCatalog _tenantCatalog;
        
        public LicensingDetailsUpdatedHandler(ITenantCatalog tenantCatalog)
        {
            _tenantCatalog = tenantCatalog;
        }
        
        public async Task Handle(LicensingDetailsUpdated message)
        {
            await _tenantCatalog.RefreshTenantLicensing(message);
        }
    }
}