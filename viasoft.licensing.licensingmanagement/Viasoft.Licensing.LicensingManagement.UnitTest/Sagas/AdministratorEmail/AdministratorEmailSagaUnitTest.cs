using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenantSaga;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Enum;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message;
using Viasoft.Licensing.LicensingManagement.Host.Sagas.AdministratorEmail.Data;
using Xunit;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Sagas.AdministratorEmail;

public class AdministratorEmailSagaUnitTest : LicensingManagementTestBase
{
    [Fact(DisplayName = "Testa o Handle UpdateAdministratorEmailMessage ao atualizar o email de administrador de um licenciamento")]
    public async Task UpdateAdministratorEmailMessage()
    {
        var (saga, utils) = AdministratorEmailSagaTestUtils.NewUtils(ServiceProvider);
        saga.Data = new AdministratorEmailSagaData();
        
        //act Handle(UpdateAdministratorEmailMessage message)
        await saga.Handle(utils.UpdateAdministratorEmailMessage);
        
        saga.Data.AssertSagaData(new AdministratorEmailSagaData
        {
            AmCreatingNewLicensedTenant = false,
            OldUserEmail = utils.UpdateAdministratorEmailMessage.OldEmail,
            TenantId = utils.UpdateAdministratorEmailMessage.TenantId,
            SagaId = utils.UpdateAdministratorEmailMessage.TenantId.ToString(),
            NewUserId = null,
            CurrentSagaStatus = CurrentSagaStatus.Processing
        });

        var expectedCreateUserCommand = new CreateUserIfNotExistsCommand
        {
            Email = utils.UpdateAdministratorEmailMessage.NewEmail,
            UserId = null,
            TenantId = utils.UpdateAdministratorEmailMessage.TenantId
        };
        
        utils.ServiceBusMock.Verify(b => b.Send(It.Is<CreateUserIfNotExistsCommand>(command =>
            command.Equals(expectedCreateUserCommand)), null));
        
        utils.ServiceBusMock.VerifyNoOtherCalls();
        utils.AuthenticationCaller.VerifyNoOtherCalls();
    }

    [Fact(DisplayName = "Testa o Handle CreateUserReply quando o email de administrador do licenciamento foi alterado e o antigo email tem um usuário existente")]
    public async Task CreateUserReplyWhenUpdateAdminEmailAndUserExists()
    {
        var (saga, utils) = AdministratorEmailSagaTestUtils.NewUtils(ServiceProvider, true);
        saga.Data = new AdministratorEmailSagaData
        {
            SagaId = utils.CreateUserReply.TenantId.ToString(),
            TenantId = utils.CreateUserReply.TenantId,
            NewUserId = null,
            OldUserEmail = utils.UpdateAdministratorEmailMessage.OldEmail,
            AmCreatingNewLicensedTenant = false,
            CurrentSagaStatus = CurrentSagaStatus.Processing
        };

        //act Handle(CreateUserReply message)
        await saga.Handle(utils.CreateUserReply);
        
        utils.ServiceBusMock.Verify(b => b.Send(It.Is<RemoveRootUserCommand>(c => 
            c.TenantId.Equals(utils.CreateUserReply.TenantId) && c.UserId.Equals(utils.GetUserOutput.Id)), null));
        
        utils.AuthenticationCaller.Verify(c => c.GetUserIdByEmail(It.Is<string>(p => p.Equals(saga.Data.OldUserEmail))));
        utils.AuthenticationCaller.VerifyNoOtherCalls();
        
        saga.Data.AssertSagaData(new AdministratorEmailSagaData
        {
            SagaId = utils.CreateUserReply.TenantId.ToString(),
            TenantId = utils.CreateUserReply.TenantId,
            NewUserId = utils.CreateUserReply.UserId,
            OldUserEmail = utils.UpdateAdministratorEmailMessage.OldEmail,
            AmCreatingNewLicensedTenant = false,
            CurrentSagaStatus = CurrentSagaStatus.Processing
        });
    }

    [Fact(DisplayName = "Testa o Handle CreateUserReply quando o email de administrador do licenciamento foi alterado e o antigo email não tem um usuário existente")]
    public async Task CreateUserReplyWhenUpdateAdminEmailAndUserNotExists()
    {
        var (saga, utils) = AdministratorEmailSagaTestUtils.NewUtils(ServiceProvider);

        saga.Data = new AdministratorEmailSagaData
        {
            SagaId = utils.CreateUserReply.TenantId.ToString(),
            TenantId = utils.CreateUserReply.TenantId,
            NewUserId = null,
            OldUserEmail = utils.UpdateAdministratorEmailMessage.OldEmail,
            AmCreatingNewLicensedTenant = false,
            CurrentSagaStatus = CurrentSagaStatus.Processing
        };
        
        await saga.Handle(utils.CreateUserReply);
        
        utils.ServiceBusMock.VerifyNoOtherCalls();
        
        utils.AuthenticationCaller.Verify(c => c.GetUserIdByEmail(It.Is<string>(p => p.Equals(saga.Data.OldUserEmail))));
        utils.AuthenticationCaller.VerifyNoOtherCalls();
        
        saga.Data.AssertSagaData(new AdministratorEmailSagaData
        {
            SagaId = utils.CreateUserReply.TenantId.ToString(),
            TenantId = utils.CreateUserReply.TenantId,
            NewUserId = utils.CreateUserReply.UserId,
            OldUserEmail = utils.UpdateAdministratorEmailMessage.OldEmail,
            AmCreatingNewLicensedTenant = false,
            CurrentSagaStatus = CurrentSagaStatus.CompletedWithFailure
        });
    }

    [Fact(DisplayName =
        "Testa o Handle RemoveRootUserReply quando o antigo RootUser foi removido com sucesso")]
    public async Task RemoveRootUserReplyWasSuccessful()
    {
        var (saga, utils) = AdministratorEmailSagaTestUtils.NewUtils(ServiceProvider);

        saga.Data = new AdministratorEmailSagaData
        {
            SagaId = utils.CreateUserReply.TenantId.ToString(),
            TenantId = utils.CreateUserReply.TenantId,
            NewUserId = utils.CreateUserReply.UserId,
            OldUserEmail = utils.UpdateAdministratorEmailMessage.OldEmail,
            AmCreatingNewLicensedTenant = false,
            CurrentSagaStatus = CurrentSagaStatus.Processing
        };
        
        //act Handle(AddRootUserToTenantReply message)
        await saga.Handle(utils.RemoveRootUserReply);
        
        utils.ServiceBusMock.Verify(b => b.Send(It.Is<AddRootUserToTenantCommand>(c => 
            c.TenantId.Equals(saga.Data.TenantId) && c.UserId.Equals(saga.Data.NewUserId)), null));
        utils.ServiceBusMock.VerifyNoOtherCalls();
        
        utils.AuthenticationCaller.VerifyNoOtherCalls();

        saga.Data.AssertSagaData(new AdministratorEmailSagaData
        {
            SagaId = utils.CreateUserReply.TenantId.ToString(),
            TenantId = utils.CreateUserReply.TenantId,
            NewUserId = utils.CreateUserReply.UserId,
            OldUserEmail = utils.UpdateAdministratorEmailMessage.OldEmail,
            AmCreatingNewLicensedTenant = false,
            CurrentSagaStatus = CurrentSagaStatus.Processing,
        });
    }

    [Fact(DisplayName =
        "Testa o Handle CreateNewLicensingMessage ao criar um novo licenciamento")]
    public async Task CreateNewLicensingMessage()
    {
        var (saga, utils) = AdministratorEmailSagaTestUtils.NewUtils(ServiceProvider);
        saga.Data = new AdministratorEmailSagaData();

        //act Handle(CreateNewLicensingMessage message)
        await saga.Handle(utils.CreateNewLicensingMessage);
        
        saga.Data.AssertSagaData(new AdministratorEmailSagaData
        {
            SagaId = utils.CreateNewLicensingMessage.TenantId.ToString(),
            TenantId = utils.CreateNewLicensingMessage.TenantId,
            NewUserId = null,
            OldUserEmail = null,
            AmCreatingNewLicensedTenant = true,
            CurrentSagaStatus = CurrentSagaStatus.Processing
        });

        var expectedCreateUserCommand = new CreateUserIfNotExistsCommand
        {
            Email = utils.CreateNewLicensingMessage.Email,
            UserId = utils.CreateNewLicensingMessage.AdministratorUserId,
            TenantId = utils.CreateNewLicensingMessage.TenantId
        };
        
        utils.ServiceBusMock.Verify(b => 
            b.Send(It.Is<CreateUserIfNotExistsCommand>(c => 
                c.Equals(expectedCreateUserCommand)), null));
        utils.ServiceBusMock.VerifyNoOtherCalls();
        
        utils.AuthenticationCaller.VerifyNoOtherCalls();
    }
    
    [Fact(DisplayName =
        "Testa o Handle CreateUserReply ao criar um novo licenciamento")]
    public async Task CreateUserReplyWhenCreatingNewLicensing()
    {
        var (saga, utils) = AdministratorEmailSagaTestUtils.NewUtils(ServiceProvider);
        
        saga.Data = new AdministratorEmailSagaData
        {
            SagaId = utils.CreateUserReply.TenantId.ToString(),
            TenantId = utils.CreateUserReply.TenantId,
            NewUserId = null,
            OldUserEmail = null,
            AmCreatingNewLicensedTenant = true,
            CurrentSagaStatus = CurrentSagaStatus.Processing
        };
        
        //act Handle(CreateUserReply message)
        await saga.Handle(utils.CreateUserReply);
        
        saga.Data.AssertSagaData(new AdministratorEmailSagaData
        {
            SagaId = utils.CreateUserReply.TenantId.ToString(),
            TenantId = utils.CreateUserReply.TenantId,
            NewUserId = utils.CreateUserReply.UserId,
            OldUserEmail = null,
            AmCreatingNewLicensedTenant = true,
            CurrentSagaStatus = CurrentSagaStatus.Processing
        });
        
        utils.ServiceBusMock.Verify(b => 
            b.Send(It.Is<AddRootUserToTenantCommand>(c =>
                c.UserId.Equals(utils.CreateUserReply.UserId) && c.TenantId.Equals(utils.CreateUserReply.TenantId)), null));
        utils.ServiceBusMock.VerifyNoOtherCalls();
        
        utils.AuthenticationCaller.VerifyNoOtherCalls();
    }

    [Fact(DisplayName =
        "Testa o Handle AddRootUserToTenantReply")]
    public async Task AddRootUserToTenantReply()
    {
        var (saga, utils) = AdministratorEmailSagaTestUtils.NewUtils(ServiceProvider);
        
        saga.Data = new AdministratorEmailSagaData
        {
            SagaId = utils.CreateUserReply.TenantId.ToString(),
            TenantId = utils.CreateUserReply.TenantId,
            NewUserId = utils.CreateUserReply.UserId,
            OldUserEmail = utils.UpdateAdministratorEmailMessage.OldEmail,
            AmCreatingNewLicensedTenant = true,
            CurrentSagaStatus = CurrentSagaStatus.Processing
        };
        
        //act Handle(AddRootUserToTenantReply message)
        await saga.Handle(new AddRootUserToTenantReply
        {
            UserId = default,
            TenantId = default,
            Success = true
        });

        var licensedTenant = await utils.LicensedTenantsRepo.FirstOrDefaultAsync();
        licensedTenant!.Status.Should().Be(LicensingStatus.Active);
        utils.ServiceBusMock.VerifyNoOtherCalls();
        utils.AuthenticationCaller.VerifyNoOtherCalls();
        saga.Data.AssertSagaData(new AdministratorEmailSagaData
        {
            SagaId = utils.CreateUserReply.TenantId.ToString(),
            TenantId = utils.CreateUserReply.TenantId,
            NewUserId = utils.CreateUserReply.UserId,
            OldUserEmail = utils.UpdateAdministratorEmailMessage.OldEmail,
            AmCreatingNewLicensedTenant = true,
            CurrentSagaStatus = CurrentSagaStatus.CompletedSuccessfully
        });
    }
}