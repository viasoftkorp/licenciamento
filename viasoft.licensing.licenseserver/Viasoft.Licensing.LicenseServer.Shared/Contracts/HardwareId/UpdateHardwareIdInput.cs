using System;

namespace Viasoft.Licensing.LicenseServer.Shared.Contracts.HardwareId
{
    public class UpdateHardwareIdInput
    {
        public Guid TenantId { get; set; }
        public string HardwareId { get; set; }
    }
}