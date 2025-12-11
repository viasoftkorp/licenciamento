using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Host.Sagas.AdministratorEmail.Contracts;

[Endpoint("Viasoft.Licensing.LicensingManagement.CreateNewLicensing")]
public class CreateNewLicensingMessage : IMessage, IInternalMessage
{
    public string Email { get; set; }
    public Guid TenantId { get; set; }
    public Guid? AdministratorUserId { get; set; }
}