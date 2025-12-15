using System.Text.RegularExpressions;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage
{
    public class RefreshLegacyAppLicenseInUseByUserInput
    {
        private string _cnpj;

        [StrictRequired(AllowEmptyStrings = false)]
        public string DatabaseName { get; set; }
        
        [StrictRequired(AllowEmptyStrings = false)]
        public string AppIdentifier { get; set; }
        
        [StrictRequired(AllowEmptyStrings = false)]
        public string User { get; set; }
        
        [StrictRequired(AllowEmptyStrings = false)]
        public string Cnpj
        {
            get => _cnpj;
            set => _cnpj = Regex.Replace(value, @"\D", "");
        }
        
        public string CustomAppName { get; set; }
        
        public LicenseUsageAdditionalInformationOld LicenseUsageAdditionalInformation { get; set; }
    }
}