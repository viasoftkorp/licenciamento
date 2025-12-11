using System;
using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensedTenant;
using Viasoft.Licensing.LicensingManagement.Domain.DTOs.LicensingLicenses;

namespace Viasoft.Licensing.LicensingManagement.Domain.Services.LicensingLicenses
{
    public interface IProvideLicensingLicensesService
    {
        public Task<LicensesOutput> GetLicensingLicenses(Guid identifier);
        public Task<UpdateHardwareIdOutput> UpdateHardwareId(Guid identifier, UpdateHardwareIdInput input);
    }
}