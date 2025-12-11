using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rebus.Handlers;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Licensing.LicensingManagement.Domain.AppMessages;
using Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota;

namespace Viasoft.Licensing.LicensingManagement.Host.Handlers.FileQuota
{
    public class FileQuotaViewHandler: IHandleMessages<AppUpdatedMessage>
    {
        private readonly IRepository<FileAppQuotaView> _appQuotaViewRepository;

        public FileQuotaViewHandler(IRepository<FileAppQuotaView> appQuotaViewRepository)
        {
            _appQuotaViewRepository = appQuotaViewRepository;
        }

        public async Task Handle(AppUpdatedMessage message)
        {
            var quotaToUpdate = await _appQuotaViewRepository
                    .FirstOrDefaultAsync(q => q.AppId == message.Id);
            if (quotaToUpdate == null || quotaToUpdate.AppName == message.Name)
                return;
            
            quotaToUpdate.AppName = message.Name;
            await _appQuotaViewRepository.UpdateAsync(quotaToUpdate, true);
        }
    }
}