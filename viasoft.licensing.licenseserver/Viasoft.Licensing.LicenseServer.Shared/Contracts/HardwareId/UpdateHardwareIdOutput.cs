using Viasoft.Licensing.LicenseServer.Shared.Enums;

namespace Viasoft.Licensing.LicenseServer.Shared.Contracts.HardwareId
{
    public class UpdateHardwareIdOutput
    {
        public bool IsSuccess { get; set; }
        public UpdateHardwareIdOutputEnum Code { get; set; }
    }
}