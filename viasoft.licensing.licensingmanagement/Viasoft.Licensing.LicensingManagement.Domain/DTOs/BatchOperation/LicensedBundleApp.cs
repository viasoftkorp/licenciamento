using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation
{
    public class LicensedBundleApp
    {
        public Guid BundleId { get; set; }
        
        public Guid AppId { get; set; }
    }
}