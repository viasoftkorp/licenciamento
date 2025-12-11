using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.App
{
    public class AppCreateOutput
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Identifier { get; set; }
        
        public string SoftwareName { get; set; }
        
        public Guid SoftwareId { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool IsDefault { get; set; }
        
        public Enums.Domain Domain { get; set; }
        
        public OperationValidation OperationValidation { get; set; }

        public string OperationValidationDescription => OperationValidation.ToString();
    }
}