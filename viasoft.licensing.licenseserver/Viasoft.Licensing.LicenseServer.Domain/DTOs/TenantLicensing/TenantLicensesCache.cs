using System;
using System.Text;
using Newtonsoft.Json;
using Viasoft.Licensing.LicenseServer.Domain.EntityFrameworkCore;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.TenantLicensing
{
    public class TenantLicensesCache
    {
        public TenantLicensesCache()
        {
            
        }

        public TenantLicensesCache(PersistedTenantLicensesCache input)
        {
            Id = input.Id;
            TenantLicenses = JsonConvert.DeserializeObject<TenantLicenses>(Encoding.UTF8.GetString(input.TenantLicenses));
            LogDateTime = input.LogDateTime;
        }
        
        public Guid Id { get; set; }
        public TenantLicenses TenantLicenses { get; set; }
        public DateTime LogDateTime { get; set;}
    }
}