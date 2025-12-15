using System;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseCache
{
    public class LicenseByTenantIdCache
    {
        public Guid Id { get; set; }
        public LicenseByTenantIdOld LicenseByTenantId { get; set; }
        public DateTime LogDateTime { get; set;}
    }
}