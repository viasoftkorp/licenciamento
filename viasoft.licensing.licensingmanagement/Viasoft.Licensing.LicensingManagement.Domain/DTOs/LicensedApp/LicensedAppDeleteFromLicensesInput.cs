using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedApp
{
    public class LicensedAppDeleteFromLicensesInput
    {
        public LicensedAppDeleteFromLicensesInput()
        {
            LicensedTenantsId = new List<Guid>();
        }
        public List<Guid> LicensedTenantsId { get; set; }
        
        public Guid AppId { get; set; }
    }
}