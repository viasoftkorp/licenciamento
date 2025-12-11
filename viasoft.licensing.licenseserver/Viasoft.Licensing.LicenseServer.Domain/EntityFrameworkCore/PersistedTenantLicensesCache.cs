using System;
using System.Text;
using Newtonsoft.Json;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.TenantLicensing;

namespace Viasoft.Licensing.LicenseServer.Domain.EntityFrameworkCore;

//this class was created based on TenantLicensesCache excluding complex types to support relational database usage
public class PersistedTenantLicensesCache
{
    public PersistedTenantLicensesCache()
    {
        
    }
    public PersistedTenantLicensesCache(TenantLicensesCache tenantLicensesCache)
    {
        Id = tenantLicensesCache.Id;
        Update(tenantLicensesCache);
    }
    
    public Guid Id { get; set; }
    public byte[] TenantLicenses { get; set; }
    public DateTime LogDateTime { get; set;}

    public void Update(TenantLicensesCache tenantLicensesCache)
    {
        TenantLicenses = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(tenantLicensesCache.TenantLicenses));
        LogDateTime = tenantLicensesCache.LogDateTime;
    }
}