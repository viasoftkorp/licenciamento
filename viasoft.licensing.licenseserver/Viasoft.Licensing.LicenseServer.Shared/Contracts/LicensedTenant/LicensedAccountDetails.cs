namespace Viasoft.Licensing.LicenseServer.Shared.Contracts.LicensedTenant
{
    public class LicensedAccountDetails
    {
        private LicensedAccountDetails(LicensedAccountDetails licensedAccountDetails)
        {
            Email = licensedAccountDetails.Email;
            Phone = licensedAccountDetails.Phone;
            Status = licensedAccountDetails.Status;
            CnpjCpf = licensedAccountDetails.CnpjCpf;
            CompanyName = licensedAccountDetails.CompanyName;
            TradingName = licensedAccountDetails.TradingName;
            WebSite = licensedAccountDetails.WebSite;
        }

        public LicensedAccountDetails()
        {
            
        }
        
        public string CompanyName { get; set; }
        
        public string Email { get; set; }
        
        public LicensedAccountStatusEnum Status {get; set; }
        
        public string Phone { get; set; }
        
        public string WebSite { get; set; }
        
        public string CnpjCpf { get; set; }
        
        public string TradingName { get; set; }
        
        public LicensedAccountDetails Clone()
        {
            var licensedAccountDetails = new LicensedAccountDetails(this);
            return licensedAccountDetails;
        }
    }
}