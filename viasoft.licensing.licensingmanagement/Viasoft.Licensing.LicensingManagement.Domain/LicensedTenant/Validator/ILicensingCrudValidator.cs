using System.Threading.Tasks;
using Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.DTO;

namespace Viasoft.Licensing.LicensingManagement.Domain.LicensedTenant.Validator
{
    public interface ILicensingCrudValidator
    {
        Task<(bool isValid, LicenseTenantCreateOutput output)> ValidateLicensingForCreate(LicenseTenantCreateInput input);
        Task<(bool isValid, LicenseTenantUpdateOutput output)> ValidateLicensingForUpdate(LicenseTenantUpdateInput input);
    }
}