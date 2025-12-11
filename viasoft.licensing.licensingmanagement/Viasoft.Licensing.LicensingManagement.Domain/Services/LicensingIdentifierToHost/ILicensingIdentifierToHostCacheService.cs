using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.AmbientData;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.HostTenantId;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.LicensingIdentifierToHost
{
    public interface ILicensingIdentifierToHostCacheService
    {
        Task<HostTenantIdOutput> GetHostTenantIdFromLicensingIdentifier(Guid licensingIdentifier, TenantIdParameterKind kind);
    }
}