using System;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserAppLicense;
using Viasoft.Licensing.LicenseServer.Domain.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.Extensions
{
    public static class UpdateNamedUserAppLicenseOutputExtensions
    {
        public static TryConsumeLicenseOutput ToConsumeLicenseOutput(this UpdateNamedUserAppLicenseOutput updateNamedUserAppResult, LicenseTenantStatusApp statusApp)
        {
            switch (updateNamedUserAppResult.ValidationCode)
            {
                case UpdateNamedUserAppLicenseValidationCode.NoLicensedTenant:
                    return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.CouldNotUpdateNamedUserAppLicenseDueToNoLicensedTenant, statusApp.AppName, statusApp.SoftwareIdentifier, statusApp.SoftwareName);
                case UpdateNamedUserAppLicenseValidationCode.NoLicensedApp:
                    return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.CouldNotUpdateNamedUserAppLicenseDueToNoLicensedApp, statusApp.AppName, statusApp.SoftwareIdentifier, statusApp.SoftwareName);
                case UpdateNamedUserAppLicenseValidationCode.NoNamedUser:
                    return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.CouldNotUpdateNamedUserAppLicenseDueToNoNamedUser, statusApp.AppName, statusApp.SoftwareIdentifier, statusApp.SoftwareName);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}