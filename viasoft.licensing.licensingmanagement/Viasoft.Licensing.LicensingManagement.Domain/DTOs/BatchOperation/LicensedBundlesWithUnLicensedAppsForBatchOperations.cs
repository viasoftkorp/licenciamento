using System;
using System.Collections.Generic;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation
{
    public class LicensedBundlesWithUnLicensedAppsForBatchOperations
    {
        public Guid BundleId { get; set; }
        
        public Guid LicensedTenantId { get; set; }
        
        public int NumberOfLicenses { get; set; }
        
        public LicensedBundleStatus Status { get; set; }
        
        public List<AppsGetForBatchOperations> AppsGetForBatchOperations { get; set; }
    }
}