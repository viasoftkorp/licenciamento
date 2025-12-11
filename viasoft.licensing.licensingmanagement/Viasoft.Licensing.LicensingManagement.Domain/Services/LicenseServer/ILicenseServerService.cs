using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.LicenseServer
{
    public interface ILicenseServerService
    {
        Task<LicenseByIdentifier> GetLicenseByIdentifier(Guid identifier);
        
        Task<List<LicenseByIdentifier>> GetLicensesByIdentifiers(List<Guid> identifiers);
    }
}