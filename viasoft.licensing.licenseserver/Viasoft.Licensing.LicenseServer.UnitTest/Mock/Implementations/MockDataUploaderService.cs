using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing;
using Viasoft.Licensing.LicenseServer.Domain.Old.Services.DataUploader;

namespace Viasoft.Licensing.LicenseServer.UnitTest.Mock.Implementations
{
    public class MockDataUploaderService: IDataUploaderService
    {
        public Task<bool> UploadLicenseUsageInRealTime(LicenseUsageInRealTime input)
        {
            return Task.FromResult(true);
        }
    }
}