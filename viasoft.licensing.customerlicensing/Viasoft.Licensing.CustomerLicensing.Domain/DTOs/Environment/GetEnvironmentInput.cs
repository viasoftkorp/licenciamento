using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Environment;

public class GetEnvironmentInput
{
    public List<Guid> TenantIds { get; set; }
}