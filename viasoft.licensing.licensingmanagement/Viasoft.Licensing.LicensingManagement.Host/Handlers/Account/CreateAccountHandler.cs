using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rebus.Handlers;
using Rebus.Retry.Simple;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.MultiTenancy.Abstractions.Tenant;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Message;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Host.Handlers.Account
{
    public class CreateAccountHandler: IHandleMessages<CreateAccountCommand>, IHandleMessages<IFailed<CreateAccountCommand>>
    {
        private readonly IServiceBus _serviceBus;
        private readonly IRepository<Domain.Entities.Account> _accounts;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentTenant _currentTenant;

        public CreateAccountHandler(IServiceBus serviceBus, IRepository<Domain.Entities.Account> accounts, IUnitOfWork unitOfWork, ICurrentTenant currentTenant)
        {
            _serviceBus = serviceBus;
            _accounts = accounts;
            _unitOfWork = unitOfWork;
            _currentTenant = currentTenant;
        }

        public async Task Handle(CreateAccountCommand command)
        {
            var query = await _accounts
                .Select(a => a.CnpjCpf)
                .AnyAsync(a => a == command.Cnpj);

            if (query)
            {
                await _serviceBus.Reply(new CreateAccountReply
                {
                    Code = CreateAccountEnum.DuplicatedCnpj,
                    Message = $"Cnpj: {command.Cnpj} is already being used by a account",
                    Id = command.Id
                });
                return;
            }

            var entity = new Domain.Entities.Account
            {
                City = command.City,
                Country = command.Country,
                Detail = command.Detail,
                Email = command.Email,
                Neighborhood = command.Neighborhood,
                Id = command.Id,
                Number = command.Number,
                Phone = command.Phone,
                State = command.State,
                Status = AccountStatus.Active,
                Street = command.Street,
                BillingEmail = command.BillingEmail,
                CnpjCpf = command.Cnpj,
                CompanyName = command.CompanyName,
                TenantId = _currentTenant.Id,
                ZipCode = command.ZipCode,
                WebSite = command.WebSite,
                TradingName = command.TradingName
            };

            using (_unitOfWork.Begin())
            {
                await _accounts.InsertAsync(entity);

                await _serviceBus.Reply(new CreateAccountReply
                {
                    Code = CreateAccountEnum.AccountCreated,
                    Id = command.Id,
                });

                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task Handle(IFailed<CreateAccountCommand> message)
        {
            await _serviceBus.Reply(new CreateAccountReply
            {
                Code = CreateAccountEnum.UnknownError,
                Message = message.ErrorDescription,
                Id = message.Message.Id
            });
        }
    }
}