using System;
using System.Collections.Generic;
using Viasoft.Core.ServiceBus.Abstractions;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Message
{
    [Endpoint("Viasoft.Licensing.LicensingManagement.CreateLicensedTenant")]
    public class CreateLicensedTenantCommand: IMessage, IInternalMessage
    {
        public Guid AccountId { get; set; }
        
        public string LicensedCnpjs { get; set; }

        public string AdministratorEmail { get; set; }
        public List<Guid> BundleIds { get; set; }
        public int NumberOfLicenses { get; set; }
        public Guid LicensingIdentifier { get; set; }
    }
}