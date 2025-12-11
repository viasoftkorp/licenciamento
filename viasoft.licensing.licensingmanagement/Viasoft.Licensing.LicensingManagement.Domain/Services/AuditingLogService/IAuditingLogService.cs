using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.AuditingLogService
{
    public interface IAuditingLogService
    {
        Task<AuditingLog> InsertLogs(AuditingLog newLog);
    }
}