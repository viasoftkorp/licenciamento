using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Classes;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.LicensingManager
{
    public interface ILicensingManagerService
    {
        Task<ConsumeLicenseOutput> ConsumeLicense(ConsumeLicenseInput input);
        
        Task<ReleaseLicenseOutput> ReleaseLicense(ReleaseLicenseInput input);

        LicenseTenantStatusCurrent GetCurrentState();

        Task EvaluateAndReleaseLicensesBasedOnHeartbeat();
        
        Task<RefreshAppLicenseInUseByUserOutput> RefreshAppLicenseInUseByUser(RefreshAppLicenseInUseByUserInput input);

        List<KeyValuePair<string, List<AppLicenseConsumer>>> GetAllLicensesInUse();
        
        Task RestoreLicensesInUse(List<KeyValuePair<string, List<AppLicenseConsumer>>> licensesInUse);
    }
}