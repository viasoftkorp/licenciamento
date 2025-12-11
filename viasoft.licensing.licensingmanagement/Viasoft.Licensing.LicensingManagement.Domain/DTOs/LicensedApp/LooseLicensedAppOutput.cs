using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp
{
    public class LooseLicensedAppOutput
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public Guid SoftwareId { get; set; }
        
        public string SoftwareName { get; set; }
        
        public LicensedAppStatus Status { get; set; }

        public int NumberOfLicenses { get; set; }

        public int AdditionalNumberOfLicenses { get; set; }
        
        public Enums.Domain Domain { get; set; }
        
        public int NumberOfTemporaryLicenses { get; set; }
        
        public DateTime? ExpirationDateOfTemporaryLicenses { get; set; }
        
        public DateTime? ExpirationDateTime { get; set; }

        public LicensingModels LicensingModel { get; set; }
        
        public LicensingModes? LicensingMode { get; set; }
        
        public Guid LicensedAppId { get; set; }
    }
}