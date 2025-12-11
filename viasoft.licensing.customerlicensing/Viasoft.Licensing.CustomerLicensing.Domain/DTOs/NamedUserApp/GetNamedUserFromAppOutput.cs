using Viasoft.Core.DDD.Application.Dto.Paged;
using Viasoft.Licensing.CustomerLicensing.Domain.Enums;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.NamedUserApp
{
    public class GetNamedUserFromAppOutput
    {
        public PagedResultDto<NamedUserAppLicenseOutput> NamedUserAppLicenseOutputs { get; set; }
        public GetNamedUserFromLicensedAppValidationCode ValidationCode { get; set; }
    }
}