using System;
using System.Threading.Tasks;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseCache;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Initializer;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseCache
{
    public class LicenseCacheService: ILicenseCacheService, ISingletonDependency
    {
        public Task StoreLicenseByTenantId(LicenseByTenantIdOld licenseByTenantIdOld)
        {
            var input = new LicenseByTenantIdCache
            {
                Id = licenseByTenantIdOld.Identifier,
                LicenseByTenantId = licenseByTenantIdOld,
                LogDateTime = DateTime.UtcNow
            };
            
            using (var db = LiteDbInitializer.NewDatabase(LiteDbConsts.BuildTenantConnectionStringOld(input.Id)))
            {
                var licenseByTenantIdCollection = db.GetCollection<LicenseByTenantIdCache>(nameof(LicenseByTenantIdCache));
                licenseByTenantIdCollection.Upsert(input.Id, input);
            }
            return Task.CompletedTask;
        }

        public Task<LicenseByTenantIdOld> GetLicenseByTenantId(Guid tenantId)
        {
            using (var db = LiteDbInitializer.OldNewReadonlyRepository(tenantId))
            {
                var licenseByTenantIdCache = db.FirstOrDefault<LicenseByTenantIdCache>(l => l.Id == tenantId);
                
                if (licenseByTenantIdCache == null | (licenseByTenantIdCache != null && licenseByTenantIdCache.LogDateTime < DateTime.UtcNow.AddHours(-72)))
                    return Task.FromResult<LicenseByTenantIdOld>(null);
                
                return Task.FromResult(licenseByTenantIdCache.LicenseByTenantId);
            }
        }
    }
    
}