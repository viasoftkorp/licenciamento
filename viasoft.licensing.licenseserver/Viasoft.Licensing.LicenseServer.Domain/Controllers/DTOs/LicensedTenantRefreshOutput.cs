using System;

namespace Viasoft.Licensing.LicenseServer.Domain.Controllers.DTOs
{
    public class LicensedTenantRefreshOutput
    {
        public DateTime? LastRefreshDateTime { get; set;}
        public bool? RefreshSucceed { get; set;}
    }
}