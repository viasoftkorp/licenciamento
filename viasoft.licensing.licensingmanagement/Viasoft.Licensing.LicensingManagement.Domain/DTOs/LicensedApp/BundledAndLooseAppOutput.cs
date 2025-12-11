using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp
{
    public class BundledAndLooseAppOutput
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public Guid SoftwareId { get; set; }
        
        public LicensedAppStatus Status { get; set; }

        public int NumberOfLicenses { get; set; }

        public int AdditionalNumberOfLicenses { get; set; }
    }
}