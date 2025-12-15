using System;
using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails
{
    public class TenantLicenseDetailsOutput
    {
        public Guid Identifier { get; set; }
        public LicensingStatus Status { get; set; }
        
        public string StatusDescription => Status.ToString();
        
        public DateTime? ExpirationDateTime { get; set; }
        
        public List<string> Cnpjs { get; set; }
    }
}