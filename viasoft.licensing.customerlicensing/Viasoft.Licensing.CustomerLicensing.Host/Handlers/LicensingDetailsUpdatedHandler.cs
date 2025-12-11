using System.Threading.Tasks;
using Rebus.Handlers;
using Viasoft.Licensing.CustomerLicensing.Domain.Messages;
using Viasoft.Licensing.CustomerLicensing.Domain.Services.LicenseUsageInRealTime;

namespace Viasoft.Licensing.CustomerLicensing.Host.Handlers
{
    public class LicensingDetailsUpdatedHandler: IHandleMessages<LicensingDetailsUpdated>
    {
        private readonly ILicenseUsageInRealTimeService _licenseUsageInRealTimeService;

        public LicensingDetailsUpdatedHandler(ILicenseUsageInRealTimeService licenseUsageInRealTimeService)
        {
            _licenseUsageInRealTimeService = licenseUsageInRealTimeService;
        }

        public async Task Handle(LicensingDetailsUpdated message)
        {
            await _licenseUsageInRealTimeService.UpdateLicensedApps(message);
        }
    }
}