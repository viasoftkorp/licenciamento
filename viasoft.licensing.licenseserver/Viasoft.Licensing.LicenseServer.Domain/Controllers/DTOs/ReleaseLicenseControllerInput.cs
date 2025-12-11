using System.Text.RegularExpressions;
using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicenseServer.Domain.Controllers.DTOs
{
    public class ReleaseLicenseControllerInput
    {
        private string _cnpj;

        [StrictRequired(AllowEmptyStrings = false)]
        public string AppIdentifier { get; set; } 
        
        [StrictRequired(AllowEmptyStrings = false)]
        public string User { get; set; }
        public string Cnpj
        {
            get => _cnpj;
            set => _cnpj = Regex.Replace(value, @"\D", "");
        }
        
        [StrictRequired(AllowEmptyStrings = false)]
        public string Sid { get; set; }
    }
}