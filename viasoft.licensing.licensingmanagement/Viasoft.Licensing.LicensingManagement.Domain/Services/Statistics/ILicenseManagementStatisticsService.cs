using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.Statistics;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.Statistics
{
    public interface ILicenseManagementStatisticsService
    {
        public Task<NumberOfAppsTotalOutput> GetNumberOfAppsInTotal();
    }
}