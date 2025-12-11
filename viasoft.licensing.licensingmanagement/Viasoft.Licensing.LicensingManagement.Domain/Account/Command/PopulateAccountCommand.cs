using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.Command
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.PopulateAccountMessage", "Viasoft.Licensing.LicensingManagement")]
    public class PopulateAccountCommand: ICommand, IInternalMessage
    {
    }
}