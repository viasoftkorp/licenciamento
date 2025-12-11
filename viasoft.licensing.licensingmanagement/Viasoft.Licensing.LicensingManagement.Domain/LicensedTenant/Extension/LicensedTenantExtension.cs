using System;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Extension
{
    public static class LicensedTenantExtension
    {
        public static bool HasAdministratorEmailChanged(this Entities.LicensedTenant licensedTenant, string newAdministratorEmail)
        {
            return !string.Equals(licensedTenant.AdministratorEmail, newAdministratorEmail, StringComparison.OrdinalIgnoreCase);
        }

        public static bool HasLicensedCnpjsChanged(this Entities.LicensedTenant licensedTenant, string newLicensedCnpjs)
        {
            return !string.Equals(licensedTenant.LicensedCnpjs, newLicensedCnpjs, StringComparison.OrdinalIgnoreCase);
        }

        public static bool HasLicensingStatusChanged(this Entities.LicensedTenant licensedTenant, LicensingStatus newLicensingStatus)
        {
            return licensedTenant.Status != newLicensingStatus;
        }
    }
}