using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.LicensingManagement.Domain.Enums;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.NamedUserBundleLicense
{
    public class GetNamedUserFromBundleOutput
    {
        public PagedResultDto<NamedUserBundleLicenseOutput> NamedUserBundleLicenseOutputs { get; set; }
        public GetNamedUserFromBundleValidationCode NamedUserFromBundleValidationCode { get; set; }
    }
}