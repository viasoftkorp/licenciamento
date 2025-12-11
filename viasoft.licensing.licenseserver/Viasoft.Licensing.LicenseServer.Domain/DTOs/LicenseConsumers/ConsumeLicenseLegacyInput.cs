using System.Text.RegularExpressions;
using Viasoft.Data.Attributes;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers
{
    public class ConsumeLicenseLegacyInput
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
        
        public LicenseUsageAdditionalInformation LicenseUsageAdditionalInformation { get; set; }
    }
}