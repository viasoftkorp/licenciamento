using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense
{
    public class NamedUserBundleLicenseOutput
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid LicensedTenantId { get; set; }
        public Guid LicensedBundleId { get; set; }
        public Guid NamedUserId { get; set; }
        public string NamedUserEmail { get; set; }
        public string DeviceId { get; set; }
        public OperationValidation OperationValidation { get; set; }

        public static NamedUserBundleLicenseOutput ConstructFromEntity(Entities.NamedUserBundleLicense namedUserBundleLicense)
        {
            return new()
            {
                Id = namedUserBundleLicense.Id,
                DeviceId = namedUserBundleLicense.DeviceId,
                TenantId = namedUserBundleLicense.TenantId,
                LicensedBundleId = namedUserBundleLicense.LicensedBundleId,
                LicensedTenantId = namedUserBundleLicense.LicensedTenantId,
                NamedUserEmail = namedUserBundleLicense.NamedUserEmail,
                NamedUserId = namedUserBundleLicense.NamedUserId,
                OperationValidation = OperationValidation.NoError
            };
        }
    }
}