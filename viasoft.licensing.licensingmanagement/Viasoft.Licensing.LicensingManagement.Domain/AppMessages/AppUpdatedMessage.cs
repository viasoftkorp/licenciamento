using System;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.AppMessages
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.AppUpdated")]
    public class AppUpdatedMessage: IMessage, IInternalMessage
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Identifier { get; set; }

        public bool IsActive { get; set; }

        public Guid SoftwareId { get; set; }
        
        public bool Default { get; set; }
        
        public Enums.Domain Domain { get; set; }
    }
}