using System;
using System.Text.RegularExpressions;
using Viasoft.Data.Attributes;
using Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.DTOs.LicenseConsumers
{
    public class ConsumeLicenseInput
    {
        private string _cnpj;
        
        [StrictRequired]
        public Guid TenantId { get; set; }
        
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