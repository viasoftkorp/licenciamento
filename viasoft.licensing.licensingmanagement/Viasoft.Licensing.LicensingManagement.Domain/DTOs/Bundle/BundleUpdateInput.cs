using System;
using Viasoft.Core.DDD.Application.Dto.Entities;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Bundle
{
    public class BundleUpdateInput : IEntityDto
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
        public bool IsCustom { get; set; }
        
    }
}