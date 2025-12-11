using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.HostTenantId;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.HostTenant
{
    public interface IHostTenantService
    {
        Task<HostTenantIdOutput> GetHostTenantIdFromLicensingIdentifier(Guid licensingIdentifier);
        Task<HostTenantIdOutput> GetHostTenantIdFromLicensedTenantId(Guid licensedTenantId);

    }
}