using System;
using System.ComponentModel.DataAnnotations.Schema;
using Viasoft.Core.DDD.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota
{
    [Table("FileTenantQuota")]
    public class FileTenantQuota: IEntity
    {
        public Guid Id { get; set; }
        public Guid LicenseTenantId { get; set; }
        public long QuotaLimit { get; set; }
    }
}