using System;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.AuditingLog
{
    public class AuditingLogOutput
    {
        public string UserName { get; set; }
        
        public Guid UserId { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public string Details { get; set; }
        
        public string ActionName { get; set; }
        
        public LogAction Action { get; set; }
        
        public LogType Type { get; set; }

        public Guid TenantId { get; set; }
    }
}