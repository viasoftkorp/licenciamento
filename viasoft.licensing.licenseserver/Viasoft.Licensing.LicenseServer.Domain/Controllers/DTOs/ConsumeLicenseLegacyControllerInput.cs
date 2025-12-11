using Viasoft.Data.Attributes;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.Domain.Controllers.DTOs
{
    public class ConsumeLicenseLegacyControllerInput
    {
        [StrictRequired(AllowEmptyStrings = false)]
        public string AppIdentifier { get; set; }
        
        [StrictRequired(AllowEmptyStrings = false)]
        public string User { get; set; }

        [StrictRequired(AllowEmptyStrings = false)]
        public string Cnpj { get; set; }

        public string CustomAppName { get; set; }

        [StrictRequired(AllowEmptyStrings = false)]
        public string Sid { get; set; }
        
        [StrictRequired]
        public bool IsTerminalServer { get; set; }
        
        public LicenseUsageAdditionalInformation LicenseUsageAdditionalInformation { get; set; }
    }
}