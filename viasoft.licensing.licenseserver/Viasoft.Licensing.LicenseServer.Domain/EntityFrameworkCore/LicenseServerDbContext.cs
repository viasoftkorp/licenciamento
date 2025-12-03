using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Viasoft.Licensing.LicenseServer.Domain.Contracts.UserBehaviour;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseDetails;

namespace Viasoft.Licensing.LicenseServer.Domain.EntityFrameworkCore;

public class LicenseServerDbContext : DbContext
{
    public virtual DbSet<LicenseUsageBehaviourDetails> LicenseUsageBehaviourDetails { get; set; }
    public virtual DbSet<TenantLicenseStatusRefreshInfo> TenantLicenseStatusRefreshInfos { get; set; }
    public virtual DbSet<PersistedTenantLicensesCache> TenantLicensesCaches { get; set; }
    public virtual DbSet<PersistedLicenseUsageInRealTimeOutput> LicenseUsageInRealTimeOutputs { get; set; }
    
    public string DbPath { get; }
    public static string DefaultDirectory => "Db";
    private static string DbExtension => ".db";
    private static string DbPrefix => "V3_LicensedTenant_";

    public LicenseServerDbContext(Guid tenantId) => DbPath = GetFileName(tenantId, DbPrefix);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LicenseUsageBehaviourDetails>()
            .HasIndex(e => e.Id);

        modelBuilder.Entity<TenantLicenseStatusRefreshInfo>()
            .HasKey(e => e.TenantId);
        
        modelBuilder.Entity<PersistedLicenseUsageInRealTimeOutput>()
            .HasKey(e => e.TenantId);

        base.OnModelCreating(modelBuilder);
    }

    private static string GetFileName(Guid tenantId, string prefix)
    {
        Directory.CreateDirectory(DefaultDirectory);
        return Path.Combine(DefaultDirectory, $"{prefix}{tenantId.ToString().ToUpper()}{DbExtension}");
    }
}