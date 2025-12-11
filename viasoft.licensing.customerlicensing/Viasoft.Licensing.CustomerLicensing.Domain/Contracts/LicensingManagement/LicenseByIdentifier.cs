using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Contracts.LicensingManagement
{
    public class LicenseByIdentifier
    {
        public Guid Identifier { get; set; }
        public LicensingStatus Status { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public List<string> Cnpjs { get; set; }
        public LicensedTenantDetails LicensedTenantDetails { get; set; }
    }
}