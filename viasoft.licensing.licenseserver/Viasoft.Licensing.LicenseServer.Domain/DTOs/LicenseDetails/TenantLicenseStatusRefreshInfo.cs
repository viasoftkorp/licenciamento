using System;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseDetails
{
    public class TenantLicenseStatusRefreshInfo
    {
        public Guid TenantId { get; set; }
        
        public DateTime LastRefreshDateTime { get; set;}
        
        public bool RefreshSucceed { get; set;}

        public void Update(TenantLicenseStatusRefreshInfo input)
        {
            LastRefreshDateTime = input.LastRefreshDateTime;
            RefreshSucceed = input.RefreshSucceed;
        }

        public static TenantLicenseStatusRefreshInfo RefreshFailed(Guid tenantId)
        {
            return new TenantLicenseStatusRefreshInfo{ LastRefreshDateTime = DateTime.UtcNow, RefreshSucceed = false, TenantId = tenantId};
        }
        
        public static TenantLicenseStatusRefreshInfo RefreshOk(Guid tenantId)
        {
            return new TenantLicenseStatusRefreshInfo{ LastRefreshDateTime = DateTime.UtcNow, RefreshSucceed = true, TenantId = tenantId};
        }
    }
}