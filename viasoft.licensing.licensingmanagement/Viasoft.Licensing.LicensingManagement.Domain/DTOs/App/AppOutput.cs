using System;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.App
{
    public class AppOutput
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }

        public string Identifier { get; set; }

        public bool IsActive { get; set; }

        public Guid SoftwareId { get; set; }
        
        public bool IsDefault { get; set; }
        
        public Enums.Domain Domain { get; set; }
    }
}