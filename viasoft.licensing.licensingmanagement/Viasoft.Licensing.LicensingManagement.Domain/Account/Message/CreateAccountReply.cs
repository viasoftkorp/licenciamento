using System;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.Message
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.CreateAccountReply")]
    public class CreateAccountReply: IMessage, IInternalMessage
    {
        public string Message { get; set; }
        public CreateAccountEnum Code { get; set; }
        public Guid Id { get; set; }
    }
}