using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle
{
    public class LicensedBundleOutput
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Identifier { get; set; }

        public bool IsActive { get; set; }
        
        public bool IsCustom { get; set; }
        
        public Guid SoftwareId { get; set; }
        
        public string SoftwareName { get; set; }
        
        public int NumberOfLicenses { get; set; }
        
        public int NumberOfTemporaryLicenses { get; set; }
        
        public DateTime? ExpirationDateOfTemporaryLicenses { get; set; }

        public LicensingModels LicensingModel { get; set; }
        
        public LicensingModes? LicensingMode { get; set; }
        
        public Guid LicensedBundleId { get; set; }
        
        public LicensedBundleStatus Status { get; set; }
        
        public int NumberOfUsedLicenses { get; set; }

    }
}