using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Viasoft.Core.EntityFrameworkCore.Context;
using Viasoft.Core.Storage.Schema;
using Viasoft.Licensing.CustomerLicensing.Domain.Entities;

namespace Viasoft.Licensing.CustomerLicensing.Infrastructure.EntityFrameworkCore
{
    public class ViasoftCustomerLicensingDbContext: BaseDbContext
    {
        public ViasoftCustomerLicensingDbContext(DbContextOptions options, ISchemaNameProvider schemaNameProvider, 
            ILoggerFactory loggerFactory, IBaseDbContextConfigurationService configurationService) 
            : base(options, schemaNameProvider, loggerFactory, configurationService)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var entityTypeBuilder = modelBuilder.Entity<LicenseUsageInRealTime>();
                
            entityTypeBuilder.HasIndex(l => new { TenantId = l.LicensingIdentifier, l.User });
            entityTypeBuilder.HasIndex(l => l.LicensingIdentifier);
        }
        
        public DbSet<LicenseUsageInRealTime> LicenseUsageInRealTime { get; set; }
        public DbSet<OwnedAppCount> OwnedAppCount { get; set; }
    }
}