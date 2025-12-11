using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.Message
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.PopulateAccountMessage")]
    public class PopulateAccountMessage : IMessage, IInternalMessage
    {
    }
}