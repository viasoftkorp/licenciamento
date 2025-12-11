using System;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle
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
        
    }
}