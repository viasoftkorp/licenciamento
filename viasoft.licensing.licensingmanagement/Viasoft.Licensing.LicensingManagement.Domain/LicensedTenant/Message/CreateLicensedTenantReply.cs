using System;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.CreateLicensedTenantReply")]
    public class CreateLicensedTenantReply: IMessage, IInternalMessage
    {
        public string Message { get; set; }
        public Guid LicensingIdentifier { get; set; }
        public OperationValidation Code { get; set; }
    }
}