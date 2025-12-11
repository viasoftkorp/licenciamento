using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp
{
    public class BundledLicensedAppOutput
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }

        public string SoftwareName { get; set; }

        public LicensedAppStatus Status { get; set; }

        public Enums.Domain Domain { get; set; }
    }
}