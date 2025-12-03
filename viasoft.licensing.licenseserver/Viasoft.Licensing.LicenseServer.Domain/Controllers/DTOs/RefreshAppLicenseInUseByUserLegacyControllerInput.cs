using System.Text.RegularExpressions;
using Viasoft.Data.Attributes;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.Domain.Controllers.DTOs
{
    public class RefreshAppLicenseInUseByUserLegacyControllerInput
    {
        private string _cnpj;

        [StrictRequired(AllowEmptyStrings = false)]
        public string AppIdentifier { get; set; }

        [StrictRequired(AllowEmptyStrings = false)]
        public string Cnpj
        {
            get => _cnpj;
            set => _cnpj = Regex.Replace(value, @"\D", "");
        }
        
        public string CustomAppName { get; set; }
        
        [StrictRequired(AllowEmptyStrings = false)]
        public string Sid { get; set; }
        
        [StrictRequired]
        public bool IsTerminalServer { get; set; }

        public LicenseUsageAdditionalInformation LicenseUsageAdditionalInformation { get; set; }
    }
}