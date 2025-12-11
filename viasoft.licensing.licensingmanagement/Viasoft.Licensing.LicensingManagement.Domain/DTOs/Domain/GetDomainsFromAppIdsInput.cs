using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Domain
{
    public class GetDomainsFromAppIdsInput
    {
        public Guid LicensingIdentifier { get; set; }
        
        public List<string> AppsIds { get; set; }
        
    }
}