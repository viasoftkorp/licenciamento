using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant
{
    public class LicensedBundleDetailsTemporaryLicenses
    {
        public Guid BundleId { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public LicensedBundleStatus Status { get; set; }
        public int NumberOfLicenses { get; set; }
        public int NumberOfTemporaryLicenses { get; set; }
        public DateTime? ExpirationDateOfTemporaryLicenses { get; set; }
        public LicensingModels LicensingModel { get; set; }
        public LicensingModes? LicensingMode { get; set; }
    }
}