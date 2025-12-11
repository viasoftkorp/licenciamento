using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.Account
{
    public class AccountDetails
    {
        
        public string CompanyName { get; set; }
        
        public string Email { get; set; }
        
        public AccountStatus Status {get; set; }
        
        public string Phone { get; set; }
        
        public string WebSite { get; set; }
        
        public string CnpjCpf { get; set; }
        
        public string TradingName { get; set; }

    }
}