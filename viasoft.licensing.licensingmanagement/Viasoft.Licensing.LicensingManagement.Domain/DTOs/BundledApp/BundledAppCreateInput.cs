using System;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp
{
    public class BundledAppCreateInput
    {
        [StrictRequired]
        public Guid BundleId { get; set; }
        
        [StrictRequired]
        public Guid AppId { get; set; }
        
    }
}