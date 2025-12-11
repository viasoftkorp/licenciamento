using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation
{
    public class LicensedBundleApps
    {
        public Guid BundleId { get; set; }
        
        public List<Guid> AppIds { get; set; }
    }
}