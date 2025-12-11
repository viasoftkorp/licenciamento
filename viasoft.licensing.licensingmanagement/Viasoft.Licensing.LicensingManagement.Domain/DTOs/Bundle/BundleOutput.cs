using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle
{
    public class BundleOutput
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Identifier { get; set; }

        public bool IsActive { get; set; }
        
        public bool IsCustom { get; set; }
        
        public Guid SoftwareId { get; set; }
    }
}