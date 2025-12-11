using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensingLicenses
{
    public class UpdateHardwareIdOutput
    {
        public bool IsSuccess { get; set; }
        public UpdateHardwareIdEnum Code { get; set; }
    }
}