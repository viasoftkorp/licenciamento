using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant
{
    public class LicensedAppDetailsTemporaryLicenses
    {
        public Guid? LicensedBundleId { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public Guid AppId { get; set; }
        public LicensedAppStatus Status { get; set; }
        public int NumberOfLicenses { get; set; }
        public int AdditionalNumberOfLicenses { get; set; }
        public int NumberOfTemporaryLicenses { get; set; }
        public DateTime? ExpirationDateOfTemporaryLicenses { get; set; }
        public string SoftwareIdentifier { get; set; }
        public string SoftwareName { get; set; }
        public LicensingModels LicensingModel { get; set; }
        public LicensingModes? LicensingMode { get; set; }
    }
}