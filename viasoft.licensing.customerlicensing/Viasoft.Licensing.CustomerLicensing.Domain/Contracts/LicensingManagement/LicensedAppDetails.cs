using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Contracts.LicensingManagement
{
    public class LicensedAppDetails
    {
        public Guid? LicensedBundleId { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public Guid AppId { get; set; }
        public LicensedAppStatus Status { get; set; }
        public int NumberOfLicenses { get; set; }
        public int AdditionalNumberOfLicenses { get; set; }
        public string SoftwareIdentifier { get; set; }
        public string SoftwareName { get; set; }
    }
}