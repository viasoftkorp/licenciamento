using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Viasoft.Core.IoC.Abstractions;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.CustomerLicensing;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.UserBehaviour;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;
using Viasoft.Licensing.LicenseServer.Shared.Consts;
using Viasoft.Licensing.LicenseServer.Shared.Initializer;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseUsage
{
    public class LicenseUsageService: ILicenseUsageService, ISingletonDependency
    {
        private readonly IMapper _mapper;

        public LicenseUsageService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public void StoreDoneUsageLog(StoreDoneUsageLog input)
        {
            var licenseUsage = _mapper.Map<LicenseUsageBehaviourDetails>(input);
            licenseUsage.LogDateTime = DateTime.UtcNow;
            licenseUsage.DurationInSeconds = (int) (licenseUsage.EndTime - licenseUsage.StartTime).TotalSeconds;
            
            using (var db = LiteDbInitializer.NewDatabase(LiteDbConsts.BuildTenantConnectionStringOld(input.TenantId)))
            {
                var licenseUsages = db.GetCollection<LicenseUsageBehaviourDetails>(nameof(LicenseUsageBehaviourDetails));
                licenseUsages.EnsureIndex(x => x.Id);
                licenseUsages.Insert(licenseUsage);
            }
        }

        public Task<List<LicenseUsageBehaviourDetails>> GetLicensesUsage(Guid tenantId)
        {
            using (var db = LiteDbInitializer.OldNewReadonlyRepository(tenantId))
                return Task.FromResult(db.Fetch<LicenseUsageBehaviourDetails>(x => x.TenantId == tenantId).OrderBy(l => l.LogDateTime).ToList());
        }

        public LicenseUsageInRealTime GetLastUploadedLicenseUsageInRealTime(Guid tenantId)
        {
            using (var db = LiteDbInitializer.OldNewReadonlyRepository(tenantId))
                return db.Fetch<LicenseUsageInRealTime>(x => x.TenantId == tenantId).FirstOrDefault();
        }

        public void StoreLastUploadedLicenseUsageInRealTime(LicenseUsageInRealTime licenseUsageInRealTime)
        {
            using (var db = LiteDbInitializer.NewDatabase(LiteDbConsts.BuildTenantConnectionStringOld(licenseUsageInRealTime.TenantId)))
            {
                var licenseByTenantIdCollection = db.GetCollection<LicenseUsageInRealTime>(nameof(LicenseUsageInRealTime));
                licenseByTenantIdCollection.Upsert(licenseUsageInRealTime.TenantId, licenseUsageInRealTime);
            }
        }
    }
}