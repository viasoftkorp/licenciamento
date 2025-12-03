using System;
using System.Collections.Generic;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.CustomerLicensing;

namespace Viasoft.Licensing.LicenseServer.Domain.DataUploader.Models
{
    public class LicenseUsageInRealTimeRawData
    {
        public Guid TenantId { get; set; }
        public List<LicenseUsageInRealTimeDetails> LicenseUsageInRealTimeDetails { get; set; }
        public List<string> SoftwareUtilized { get; set; }
    }
}