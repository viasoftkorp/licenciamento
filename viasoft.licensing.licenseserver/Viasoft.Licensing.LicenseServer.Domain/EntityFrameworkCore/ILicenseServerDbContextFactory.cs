using System;
using System.Threading.Tasks;

namespace Viasoft.Licensing.LicenseServer.Domain.EntityFrameworkCore;

public interface ILicenseServerDbContextFactory
{
    public Task<LicenseServerDbContext> ConstructedDbContext(Guid tenantId);
}