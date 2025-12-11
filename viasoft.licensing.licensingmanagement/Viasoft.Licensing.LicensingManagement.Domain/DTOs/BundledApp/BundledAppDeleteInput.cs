using System;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.BundledApp
{
    public class BundledAppDeleteInput
    {
        [StrictRequired]
        public Guid BundleId { get; set; }
        
        [StrictRequired]
        public Guid AppId { get; set; }
        
    }
}