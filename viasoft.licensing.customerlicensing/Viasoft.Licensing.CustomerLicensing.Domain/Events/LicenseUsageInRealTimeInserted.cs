using System;
using System.Collections.Generic;
using Viasoft.Core.ServiceBus.Abstractions;
using Viasoft.Licensing.CustomerLicensing.Domain.DTOs.LicenseUsageInRealTime;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Events
{
    [Endpoint("Viasoft.Licensing.CustomerLicensing.LicenseUsageInRealTimeInserted")]
    public class LicenseUsageInRealTimeInserted: IEvent, IInternalMessage
    {
        public Guid LicensingIdentifier { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public List<InsertedLicenseUsageInRealTime> InsertedLicensesUsages { get; set; }
    }
}