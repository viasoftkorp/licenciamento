using System;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Product
{
    public class GetProductByIdInput
    {
        public ProductType ProductType { get; set; }
        public Guid LicensingIdentifier { get; set; }
    }
}