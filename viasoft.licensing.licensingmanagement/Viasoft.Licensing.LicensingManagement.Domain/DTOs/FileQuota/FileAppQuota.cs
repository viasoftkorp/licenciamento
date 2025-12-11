using System;
using Viasoft.Core.DDD.Application.Dto.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.FileQuota
{
    public class FileAppQuota: IEntityDto
    {
        public Guid Id { get; set; }
        
        public Guid TenantId { get; set; }
        
        public string AppId { get; set; }
        
        public long QuotaLimit { get; set; }
    }
}