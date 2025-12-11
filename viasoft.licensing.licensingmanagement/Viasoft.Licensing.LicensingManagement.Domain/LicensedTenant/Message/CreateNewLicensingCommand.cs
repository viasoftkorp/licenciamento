using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message;

[Endpoint("Viasoft.Licensing.LicensingManagement.CreateNewLicensing", "Viasoft.Licensing.LicensingManagement")]
public class CreateNewLicensingCommand : ICommand
{
    public string Email { get; set; }
    public Guid TenantId { get; set; }
    public Guid? AdministratorUserId { get; set; }
}