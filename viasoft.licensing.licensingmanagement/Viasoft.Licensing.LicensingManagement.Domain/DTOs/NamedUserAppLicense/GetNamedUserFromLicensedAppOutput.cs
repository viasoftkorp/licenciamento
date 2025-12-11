using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserAppLicense
{
    public class GetNamedUserFromLicensedAppOutput
    {
        public PagedResultDto<NamedUserAppLicenseOutput> NamedUserAppLicenseOutputs { get; set; }
        public GetNamedUserFromLicensedAppValidationCode ValidationCode { get; set; }
    }
}