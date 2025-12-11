using System;
using System.Collections.Generic;
using System.Linq;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.TenantLicensing;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Contracts.LicensedTenant
{
    public class LicenseByTenantId
    {
        public Guid Identifier { get; set; }
        public LicensingStatus Status { get; set; }        
        public LicenseConsumeType LicenseConsumeType { get; set; }
        public string StatusDescription => Status.ToString();
        public DateTime? ExpirationDateTime { get; set; }
        public List<string> Cnpjs { get; set; }
        public LicensedTenantDetails LicensedTenantDetails { get; set; }
        public string HardwareId { get; set; }

        public LicenseByTenantId()
        {
        }

        public LicenseByTenantId(TenantLicenses tenantLicenses)
        {
            Identifier = tenantLicenses.LicensedTenant.Identifier;
            Cnpjs = tenantLicenses.LicensedTenant.LicensedCnpjs.Split(',').ToList();
            Status = tenantLicenses.LicensedTenant.Status;
            HardwareId = tenantLicenses.LicensedTenant.HardwareId;
            ExpirationDateTime = tenantLicenses.LicensedTenant.ExpirationDateTime;
            LicenseConsumeType = tenantLicenses.LicensedTenant.LicenseConsumeType;
            LicensedTenantDetails = new LicensedTenantDetails
            {
                AccountDetails = tenantLicenses.AccountDetails,
                OwnedApps = tenantLicenses.OwnedApps,
                OwnedBundles = tenantLicenses.OwnedBundles,
                NamedUserAppLicenses = tenantLicenses.NamedUserAppLicenses,
                NamedUserBundleLicenses = tenantLicenses.NamedUserBundleLicenses,
                LicensedTenantSettings = tenantLicenses.LicensedTenantSettings
            };
        }
    }
}