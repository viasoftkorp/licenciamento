using System;
using Viasoft.Core.DDD.Application.Dto.Entities;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.FileQuota
{
    public class FileTenantQuotaInput: IEntityDto
    {
        public Guid Id { get; set; }
        
        public Guid LicenseTenantId { get; set; }
        
        [StrictRequired]
        public long QuotaLimit { get; set; }
    }
}