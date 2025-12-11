using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Product
{
    public class ProductOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public bool IsActive { get; set; }
        public Guid SoftwareId { get; set; }
        public string SoftwareName { get; set; }
        public int NumberOfLicenses { get; set; }
        public LicensingModels LicensingModel { get; set; }
        public LicensingModes? LicensingMode { get; set; }
        public ProductStatus Status { get; set; }
        public int NumberOfUsedLicenses { get; set; }
        public ProductType ProductType { get; set; }
        public Guid ProductId { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
    }
}