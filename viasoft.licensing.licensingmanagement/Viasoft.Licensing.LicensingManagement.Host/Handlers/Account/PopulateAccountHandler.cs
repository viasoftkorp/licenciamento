using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Rebus.Handlers;
using Rebus.Retry.Simple;
using Viasoft.Core.ApiClient;
using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Licensing.LicensingManagement.Domain.Account.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Message;
using Viasoft.PushNotifications.Abstractions.Contracts;
using Viasoft.PushNotifications.Abstractions.Notification;

namespace Viasoft.Licensing.LicensingManagement.Host.Handlers.Account
{
    public class PopulateAccountHandler : IHandleMessages<PopulateAccountMessage>, IHandleMessages<IFailed<PopulateAccountMessage>>
    {
        private readonly IApiClientCallBuilder _apiClientCallBuilder;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Domain.Entities.Account> _accounts;
        private readonly IPushNotification _pushNotification;

        public PopulateAccountHandler(IApiClientCallBuilder apiClientCallBuilder, IUnitOfWork unitOfWork, 
            IRepository<Domain.Entities.Account> accounts, IPushNotification pushNotification)
        {
            _apiClientCallBuilder = apiClientCallBuilder;
            _unitOfWork = unitOfWork;
            _accounts = accounts;
            _pushNotification = pushNotification;
        }

        public async Task Handle(PopulateAccountMessage message)
        {
            var skipCount = 0;
            var insertedAccounts = 0;
            while (true)
            {
                var gatewayCall = _apiClientCallBuilder.WithEndpoint("Sales/CRM/Accounts/Account/NoAuthorizationGetAll?skipCount=" + skipCount + "&MaxResultCount=100")
                .WithServiceName("Viasoft.Sales.CRM.Accounts")
                .WithHttpMethod(HttpMethod.Get)
                .Build();

                var result = await gatewayCall.ResponseCallAsync<PagedResultDto<CrmAccountOutput>>();

                if (result.Items.Count == 0) break;

                using (_unitOfWork.Begin())
                {
                    foreach (var account in result.Items)
                    {
                        var replacedCnpjCpf = account.NormalizedCnpjCpf;
                        if (account.Status == CrmAccountStatus.Active && !_accounts.Any(l => l.CnpjCpf == replacedCnpjCpf))
                        {
                            var newLicensingAccount = new Domain.Entities.Account
                            {
                                City = account.MainAddress.City,
                                Detail = account.MainAddress.Detail,
                                Email = account.Email,
                                Id = new Guid(),
                                Neighborhood = account.MainAddress.Neighborhood,
                                Number = account.MainAddress.Number,
                                Phone = account.Phone,
                                State = account.MainAddress.State,
                                Status = AccountStatus.Active,
                                Street = account.MainAddress.Street,
                                BillingEmail = account.BillingEmail,
                                CnpjCpf = replacedCnpjCpf,
                                CompanyName = account.CompanyName,
                                TradingName = account.TradingName,
                                WebSite = account.WebSite,
                                ZipCode = account.NormalizedZipCode
                            };
                            await _accounts.InsertAsync(newLicensingAccount);
                            insertedAccounts ++;
                        }
                    }

                    await _unitOfWork.CompleteAsync();
                    skipCount += 100;
                }
            }

            await _pushNotification.SendAsync(new Payload
            {
                Body = insertedAccounts + " Contas foram inseridas",
                Header = "Sincronização com CRM foi concluída"
            });
        }

        public async Task Handle(IFailed<PopulateAccountMessage> message)
        {
            await _pushNotification.SendAsync(new Payload
            {
                Body = "Detalhes do erro: " + Environment.NewLine + message.ErrorDescription,
                Header = "Erro ao sincronizar contas com CRM"
            });
        }
    }
}