using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Viasoft.Core.Authentication.Proxy.Dtos.Outputs;
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
using Viasoft.Licensing.LicensingManagement.Host.Sagas.AdministratorEmail;
using Viasoft.Licensing.LicensingManagement.Host.Sagas.AdministratorEmail.Contracts;
using Viasoft.PushNotifications.Abstractions.Notification;

namespace Viasoft.Licensing.LicensingManagement.UnitTest.Sagas.AdministratorEmail;

public class AdministratorEmailSagaTestUtils
{
    private static readonly ILogger<AdministratorEmailSaga> Logger = new NullLogger<AdministratorEmailSaga>();
    public Mock<IServiceBus> ServiceBusMock { get;}
    public Mock<IAuthenticationCaller> AuthenticationCaller { get;}
    public IRepository<LicensedTenant> LicensedTenantsRepo { get;}
    public Mock<IPushNotification> NotificationMock { get;}
    public Mock<IDateTimeProvider> DateTimeProviderMock { get; }
    public Mock<ILicensedTenantService> LicensedTenantServiceMock { get; }

    public LicensedTenant LicensedTenant { get; }
    public UpdateAdministratorEmailMessage UpdateAdministratorEmailMessage { get; }
    public CreateNewLicensingMessage CreateNewLicensingMessage { get; }
    public CreateUserReply CreateUserReply { get; }
    public GetUserOutput  GetUserOutput { get; }
    public RemoveRootUserReply  RemoveRootUserReply { get; }

    public static (AdministratorEmailSaga, AdministratorEmailSagaTestUtils) NewUtils(IServiceProvider serviceProvider, bool needUserFromAuth = false)
    {
        var utils = new AdministratorEmailSagaTestUtils(serviceProvider);
        var user = needUserFromAuth ? utils.GetUserOutput : null;
        utils.AuthenticationCaller.Setup(c => c.GetUserIdByEmail(It.IsAny<string>())).ReturnsAsync(user);
        utils.LicensedTenantsRepo.InsertAsync(utils.LicensedTenant, true);
        var saga = new AdministratorEmailSaga(utils.ServiceBusMock.Object, utils.AuthenticationCaller.Object, utils.LicensedTenantsRepo, utils.NotificationMock.Object, Logger, utils.DateTimeProviderMock.Object, utils.LicensedTenantServiceMock.Object);
        return (saga, utils);
    }

    private AdministratorEmailSagaTestUtils(IServiceProvider serviceProvider)
    {
        var tenantId = Guid.Parse("37f22505-087d-4a0e-8a9d-d8a1e64ec51f");

        LicensedTenant = new LicensedTenant
        {
            Id = Guid.NewGuid(),
            Status = LicensingStatus.Blocked,
            Identifier = tenantId
        };

        UpdateAdministratorEmailMessage = new UpdateAdministratorEmailMessage
        {
            OldEmail = "old@email.com",
            NewEmail = "new@email.com",
            TenantId = tenantId
        };

        CreateUserReply = new CreateUserReply
        {
            Status = "UserAlreadyExists",
            UserId = Guid.Parse("a37e0d11-8182-40ec-b52e-16c78ece5f7d"),
            TenantId = tenantId,
            Success = true
        };

        GetUserOutput = new GetUserOutput
        {
            Id = Guid.Parse("5dbad7a5-1844-4bb0-aee4-1209a965f4bc"),
            IsActive = true
        };

        RemoveRootUserReply = new RemoveRootUserReply
        {
            UserId = Guid.Parse("5dbad7a5-1844-4bb0-aee4-1209a965f4bc"),
            TenantId = tenantId,
            Success = true
        };

        CreateNewLicensingMessage = new CreateNewLicensingMessage
        {
            Email = "new@email.com",
            TenantId = tenantId,
            AdministratorUserId = null,
        };

        ServiceBusMock = new Mock<IServiceBus>();
        AuthenticationCaller = new Mock<IAuthenticationCaller>();
        NotificationMock = new Mock<IPushNotification>();
        DateTimeProviderMock = new Mock<IDateTimeProvider>();
        LicensedTenantServiceMock = new Mock<ILicensedTenantService>();
        LicensedTenantServiceMock
            .Setup(s => s.UpdateTenantLicensing(It.IsAny<LicenseTenantUpdateInput>(), It.IsAny<LicensedTenantSagaInfo>()))
            .ReturnsAsync(new LicenseTenantUpdateOutput());
        LicensedTenantsRepo = serviceProvider.GetRequiredService<IRepository<LicensedTenant>>();
    }
}

