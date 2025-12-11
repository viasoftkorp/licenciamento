using System;
using System.ComponentModel.DataAnnotations.Schema;
using Viasoft.Core.DDD.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota
{
    [Table("FileAppQuota")]
    public class FileAppQuotaView: IEntity
    {
        public Guid Id { get; set; }
        public Guid LicensedTenantId { get; set; }
        public Guid AppId { get; set; }
        public string AppName { get; set; }
        public long QuotaLimit { get; set; }
    }
}