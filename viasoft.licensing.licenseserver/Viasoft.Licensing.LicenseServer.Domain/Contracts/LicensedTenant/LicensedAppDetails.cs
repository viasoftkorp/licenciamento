using System;
using Viasoft.Licensing.LicenseServer.Domain.Enums;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant
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
        public LicensingModels LicensingModel { get; set; }
        public LicensingModes? LicensingMode { get; set; }
        public Guid LicensedAppId { get; set; }
    }
}