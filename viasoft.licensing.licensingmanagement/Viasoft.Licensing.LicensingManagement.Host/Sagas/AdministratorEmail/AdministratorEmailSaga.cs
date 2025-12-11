using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using Rebus.Sagas;
using Rebus.Sagas.Idempotent;
using Viasoft.Core.ApiClient.Exceptions;
using Viasoft.Core.DateTimeProvider;
using Viasoft.Core.DDD.Repositories;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSaga;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Service;
using Viasoft.Licensing.LicensingManagement.Domain.Services.Authentication;
using Viasoft.Licensing.LicensingManagement.Host.Extensions;
using Viasoft.Licensing.LicensingManagement.Host.Sagas.AdministratorEmail.Contracts;
using Viasoft.Licensing.LicensingManagement.Host.Sagas.AdministratorEmail.Data;
using Viasoft.PushNotifications.Abstractions.Notification;

namespace Viasoft.Licensing.LicensingManagement.Host.Sagas.AdministratorEmail;

public class AdministratorEmailSaga : IdempotentSaga<AdministratorEmailSagaData>, IAmInitiatedBy<UpdateAdministratorEmailMessage>, 
    IAmInitiatedBy<CreateNewLicensingMessage>, IHandleMessages<RemoveRootUserReply>, IHandleMessages<AddRootUserToTenantReply>,
    IHandleMessages<CreateUserReply>
{
    private readonly IServiceBus _serviceBus;
    private readonly IAuthenticationCaller _authenticationCaller;
    private readonly IRepository<LicensedTenant> _licensedTenants;
    private readonly IPushNotification _notification;
    private readonly ILogger<AdministratorEmailSaga> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILicensedTenantService _licensedTenantService;

    public AdministratorEmailSaga(IServiceBus serviceBus, IAuthenticationCaller authenticationCaller, IRepository<LicensedTenant> licensedTenants, 
        IPushNotification notification, ILogger<AdministratorEmailSaga> logger, IDateTimeProvider dateTimeProvider, ILicensedTenantService licensedTenantService)
    { 
        _serviceBus = serviceBus;
        _authenticationCaller = authenticationCaller;
        _licensedTenants = licensedTenants;
        _notification = notification;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _licensedTenantService = licensedTenantService;
    }

    protected override void CorrelateMessages(ICorrelationConfig<AdministratorEmailSagaData> config)
    {
        config.Correlate<UpdateAdministratorEmailMessage>(m => m.TenantId,
            i => i.SagaId);
        config.Correlate<CreateNewLicensingMessage>(m => m.TenantId,
            i => i.SagaId);
        config.Correlate<CreateUserReply>(m => m.TenantId,
            i => i.SagaId);
        config.Correlate<RemoveRootUserReply>(m => m.TenantId,
            i => i.SagaId);
        config.Correlate<AddRootUserToTenantReply>(m => m.TenantId,
            i => i.SagaId);
    }

    public async Task Handle(UpdateAdministratorEmailMessage message)
    {
        await SetInitialData(message.TenantId, message.OldEmail, message.NewEmail,false);

        await _serviceBus.Send(new CreateUserIfNotExistsCommand
        {
            Email = message.NewEmail,
            UserId = null,
            TenantId = message.TenantId
        });
    }

    public async Task Handle(CreateNewLicensingMessage message)
    {
        await SetInitialData(message.TenantId, null, message.Email,true);

        await _serviceBus.Send(new CreateUserIfNotExistsCommand
        {
            Email = message.Email,
            UserId = message.AdministratorUserId,
            TenantId = message.TenantId
        });
    }
    
    public async Task Handle(RemoveRootUserReply message)
    {
        if (message.Success)
        {
            await _serviceBus.Send(new AddRootUserToTenantCommand
            {
                UserId = Data.NewUserId!.Value,
                TenantId = Data.TenantId
            });
        }
        else
        {
            await MarkAsCompleted(CurrentSagaStatus.CompletedWithFailure, new SagaErrorDetails
            {
                WhereHappened = "RemoveRootUserReply Handle",
                Description = "The reply message from authorization AddRootUserToTenantCommand indicates failure.",
                Exception = message.ErrorDescription
            });
        }
    }

    public async Task Handle(AddRootUserToTenantReply message)
    {
        if (message.Success)
        {
            await MarkAsCompleted(CurrentSagaStatus.CompletedSuccessfully, null);
        }
        else
        {
            await MarkAsCompleted(CurrentSagaStatus.CompletedWithFailure, new SagaErrorDetails
            {
                WhereHappened = "AddRootUserToTenantReply Handle",
                Description = "The reply message from authorization RemoveRootUser indicates failure",
                Exception = message.ErrorDescription
            });
        }
        
    }
    
    public async Task Handle(CreateUserReply message)
    {
        if (!message.Success)
        {
            await MarkAsCompleted(CurrentSagaStatus.CompletedWithFailure, new SagaErrorDetails
            {
                WhereHappened = "CreateUserReply Handle",
                Description = "The reply message from authentication CreateUserIfNotExistsMessage indicates failure",
                Exception = message.ErrorDescription   
            });
            
            return;
        }
        
        Data.NewUserId = message.UserId;
        
        if (Data.AmCreatingNewLicensedTenant)
        {
            await _serviceBus.Send(new AddRootUserToTenantCommand
            {
                UserId = Data.NewUserId!.Value,
                TenantId = message.TenantId
            });
        }
        else
        {
            await RemoveRootUser(message);
        }
    }

    private async Task RemoveRootUser(CreateUserReply message)
    {
        try
        {
            var oldUserId = await GetUserIdByEmail(Data.OldUserEmail);
            if (oldUserId.GetValueOrDefault() == Guid.Empty)
            {
                await MarkAsCompleted(CurrentSagaStatus.CompletedWithFailure, new SagaErrorDetails
                {
                    WhereHappened = "CreateUserReply Handle",
                    Description = "OldUserId is Guid.Empty"
                });
                
                return;
            }

            await _serviceBus.Send(new RemoveRootUserCommand
            {
                TenantId = message.TenantId,
                UserId = oldUserId!.Value
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Houve um problema ao tentar buscar usuário relacionado ao antigo email de administrador");
            await MarkAsCompleted(CurrentSagaStatus.CompletedWithFailure, new SagaErrorDetails
            {
                WhereHappened = "CreateUserReply Handle",
                Description = "Unknown exception from authentication call GetUserIdByEmail" + message.Status,
                Exception = e.ToString()
            });
        }
    }

    private async Task MarkAsCompleted(CurrentSagaStatus sagaStatus, SagaErrorDetails errorDetails)
    {
        Data.CurrentSagaStatus = sagaStatus;
        await UpdateLicensedTenantStatusAndSagaInfo(errorDetails);
        await SendSagaStatusNotification();
        base.MarkAsComplete();
    }
    
    private async Task SendSagaStatusNotification()
    {
        await _notification.SendUpdateAsync(new LicensedTenantSagaStatusUpdateNotification
        {
            //currentTick é necessário pois as notifications podem chegar no front em ordem diferente da de envio 
            CurrentTick = _dateTimeProvider.GetUnixTimeTicks(),
            AmCreatingNewLicensedTenant = Data.AmCreatingNewLicensedTenant,
            Status = Data.CurrentSagaStatus,
            CurrentLicensedTenantStatus = Data.CurrentLicensedTenantStatus!.Value,
            NewEmail = Data.NewUserEmail
        });
    }
    
    private async Task UpdateLicensedTenantStatusAndSagaInfo(SagaErrorDetails errorDetails)
    {
        var licensedTenant = await _licensedTenants.Where(l => l.Identifier == Data.TenantId).FirstOrDefaultAsync();
        if (licensedTenant == null) return;

        var sagaInfo = new LicensedTenantSagaInfo
        {
            AmCreatingNewLicensedTenant = Data.AmCreatingNewLicensedTenant,
            Status = Data.CurrentSagaStatus,
            ErrorDetails = errorDetails
        };

        // somente mudamos o status do licenciamento para ativo na criação de um licenciamento novo pois é perigoso 
        // deixar o mesmo bloqueado sendo que já estava sendo utilizado
        if (Data.CurrentSagaStatus == CurrentSagaStatus.CompletedSuccessfully && Data.AmCreatingNewLicensedTenant)
        {
            licensedTenant.Status = LicensingStatus.Active;
        }

        await _licensedTenantService.UpdateTenantLicensing(new LicenseTenantUpdateInput
        {
            Id = licensedTenant.Id,
            AccountId = licensedTenant.AccountId,
            Status = licensedTenant.Status,
            Identifier = licensedTenant.Identifier,
            ExpirationDateTime = licensedTenant.ExpirationDateTime,
            LicenseConsumeType = licensedTenant.LicenseConsumeType,
            LicensedCnpjs = licensedTenant.LicensedCnpjs,
            AdministratorEmail = licensedTenant.AdministratorEmail,
            Notes = licensedTenant.Notes != null ? Encoding.UTF8.GetString(licensedTenant.Notes) : null,
            HardwareId = licensedTenant.HardwareId
        }, sagaInfo);
        
        Data.CurrentLicensedTenantStatus = licensedTenant.Status;
    }

    private async Task SetInitialData(Guid tenantId, string oldEmail, string newEmail, bool amCreatingNewLicensedTenant)
    {
        Data.SagaId = tenantId.ToString();
        Data.AmCreatingNewLicensedTenant = amCreatingNewLicensedTenant;
        Data.OldUserEmail = oldEmail;
        Data.NewUserEmail = newEmail;
        Data.TenantId = tenantId;
        Data.CurrentSagaStatus = CurrentSagaStatus.Processing;
        await UpdateLicensedTenantStatusAndSagaInfo(null);
        await SendSagaStatusNotification();
    }

    private async Task<Guid?> GetUserIdByEmail(string email)
    {
        var user = await _authenticationCaller.GetUserIdByEmail(email);
        return user?.Id;
    }
}