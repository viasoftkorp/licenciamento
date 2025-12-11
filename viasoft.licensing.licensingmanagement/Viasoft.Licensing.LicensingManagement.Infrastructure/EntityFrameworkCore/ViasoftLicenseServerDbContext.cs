using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Viasoft.Core.EntityFrameworkCore.Context;
using Viasoft.Core.Storage.Schema;
using Viasoft.Licensing.LicensingManagement.Domain.Entities;
using Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota;
using FileTenantQuota = Viasoft.Licensing.LicensingManagement.Domain.Entities.FileQuota.FileTenantQuota;

namespace Viasoft.Licensing.LicensingManagement.Infrastructure.EntityFrameworkCore
{
    public class ViasoftLicenseServerDbContext: BaseDbContext
    {
        public ViasoftLicenseServerDbContext(DbContextOptions options, ISchemaNameProvider schemaNameProvider, ILoggerFactory loggerFactory, IBaseDbContextConfigurationService configurationService) : base(options, schemaNameProvider, loggerFactory, configurationService)
        {
        }

        private void CreateUniqueIndexes(ModelBuilder modelBuilder)
        {
            var isDeleted = "[IsDeleted] = 0";

            modelBuilder.Entity<Account>().HasIndex(a => a.CnpjCpf)
                .HasFilter(isDeleted)
                .IsUnique();
            
            modelBuilder.Entity<App>().HasIndex(a => new {a.Identifier, a.TenantId})
                .HasFilter(isDeleted)
                .IsUnique();
            
            modelBuilder.Entity<Bundle>().HasIndex(b => new {b.Identifier, b.TenantId})
                .HasFilter(isDeleted)
                .IsUnique();
            
            modelBuilder.Entity<BundledApp>().HasIndex(b => new {b.BundleId, b.AppId, b.TenantId})
                .HasFilter(isDeleted)
                .IsUnique();
            
            modelBuilder.Entity<InfrastructureConfiguration>().HasIndex(i => i.LicensedTenantId)
                .HasFilter(isDeleted)
                .IsUnique();
            
            modelBuilder.Entity<LicensedApp>().HasIndex(l => new {l.TenantId, l.LicensedTenantId, l.AppId})
                .HasFilter(isDeleted)
                .IsUnique();
            
            modelBuilder.Entity<LicensedApp>().HasIndex(l => new {l.LicensedTenantId, l.TenantId})
                .HasFilter(isDeleted)
                .IncludeProperties(nameof(LicensedApp.AppId));
            
            modelBuilder.Entity<LicensedBundle>().HasIndex(l => new {l.TenantId, l.LicensedTenantId, l.BundleId})
                .HasFilter(isDeleted)
                .IsUnique();
            
            modelBuilder.Entity<LicensedTenant>().HasIndex(l => new {l.Identifier, l.TenantId})
                .HasFilter(isDeleted)
                .IsUnique();
            
            modelBuilder.Entity<LicensedTenant>().HasIndex(l => new {l.AccountId, l.TenantId})
                .HasFilter(isDeleted)
                .IsUnique();
            
            modelBuilder.Entity<LicensedTenant>().HasIndex(l => l.AdministratorEmail)
                .HasFilter(isDeleted)
                .IsUnique();
            
            modelBuilder.Entity<Software>().HasIndex(s => new {s.TenantId, s.Identifier})
                .HasFilter(isDeleted)
                .IsUnique();
            
            modelBuilder.Entity<FileAppQuotaView>().HasIndex(f => new {f.LicensedTenantId, f.AppId})
                .IsUnique();
            
            modelBuilder.Entity<FileTenantQuota>().HasIndex(f => f.LicenseTenantId)
                .IsUnique();

            modelBuilder.Entity<LicensedTenantSettings>()
                .HasIndex(licensedTenantSettings => new
                {
                    licensedTenantSettings.Key,
                    licensedTenantSettings.LicensingIdentifier,
                    licensedTenantSettings.TenantId
                })
                .HasFilter(isDeleted)
                .IsUnique();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AuditingLog>().Metadata.GetProperties().First(p => p.Name == nameof(Domain.Entities.AuditingLog.Details)).SetMaxLength(null);
            modelBuilder.Entity<LicensedTenant>().Metadata.GetProperties().First(p => p.Name == nameof(LicensedTenant.LicensedCnpjs)).SetMaxLength(null);
            modelBuilder.Entity<LicensedTenantView>().Metadata.GetProperties().First(p => p.Name == nameof(LicensedTenantView.LicensedCnpjs)).SetMaxLength(null);
            CreateUniqueIndexes(modelBuilder);
            
            modelBuilder.Entity<LicensedTenant>().Metadata.GetProperties().First(p => p.Name == nameof(LicensedTenant.HardwareId)).SetMaxLength(null);
            modelBuilder.Entity<LicensedTenantView>().Metadata.GetProperties().First(p => p.Name == nameof(LicensedTenantView.HardwareId)).SetMaxLength(null);
            modelBuilder.Entity<NamedUserBundleLicense>().Metadata.GetProperties().First(p => p.Name == nameof(NamedUserBundleLicense.DeviceId)).SetMaxLength(null);
            modelBuilder.Entity<NamedUserAppLicense>().Metadata.GetProperties().First(p => p.Name == nameof(NamedUserAppLicense.DeviceId)).SetMaxLength(null);
            
        }

        public DbSet<Software> Softwares { get; set; }
        public DbSet<Bundle> Bundles { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<BundledApp> BundledApps { get; set; }
        public DbSet<LicensedTenant> LicensedTenants { get; set; }
        public DbSet<LicensedBundle> LicensedBundles { get; set; }
        public DbSet<LicensedApp> LicensedApps { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<LicensedTenantView> LicensedTenantViews { get; set; }
        public DbSet<AuditingLog> AuditingLog { get; set; }
        public DbSet<FileTenantQuota> TenantQuotas { get; set; }
        public DbSet<FileAppQuotaView> AppQuotas { get; set; }
        public DbSet<InfrastructureConfiguration> InfrastructureConfigurations { get; set; }
        public DbSet<NamedUserBundleLicense> NamedUserBundleLicenses { get; set; }
        public DbSet<NamedUserAppLicense> NamedUserAppLicenses { get; set; }
        public DbSet<LicensedTenantSettings> LicensedTenantSettings { get; set; }
    }
}