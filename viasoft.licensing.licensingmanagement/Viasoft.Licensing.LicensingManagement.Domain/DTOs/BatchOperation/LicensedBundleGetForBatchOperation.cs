using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation
{
    public class LicensedBundleGetForBatchOperation
    {
        public Guid BundleId { get; set; }
        public Guid LicensedTenantId { get; set; }
    }
}