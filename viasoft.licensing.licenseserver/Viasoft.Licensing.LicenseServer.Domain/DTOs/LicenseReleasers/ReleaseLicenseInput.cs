using System;
using System.Text.RegularExpressions;

namespace Viasoft.Licensing.LicenseServer.Domain.DTOs.LicenseReleasers
{
    public class ReleaseLicenseInput
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
        
        public string Token { get; set; }
        
        public bool IsTerminalServer { get; set; }
    }
}