using System;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseDetails
{
    public class TenantLicenseStatusRefreshInfo
    {
        public Guid TenantId { get; set; }
        
        public DateTime LastRefreshDateTime { get; set;}
        
        public bool RefreshSucceed { get; set;}
    }
}