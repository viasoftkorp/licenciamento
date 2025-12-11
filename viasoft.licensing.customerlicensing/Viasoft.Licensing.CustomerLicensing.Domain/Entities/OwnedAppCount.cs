using System;
using Viasoft.Core.DDD.Entities;

namespace Viasoft.Licensing.CustomerLicensing.Domain.Entities
{
    public class OwnedAppCount : Entity
    {
        public int Count { get; set; }
        public Guid LicensingIdentifier { get; set; }
    }
}