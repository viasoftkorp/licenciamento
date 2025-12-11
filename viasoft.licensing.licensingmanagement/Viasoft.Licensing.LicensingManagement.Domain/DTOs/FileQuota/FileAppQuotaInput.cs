using System;
using Viasoft.Core.DDD.Application.Dto.Entities;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.FileQuota
{
    public class FileAppQuotaInput: IEntityDto
    {
        public Guid Id { get; set; }
        
        public Guid LicensedTenantId { get; set; }
        
        [StrictRequired]
        public Guid AppId { get; set; }
        
        [StrictRequired]
        public long QuotaLimit { get; set; }
    }
}