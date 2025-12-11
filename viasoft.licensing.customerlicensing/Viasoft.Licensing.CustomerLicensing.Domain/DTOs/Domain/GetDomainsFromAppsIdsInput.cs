using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Domain
{
    public class DomainsFromAppsIdsInput
    {
        public Guid LicensingIdentifier { get; set; }
        
        public List<string> AppsIds { get; set; }
    }
}