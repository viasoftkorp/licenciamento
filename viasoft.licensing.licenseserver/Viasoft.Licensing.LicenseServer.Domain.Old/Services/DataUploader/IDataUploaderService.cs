using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.DataUploader
{
    public interface IDataUploaderService
    {
        Task<bool> UploadLicenseUsageInRealTime(LicenseUsageInRealTime input);
    }
}