using System;
using System.Collections.Generic;
using Viasoft.Core.DDD.Application.Dto.Paged;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.App
{
    public class GetAllAppsInput : PagedFilteredAndSortedRequestInput
    {
        public Guid SoftwareId { get; set; }
        
        public List<Guid> AlreadyLicensedApps {get; set;}
    }
}