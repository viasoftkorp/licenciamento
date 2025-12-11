using System.Threading.Tasks;
using Rebus.Handlers;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.EntityFrameworkCore.Extensions;
using Viasoft.Licensing.CustomerLicensing.Domain.Entities;
using Viasoft.Licensing.CustomerLicensing.Domain.Messages;

namespace Viasoft.Licensing.CustomerLicensing.Host.Handlers
{
    public class AccountUpdatedHandler : IHandleMessages<AccountUpdatedMessage>
    {
        private readonly IRepository<LicenseUsageInRealTime> _usageInRealTime;
        private readonly IUnitOfWork _unitOfWork;

        public AccountUpdatedHandler(IRepository<LicenseUsageInRealTime> usageInRealTime, IUnitOfWork unitOfWork)
        {
            _usageInRealTime = usageInRealTime;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AccountUpdatedMessage message)
        {
            using (_unitOfWork.Begin(opt => opt.LazyTransactionInitiation = false))
            {
                await _usageInRealTime.BatchUpdateAsync(
                    u => new LicenseUsageInRealTime {AccountName = message.CompanyName},
                    p => p.AccountId == message.AccountId);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}