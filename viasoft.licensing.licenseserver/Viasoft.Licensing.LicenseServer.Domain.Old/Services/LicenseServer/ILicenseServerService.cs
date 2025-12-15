using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicenseServer.Domain.Old.Contracts.LicensedTenant;
using Viasoft.Licensing.LicenseServer.Shared.Contracts.HardwareId;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Services.LicenseServer
{
    public interface ILicenseServerService
    {
        Task<LicenseByTenantIdOld> GetLicenseByTenantId(Guid tenantId);
        
        Task<UpdateHardwareIdOutput> UpdateHardwareId(UpdateHardwareIdInput input);
    }
}