using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle
{
    public class BundleCreateOutput
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Identifier { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool IsCustom { get; set; }
        
        public Guid SoftwareId { get; set; }
        
        public string SoftwareName { get; set; }
        
        public OperationValidation OperationValidation { get; set; }

        public string OperationValidationDescription => OperationValidation.ToString();
    }
}