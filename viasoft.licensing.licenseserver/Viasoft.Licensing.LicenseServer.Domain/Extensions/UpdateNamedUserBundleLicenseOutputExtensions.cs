using System;
using Viasoft.Licensing.LicenseServer.Domain.Classes.LicenseTenantStatus;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.NamedUserBundleLicense;
using Viasoft.Licensing.LicenseServer.Domain.Enums;

namespace Viasoft.Licensing.LicenseServer.Domain.Extensions
{
    public static class UpdateNamedUserBundleLicenseOutputExtensions
    {
        public static TryConsumeLicenseOutput ToConsumeLicenseOutput(this UpdateNamedUserBundleLicenseOutput updateNamedUserBundleLicenseOutput, LicenseTenantStatusApp statusApp)
        {
            switch (updateNamedUserBundleLicenseOutput.ValidationCode)
            {
                case UpdateNamedUserBundleLicenseValidationCode.NoLicensedTenant:
                    return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.CouldNotUpdateNamedUserBundleLicenseDueToNoLicensedTenant, statusApp.AppName, statusApp.SoftwareIdentifier, statusApp.SoftwareName);
                case UpdateNamedUserBundleLicenseValidationCode.NoLicensedBundle:
                    return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.CouldNotUpdateNamedUserBundleLicenseDueToNoLicensedBundle, statusApp.AppName, statusApp.SoftwareIdentifier, statusApp.SoftwareName);
                case UpdateNamedUserBundleLicenseValidationCode.NoNamedUser:
                    return new TryConsumeLicenseOutput(ConsumeAppLicenseStatus.CouldNotUpdateNamedUserBundleLicenseDueToNoNamedUser, statusApp.AppName, statusApp.SoftwareIdentifier, statusApp.SoftwareName);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}