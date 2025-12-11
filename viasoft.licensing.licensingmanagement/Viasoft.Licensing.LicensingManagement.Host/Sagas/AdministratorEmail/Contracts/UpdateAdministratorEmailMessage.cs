using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Host.Sagas.AdministratorEmail.Contracts;

[Endpoint("Viasoft.Licensing.LicensingManagement.UpdateAdministratorEmail")]
public class UpdateAdministratorEmailMessage : IMessage, IInternalMessage 
{
    public string OldEmail { get; set; }
    public string NewEmail { get; set; }
    public Guid TenantId { get; set; }
}