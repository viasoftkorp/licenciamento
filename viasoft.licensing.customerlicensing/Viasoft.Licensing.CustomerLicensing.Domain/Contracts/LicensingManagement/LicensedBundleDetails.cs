using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Contracts.LicensingManagement
{
    public class LicensedBundleDetails
    {
        public Guid BundleId { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public LicensedBundleStatus Status { get; set; }
        public int NumberOfLicenses { get; set; }
    }
}