using System;
using System.Collections.Generic;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant
{
    public class LicenseByIdentifier
    {
        public Guid Identifier { get; set; }
        public LicenseConsumeType LicenseConsumeType { get; set; }
        public LicensingStatus Status { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public List<string> Cnpjs { get; set; }
        public string HardwareId { get; set; }
        public LicensedTenantDetails LicensedTenantDetails { get; set; }
    }
}