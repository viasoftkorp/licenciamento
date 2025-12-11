using System;
using Viasoft.Core.DDD.Application.Dto.Entities;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Software
{
    public class SoftwareCreateInput : IEntityDto
    {
        [StrictRequired]
        public Guid Id { get; set; }
        
        [StrictRequired]
        public string Name { get; set; }
        
        [StrictRequired]
        public string Identifier { get; set; }
        
        public string Company { get; set; }
        
        [StrictRequired]
        public bool IsActive { get; set; }
    }
}