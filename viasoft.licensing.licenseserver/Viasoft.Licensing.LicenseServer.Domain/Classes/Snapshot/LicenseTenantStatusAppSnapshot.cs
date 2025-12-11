using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Domain.Abstractions.NamedUserLicense;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.Enums;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Classes.Snapshot
{
    public class LicenseTenantStatusAppSnapshot
    {
        public string AppIdentifier { get; }
        public string AppName { get; }
        public int AppLicenses { get; }
        public int AppLicensesConsumed { get;  }
        public LicensedAppStatus Status { get; }
        public string StatusDescription => Status.ToString();        
        public Dictionary<string, List<AppLicenseConsumer>> AppLicenseConsumers { get; }
        public LicensingModels LicensingModel { get; }
        public LicensingModes? LicensingMode { get; }
        public List<INamedUserLicense> NamedUserLicenses { get; }
        public string SoftwareIdentifier { get; }
        public string SoftwareName { get; }
        
        
        public LicenseTenantStatusAppSnapshot()
        {
        }

        public LicenseTenantStatusAppSnapshot(LicenseTenantStatusApp app)
        {
            AppIdentifier = app.AppIdentifier;
            AppName = app.AppName;
            AppLicenses = app.AppLicenses;
            AppLicensesConsumed = app.AppLicensesConsumed;
            Status = app.Status;
            AppLicenseConsumers = app.AppLicenseConsumers;
            LicensingModel = app.LicensingModel;
            LicensingMode = app.LicensingMode;
            NamedUserLicenses = app.NamedUserLicenses;
            SoftwareIdentifier = app.SoftwareIdentifier;
            SoftwareName = app.SoftwareName;
        }
    }
}