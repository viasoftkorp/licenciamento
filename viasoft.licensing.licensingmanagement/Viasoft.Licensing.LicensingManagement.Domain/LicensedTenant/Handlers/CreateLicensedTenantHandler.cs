using System;
using System.Threading.Tasks;
using Rebus.Handlers;
using Rebus.Retry.Simple;
using Viasoft.Core.DDD.UnitOfWork;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Handlers
{
    public class CreateLicensedTenantHandler: IHandleMessages<CreateLicensedTenantCommand>, IHandleMessages<IFailed<CreateLicensedTenantCommand>>
    {
        private readonly IServiceBus _serviceBus;
        private readonly ILicensedTenantService _licensedTenantService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateLicensedTenantHandler(IServiceBus serviceBus, ILicensedTenantService licensedTenantService, IUnitOfWork unitOfWork)
        {
            _serviceBus = serviceBus;
            _licensedTenantService = licensedTenantService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateLicensedTenantCommand command)
        {
            var createInput = new LicenseTenantCreateInput
            {
                Id = Guid.NewGuid(),
                Identifier = command.LicensingIdentifier,
                Notes = string.Empty,
                Status = LicensingStatus.Active,
                AccountId = command.AccountId,
                AdministratorEmail = command.AdministratorEmail,
                LicensedCnpjs = command.LicensedCnpjs,
                ExpirationDateTime = null,
                LicenseConsumeType = LicenseConsumeType.Access,
                BundleIds = command.BundleIds,
                NumberOfLicenses = command.NumberOfLicenses
            };

            using (_unitOfWork.Begin())
            {
                var result = await _licensedTenantService.CreateNewTenantLicensing(createInput, false);

                await _serviceBus.Reply(new CreateLicensedTenantReply
                {
                    Code = result.OperationValidation,
                    LicensingIdentifier = result.Identifier,
                });

                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task Handle(IFailed<CreateLicensedTenantCommand> message)
        {
            await _serviceBus.Reply(new CreateLicensedTenantReply
            {
                Code = OperationValidation.UnknownError,
                LicensingIdentifier = Guid.Empty,
                Message = message.ErrorDescription,
            });
        }
    }
}