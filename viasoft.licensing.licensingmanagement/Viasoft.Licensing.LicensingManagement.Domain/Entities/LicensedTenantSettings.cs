using System;
using Viasoft.Core.DDD.Entities.Auditing;

namespace Viasoft.Licensing.LicensingManagement.Domain.Entities
{
    public class LicensedTenantSettings : FullAuditedEntity
    {
        public Guid TenantId { get; set; }
        public Guid LicensingIdentifier { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public void UpdateValue(bool useSimpleHardwareId)
        {
            Value = useSimpleHardwareId.ToString();
        }
    }
}