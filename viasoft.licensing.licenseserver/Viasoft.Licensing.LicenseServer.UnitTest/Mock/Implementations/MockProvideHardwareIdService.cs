using Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId;

namespace Viasoft.Licensing.LicenseServer.UnitTest.Mock.Implementations
{
    public class MockProvideHardwareIdService: IProvideHardwareIdService
    {
        public string ProvideHardwareId(bool useSimpleHardwareId)
        {
            return "hardwareId " + useSimpleHardwareId;
        }
    }
}