using System.Threading.Tasks;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.AuditingLogService
{
    public class AuditingLogService: IAuditingLogService, ITransientDependency
    {
        private readonly IRepository<AuditingLog> _auditingLog;
        
        public AuditingLogService(IRepository<AuditingLog> auditingLog)
        {
            _auditingLog = auditingLog;
        }
        
        public async Task<AuditingLog> InsertLogs(AuditingLog newLog)
        {
            return await _auditingLog.InsertAsync(newLog, true);
        }
    }
}