namespace Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId
{
    public interface IProvideHardwareIdService
    {
        public string ProvideHardwareId(bool useSimpleHardwareId);

        public void Reset();
    }
}