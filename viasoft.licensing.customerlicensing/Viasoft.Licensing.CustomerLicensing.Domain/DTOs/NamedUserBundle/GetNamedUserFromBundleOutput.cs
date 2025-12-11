using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserBundle
{
    public class GetNamedUserFromBundleOutput
    {
        public PagedResultDto<NamedUserBundleLicenseOutput> NamedUserBundleLicenseOutputs { get; set; }
        public GetNamedUserFromBundleValidationCode NamedUserFromBundleValidationCode { get; set; }
    }
}