using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message;

[Endpoint("Viasoft.Licensing.LicensingManagement.UpdateAdministratorEmail", "Viasoft.Licensing.LicensingManagement")]
public class UpdateAdministratorEmailCommand : ICommand, IInternalMessage    
{
    public string OldEmail { get; set; }
    public string NewEmail { get; set; }
    public Guid TenantId { get; set; }
}