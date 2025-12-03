using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Viasoft.Core.IoC.Abstractions;

namespace Viasoft.Licensing.LicenseServer.Domain.EntityFrameworkCore;

public class LicenseServerDbContextFactory : ILicenseServerDbContextFactory, ISingletonDependency
{
    private readonly ConcurrentDictionary<Guid, bool> _migratedDbContexts;

    public LicenseServerDbContextFactory()
    {
        _migratedDbContexts = new ConcurrentDictionary<Guid, bool>();
    }

    public async Task<LicenseServerDbContext> ConstructedDbContext(Guid tenantId)
    {
        var dbContext = new LicenseServerDbContext(tenantId);
        
        // controle simples para saber se o database já está migrado ou não
        // a príncipio o EF CORE vai tratar requests concorrentes
        if (!_migratedDbContexts.ContainsKey(tenantId))
        {
            await dbContext.Database.EnsureCreatedAsync();
            _migratedDbContexts.TryAdd(tenantId, true);
        }
        
        return dbContext;
    }
}