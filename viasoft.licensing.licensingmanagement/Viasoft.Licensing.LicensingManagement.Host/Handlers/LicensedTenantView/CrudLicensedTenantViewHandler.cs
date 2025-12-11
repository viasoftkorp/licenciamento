using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rebus.Handlers;
using Viasoft.Core.AmbientData.Attributes;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.EntityFrameworkCore.Extensions;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Message;
using Viasoft.Licensing.LicensingManagement.Domain.Account.Repository;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.InfrastructureConfiguration.Service;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.Message;

namespace Viasoft.Licensing.LicensingManagement.Host.Handlers.LicensedTenantView
{
    public class CrudLicensedTenantViewHandler : IHandleMessages<LicensedTenantCreatedMessage>,
        IHandleMessages<LicensedTenantUpdatedMessage>, IHandleMessages<DeleteLicensedTenantMessage>,
        IHandleMessages<AccountUpdatedMessage>
    {
        private readonly IRepository<Domain.Entities.LicensedTenantView> _licensedTenantViews;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInfrastructureConfigurationService _infrastructureConfigurationService;

        public CrudLicensedTenantViewHandler(IRepository<Domain.Entities.LicensedTenantView> licensedTenantViews,
            IAccountRepository accountRepository, IUnitOfWork unitOfWork, IInfrastructureConfigurationService infrastructureConfigurationService)
        {
            _licensedTenantViews = licensedTenantViews;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _infrastructureConfigurationService = infrastructureConfigurationService;
        }

        public async Task Handle(LicensedTenantCreatedMessage message)
        {
            if (!await _licensedTenantViews.AnyAsync(l => l.Identifier == message.Identifier))
            {
                var licensedTenantView = new Domain.Entities.LicensedTenantView
                {
                    Id = Guid.NewGuid(),
                    Identifier = message.Identifier,
                    Status = message.Status,
                    AccountId = message.AccountId,
                    AdministratorEmail = message.AdministratorEmail,
                    LicensedCnpjs = message.LicensedCnpjs,
                    LicensedTenantId = message.LicensedTenantId,
                    ExpirationDateTime = message.ExpirationDateTime,
                    AccountCompanyName = _accountRepository.GetAccountNameFromId(message.AccountId),
                    HardwareId = message.HardwareId
                };
                await _licensedTenantViews.InsertAsync(licensedTenantView);
            }
            await _infrastructureConfigurationService.CreateAsync(new InfrastructureConfigurationCreateInput
            {
                LicensedTenantId = message.LicensedTenantId,
                GatewayAddress = null
            });
        }

        //precisamos de UserNotRequired pois o HardwareId pode ser atualizado pelo servidor de licenças, e nesse caso não tem um usuário 
        [UserNotRequired]
        public async Task Handle(LicensedTenantUpdatedMessage message)
        {
            var licensedTenantViewToUpdate = await _licensedTenantViews.FirstOrDefaultAsync(l => l.LicensedTenantId == message.LicensedTenantId);
            if (licensedTenantViewToUpdate != null)
            {
                licensedTenantViewToUpdate.Identifier = message.Identifier;
                licensedTenantViewToUpdate.Status = message.Status;
                licensedTenantViewToUpdate.AccountId = message.AccountId;
                licensedTenantViewToUpdate.AdministratorEmail = message.AdministratorEmail;
                licensedTenantViewToUpdate.LicensedCnpjs = message.LicensedCnpjs;
                licensedTenantViewToUpdate.AccountCompanyName = _accountRepository.GetAccountNameFromId(message.AccountId);
                licensedTenantViewToUpdate.ExpirationDateTime = message.ExpirationDateTime;
                licensedTenantViewToUpdate.HardwareId = message.HardwareId;
                await _licensedTenantViews.UpdateAsync(licensedTenantViewToUpdate);
            }
        }

        public async Task Handle(DeleteLicensedTenantMessage message)
        {
            var licensedTenantViewToDelete = await _licensedTenantViews.FirstOrDefaultAsync(l => l.LicensedTenantId == message.LicensedTenantId);
            if (licensedTenantViewToDelete != null)
            {
                await _licensedTenantViews.DeleteAsync(licensedTenantViewToDelete.Id);
            }
            await _infrastructureConfigurationService.DeleteAsync(message.LicensedTenantId);
        }

        public async Task Handle(AccountUpdatedMessage message)
        {
            using (_unitOfWork.Begin(options => options.LazyTransactionInitiation = false))
            {
                await _licensedTenantViews.BatchUpdateAsync(view => new Domain.Entities.LicensedTenantView{AccountCompanyName = message.CompanyName}, 
                    l => l.AccountId == message.AccountId);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}