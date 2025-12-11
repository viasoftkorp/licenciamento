using System;
using Viasoft.Data.Attributes;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle
{
    public class UpdateNamedUserProductLicenseInput
    {
        [StrictRequired]
        public Guid NamedUserId { get; set; }
        [StrictRequired]
        public string NamedUserEmail { get; set; }
        [StrictRequired]
        public ProductType ProductType { get; set; }
        public string DeviceId { get; set; }
    }
}