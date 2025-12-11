using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenantView.Command
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.DeleteLicensedTenant", "Viasoft.Licensing.LicensingManagement")]
    public class DeleteLicensedTenantCommand : ICommand, IInternalMessage
    {
        public Guid LicensedTenantId { get; set; }
    }
}