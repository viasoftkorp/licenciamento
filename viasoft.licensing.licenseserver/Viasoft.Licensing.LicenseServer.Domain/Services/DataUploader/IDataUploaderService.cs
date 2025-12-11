using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models;

namespace Viasoft.Licensing.LicenseServer.Domain.Services.DataUploader
{
    public interface IDataUploaderService
    {
        Task<bool> UploadLicenseUsageInRealTime(LicenseUsageInRealTimeOutput input);
    }
}