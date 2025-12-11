using System;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp
{
    public class LicensedAppDeleteInput
    {
        [StrictRequired]
        public Guid LicensedTenantId { get; set; }
        
        [StrictRequired]
        public Guid AppId { get; set; }
    }
}