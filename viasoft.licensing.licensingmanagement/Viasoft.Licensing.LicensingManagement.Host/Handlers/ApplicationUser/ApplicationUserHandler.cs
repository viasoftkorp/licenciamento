using System.Threading.Tasks;
using Rebus.Handlers;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.EntityFrameworkCore.Extensions;
using Viasoft.Core.MultiTenancy.DataFilter;
using Viasoft.Data.DataFilter;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Events;

namespace Viasoft.Licensing.LicensingManagement.Host.Handlers.ApplicationUser
{
    public class ApplicationUserHandler: IHandleMessages<ApplicationUserUpdatedEvent>
    {
        private readonly IRepository<NamedUserAppLicense> _namedUserAppLicenses;
        private readonly IRepository<NamedUserBundleLicense> _namedUserBundleLicenses;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataFilterManager _dataFilterManager;

        public ApplicationUserHandler(IRepository<NamedUserAppLicense> namedUserAppLicenses, IRepository<NamedUserBundleLicense> namedUserBundleLicenses, IUnitOfWork unitOfWork, 
            IDataFilterManager dataFilterManager)
        {
            _namedUserAppLicenses = namedUserAppLicenses;
            _namedUserBundleLicenses = namedUserBundleLicenses;
            _unitOfWork = unitOfWork;
            _dataFilterManager = dataFilterManager;
        }

        public async Task Handle(ApplicationUserUpdatedEvent message)
        {
            using (_dataFilterManager.DisableDataFilter<MustHaveTenantDataFilter>())
            {
                using (_unitOfWork.Begin(options => options.LazyTransactionInitiation = false))
                {
                    await _namedUserAppLicenses.BatchUpdateAsync(a => new NamedUserAppLicense {NamedUserEmail = message.Email}, a => a.NamedUserId == message.Id);
                    await _namedUserBundleLicenses.BatchUpdateAsync(b => new NamedUserBundleLicense {NamedUserEmail = message.Email}, b => b.NamedUserId == message.Id);

                    await _unitOfWork.CompleteAsync();
                }
            }
        }
    }
}