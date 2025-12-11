using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Software
{
    public class SoftwareUpdateOutput
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Identifier { get; set; }

        public bool IsActive { get; set; }
        
        public OperationValidation OperationValidation { get; set; }

        public string OperationValidationDescription => OperationValidation.ToString();
    }
}