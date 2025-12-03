using System;
using System.Text.RegularExpressions;
using Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseUsage;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseConsumers
{
    public class ConsumeLicenseInput
    {
        private string _cnpj;
        
        public Guid TenantId { get; set; }
        
        public string AppIdentifier { get; set; }
        
        public string User { get; set; }

        public string Cnpj
        {
            get => _cnpj;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _cnpj = Regex.Replace(value, @"\D", "");
                }
            }
        }
        
        public string CustomAppName { get; set; }

        public string Token { get; set; }
        
        public bool IsTerminalServer { get; set; }
        
        public string DeviceId { get; set; }
        
        public LicenseUsageAdditionalInformation LicenseUsageAdditionalInformation { get; set; }
    }
}