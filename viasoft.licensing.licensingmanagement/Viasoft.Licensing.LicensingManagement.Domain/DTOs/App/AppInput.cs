using System;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.App
{
    public class AppInput
    {
        [StrictRequired]
        public Guid Id { get; set; }
        
        [StrictRequired]
        public string Name { get; set; }

        [StrictRequired]
        public string Identifier { get; set; }

        [StrictRequired]
        public bool IsActive { get; set; }

        [StrictRequired]
        public Guid SoftwareId { get; set; }
        
        [StrictRequired]
        public bool IsDefault { get; set; }
    }
}