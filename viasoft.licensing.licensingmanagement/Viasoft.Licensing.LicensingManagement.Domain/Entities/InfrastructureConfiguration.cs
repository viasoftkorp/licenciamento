using System;
using Viasoft.Core.DDD.Entities.Auditing;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities
{
    public class InfrastructureConfiguration: FullAuditedEntity
    {
        public Guid LicensedTenantId { get; set; }
        public string GatewayAddress { get; set; }
        public string DesktopDatabaseName { get; set; }
    }
}