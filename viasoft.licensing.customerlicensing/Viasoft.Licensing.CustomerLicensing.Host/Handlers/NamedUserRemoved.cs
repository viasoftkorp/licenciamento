using System.Linq;
using System.Threading.Tasks;
using Rebus.Handlers;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Licensing.CustomerLicensing.Domain.Entities;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;
using Viasoft.Licensing.CustomerLicensing.Domain.Events;
using Z.EntityFramework.Plus;

namespace Viasoft.Licensing.CustomerLicensing.Host.Handlers
{
    public class NamedUserRemoved : IHandleMessages<NamedUserRemovedFromProduct>
    {
        private readonly IRepository<LicenseUsageInRealTime> _usageInRealTime;
        private readonly IUnitOfWork _unitOfWork;

        public NamedUserRemoved(IRepository<LicenseUsageInRealTime> usageInRealTime, IUnitOfWork unitOfWork)
        {
            _usageInRealTime = usageInRealTime;
            _unitOfWork = unitOfWork;
        }
        
        public async Task Handle(NamedUserRemovedFromProduct message)
        {
            using (_unitOfWork.Begin(opt => opt.LazyTransactionInitiation = false))
            {
                await _usageInRealTime.Where(e =>
                        e.LicensingIdentifier == message.LicensingIdentifier 
                        && e.User == message.NamedUserEmail 
                        && e.LicensingModel == LicensingModels.Named && e.LicensingMode == LicensingModes.Offline)
                    .DeleteAsync();
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}